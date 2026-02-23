import axios from "axios";
import type {
  AxiosInstance,
  InternalAxiosRequestConfig,
  AxiosError,
  AxiosResponse,
  CreateAxiosDefaults,
} from "axios";
import type { QueueItem, RefreshTokenResponse } from "./types.ts";
import { RoutesPath } from "../router/routes.ts";

const API_URL: string = import.meta.env.VITE_BASE_URL;
console.log("API URL:", API_URL);

// Переменные для отслеживания процесса обновления токена
let isRefreshing: boolean = false;
let failedQueue: QueueItem[] = [];

const processQueue = (
  error: Error | null,
  token: string | null = null,
): void => {
  failedQueue.forEach((prom) => {
    if (error) {
      prom.reject(error);
    } else {
      prom.resolve(token);
    }
  });

  failedQueue = [];
};

// Создаем экземпляр axios с типами
const createApiClient = (baseURL: string): AxiosInstance => {
  const config: CreateAxiosDefaults = {
    baseURL,
  };

  const instance: AxiosInstance = axios.create(config);

  // Request interceptor
  instance.interceptors.request.use(
    (config: InternalAxiosRequestConfig): InternalAxiosRequestConfig => {
      const token: string | null = localStorage.getItem("token");

      if (token && config.headers) {
        config.headers.Authorization = `Bearer ${token}`;
      }

      return config;
    },
    (error: AxiosError): Promise<AxiosError> => {
      return Promise.reject(error);
    },
  );

  // Response interceptor
  instance.interceptors.response.use(
    (response: AxiosResponse): AxiosResponse => {
      return response;
    },
    async (error: AxiosError<unknown>): Promise<unknown> => {
      const originalRequest = error.config as InternalAxiosRequestConfig & {
        _retry?: boolean;
      };

      // Если ошибка не 401 или запрос уже был повторен
      if (error.response?.status !== 401 || originalRequest._retry) {
        return Promise.reject(error);
      }

      // Если это запрос на обновление токена и он тоже вернул 401
      if (originalRequest.url === "/auth/refresh") {
        // Очищаем токены и перенаправляем на страницу логина
        localStorage.removeItem("token");
        localStorage.removeItem("user");
        window.location.href = `/${RoutesPath.Auth}`;
        return Promise.reject(error);
      }

      if (isRefreshing) {
        // Если токен уже обновляется, добавляем запрос в очередь
        return new Promise((resolve, reject) => {
          failedQueue.push({ resolve, reject });
        })
          .then((token: unknown) => {
            originalRequest.headers.Authorization = `Bearer ${token as string}`;
            return instance(originalRequest);
          })
          .catch((err: Error) => {
            return Promise.reject(err);
          });
      }

      originalRequest._retry = true;
      isRefreshing = true;

      try {
        const jwtToken: string | null = localStorage.getItem("token");

        if (!jwtToken) {
          throw new Error("No refresh token available");
        }

        // Запрос на обновление токена
        const response = await axios.post<RefreshTokenResponse>(
          `${API_URL}/identity/refresh`,
          { jwtToken },
        );

        const { token } = response.data;

        // Сохраняем новые токены
        localStorage.setItem("token", token);

        // Обновляем заголовок авторизации
        instance.defaults.headers.common["Authorization"] = `Bearer ${token}`;

        // Обрабатываем очередь ожидающих запросов
        processQueue(null, token);

        // Повторяем оригинальный запрос
        originalRequest.headers.Authorization = `Bearer ${token}`;
        return instance(originalRequest);
      } catch (refreshError) {
        // Ошибка обновления токена
        processQueue(refreshError as Error, null);

        // Очищаем токены
        localStorage.removeItem("token");
        localStorage.removeItem("user");

        // Перенаправляем на страницу логина
        window.location.href = `/${RoutesPath.Auth}`;

        return Promise.reject(refreshError);
      } finally {
        isRefreshing = false;
      }
    },
  );

  return instance;
};

// Создаем экземпляр API клиента
export const api: AxiosInstance = createApiClient(API_URL);
