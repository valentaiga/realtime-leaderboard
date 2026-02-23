import type { Pagination } from "../../shared/api/types.ts";

export interface AutocompleteOption {
  value: string;
  label: string;
}

export interface AutocompleteFetchFnResponse extends Pagination {
  data: AutocompleteOption[];
}

export type AutocompleteFetchFn = (
  query: string,
) => Promise<AutocompleteFetchFnResponse>;
