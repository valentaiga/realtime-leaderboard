export type DateRange = "daily" | "weekly" | "monthly";

export interface Pagination {
  page: number;
  pageSize: number;
  total: number;
}

export interface RefreshTokenResponse {
  token: string;
}

export interface QueueItem {
  resolve: (value: unknown) => void;
  reject: (reason?: unknown) => void;
}

export interface ApiError extends Error {
  config?: {
    url?: string;
    _retry?: boolean;
  };
  response?: {
    status: number;
    data: unknown;
  };
}

export interface AuthCredentials {
  username: string;
  password: string;
}

export interface User {
  id: string;
  username: string;
}

export interface AuthResponse {
  token: string;
  user: User;
}

export interface GetProfileResponse {}
export interface GetRelativeTableDataResponse {}
export interface GetProfileHistoryResponse {}
