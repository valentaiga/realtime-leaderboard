import type { TableProps } from "antd";
import { useState } from "react";

interface DataType {
  id: string;
  position: number;
  username: string;
  score: number;
}

const renderPosition = (position: number) => {
  switch (position) {
    case 1:
      return "🥇";
    case 2:
      return "🥈";
    case 3:
      return "🥉";
    default:
      return position;
  }
};

const columns: TableProps<DataType>["columns"] = [
  {
    title: "Position",
    dataIndex: "position",
    key: "position",
    render: renderPosition,
  },
  {
    title: "Username",
    dataIndex: "username",
    key: "username",
  },
  {
    title: "Score",
    dataIndex: "score",
    key: "score",
  },
];

const mockData: DataType[] = [
  {
    id: "test-1",
    username: "John Brown",
    score: 32,
    position: 1,
  },
  {
    id: "test-2",
    username: "Jim Green",
    score: 42,
    position: 2,
  },
  {
    id: "test-3",
    username: "Joe Black",
    score: 32,
    position: 3,
  },
  {
    id: "test-4",
    username: "Joe Black",
    score: 32,
    position: 4,
  },
];

export const useLeaderboardTable = () => {
  const [data, setData] = useState<DataType[]>(mockData);

  return { data, setData, columns };
};
