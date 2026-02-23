import UserProfile from "../UserProfile";
import Autocomplete from "../Autocomplete";
import { useState } from "react";
import type {
  AutocompleteFetchFn,
  AutocompleteFetchFnResponse,
} from "../Autocomplete/Autocomplete.types.ts";
import styles from "./Header.module.scss";
import Navigation from "../Navigation";

const Header = () => {
  const [playerQuery, setPlayerQuery] = useState<string>("");

  const fetchData: AutocompleteFetchFn = (_value: string) => {
    return new Promise<AutocompleteFetchFnResponse>((resolve) => {
      setTimeout(() => {
        resolve({
          data: [
            { value: "1", label: String(Math.random() * 1000) },
            { value: "2", label: String(Math.random() * 1000) },
            { value: "3", label: String(Math.random() * 1000) },
            { value: "4", label: String(Math.random() * 1000) },
            { value: "5", label: String(Math.random() * 1000) },
            { value: "6", label: String(Math.random() * 1000) },
            { value: "7", label: String(Math.random() * 1000) },
            { value: "8", label: String(Math.random() * 1000) },
            { value: "9", label: String(Math.random() * 1000) },
            { value: "10", label: String(Math.random() * 1000) },
            { value: "11", label: String(Math.random() * 1000) },
            { value: "12", label: String(Math.random() * 1000) },
            { value: "13", label: String(Math.random() * 1000) },
            { value: "14", label: String(Math.random() * 1000) },
          ],
          page: 1,
          pageSize: 10,
          total: 3,
        });
      }, 1000);
    });
  };

  const onPlayerQueryChange = (query: string) => {
    setPlayerQuery(query);
  };

  return (
    <div className={styles.headerContainer}>
      <h2>Greatest leaderboard</h2>
      <Navigation />
      <Autocomplete
        value={playerQuery}
        onChange={onPlayerQueryChange}
        fetchData={fetchData}
        placeholder="Find player"
      />
      <UserProfile />
    </div>
  );
};

export default Header;
