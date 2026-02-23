import type {
  AuthCredentials,
  AuthResponse,
  RefreshTokenResponse,
} from "./types.ts";
import { RoutesPath } from "../router/routes.ts";
import { api } from "./index.ts";

export const authService = {
  async login(credentials: AuthCredentials): Promise<AuthResponse> {
    const { data } = await api.post<AuthResponse>(
      "/identity/login",
      credentials,
    );

    localStorage.setItem("token", data.token);
    localStorage.setItem("user", JSON.stringify(data.user));

    return data;
  },

  async logout(): Promise<void> {
    try {
      // Опционально: уведомить сервер о выходе
      await api.post("/identity/logout");
    } catch (error) {
      console.error("Logout error:", error);
    } finally {
      localStorage.removeItem("token");
      localStorage.removeItem("user");
      window.location.href = `/${RoutesPath.Auth}`;
    }
  },

  async refreshToken(): Promise<RefreshTokenResponse> {
    const jwtToken = localStorage.getItem("token");

    if (!jwtToken) {
      throw new Error("No refresh token available");
    }

    const response = await api.post<RefreshTokenResponse>("/identity/refresh", {
      jwtToken,
    });
    return response.data;
  },

  getToken(): string | null {
    return localStorage.getItem("token");
  },

  isAuthenticated(): boolean {
    return !!this.getToken();
  },
};
