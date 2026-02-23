import { useState } from "react";
import { Select } from "antd";
import { LoadingOutlined } from "@ant-design/icons";
import { debounce } from "../../shared/utils";
import type {
  AutocompleteFetchFn,
  AutocompleteOption,
} from "./Autocomplete.types.ts";

interface AutocompleteProps {
  fetchData: AutocompleteFetchFn;
  placeholder: string;
  value: AutocompleteOption["value"];
  onChange: (value: AutocompleteOption["value"]) => void;
  className?: string;
}

const Autocomplete = ({
  fetchData,
  placeholder,
  value,
  onChange,
  className,
}: AutocompleteProps) => {
  const [data, setData] = useState<AutocompleteOption[]>([]);
  const [loading, setLoading] = useState<boolean>(false);

  const handleSearch = debounce(async (newValue: string) => {
    try {
      setLoading(true);
      const {
        data, //, page, pageSize, total
      } = await fetchData(newValue);
      setData(data);
    } catch (error) {
      console.error("Error fetching autocomplete options:", error);
    } finally {
      setLoading(false);
    }
  }, 1500);

  return (
    <Select
      showSearch={{ filterOption: false, onSearch: handleSearch }}
      value={value || null}
      placeholder={placeholder}
      defaultActiveFirstOption={false}
      className={className}
      suffixIcon={loading ? <LoadingOutlined spin /> : null}
      onChange={onChange}
      onPopupScroll={console.log}
      loading={loading}
      notFoundContent={null}
      options={data}
    />
  );
};

export default Autocomplete;
