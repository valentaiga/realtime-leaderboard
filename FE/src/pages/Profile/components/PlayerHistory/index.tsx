import type { TabsProps } from "antd";
import { Tabs } from "antd";
import HistoryChart from "./HistoryChart";
import { useState } from "react";

interface PlayerHistoryProps {
  username: string;
}

const getTabs = (username: string): TabsProps["items"] => [
  {
    key: "1",
    label: "Daily",
    children: <HistoryChart username={username} dateRange="daily" />,
  },
  {
    key: "2",
    label: "Weekly",
    children: <HistoryChart username={username} dateRange="weekly" />,
  },
  {
    key: "3",
    label: "Monthly",
    children: <HistoryChart username={username} dateRange="monthly" />,
  },
];

const PlayerHistory = ({ username }: PlayerHistoryProps) => {
  const [currentTab, setCurrentTab] = useState("1");
  const onTabChange = (key: string) => {
    setCurrentTab(key);
  };

  return (
    <div>
      <Tabs
        activeKey={currentTab}
        items={getTabs(username)}
        onChange={onTabChange}
        size="large"
        destroyOnHidden
      />
    </div>
  );
};

export default PlayerHistory;
