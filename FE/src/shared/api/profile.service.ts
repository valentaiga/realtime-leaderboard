import type {
  DateRange,
  GetProfileResponse,
  GetRelativeTableDataResponse,
  GetProfileHistoryResponse,
} from "./types.ts";
import { api } from "./index.ts";

export const profileService = {
  async getProfileInfo(username: string): Promise<GetProfileResponse> {
    const { data } = await api.get<GetProfileResponse>("/profile/username", {
      params: {
        username,
      },
    });

    return data;
  },
  async getRelativeTableData(
    username: string,
  ): Promise<GetRelativeTableDataResponse> {
    const { data } = await api.get<GetRelativeTableDataResponse>(
      "/profile/table",
      {
        params: {
          username,
        },
      },
    );

    return data;
  },
  async getProfileHistory(
    username: string,
    dateRange: DateRange,
  ): Promise<GetProfileHistoryResponse> {
    const { data } = await api.get<GetProfileHistoryResponse>(
      "/profile/history",
      {
        params: {
          username,
          dateRange,
        },
      },
    );

    return data;
  },
};
