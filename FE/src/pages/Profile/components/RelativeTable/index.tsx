import { useEffect } from "react";
import { useLeaderboardTable } from "../../../../shared/hooks/useLeaderboardTable.ts";
import { Table } from "antd";
import { RoutesPath } from "../../../../shared/router/routes.ts";
import { useNavigate } from "react-router-dom";

interface RelativeTableProps {
  username: string;
}

const RelativeTable = ({ username }: RelativeTableProps) => {
  const {
    data, //setData,
    columns,
  } = useLeaderboardTable();
  const navigate = useNavigate();
  useEffect(() => {
    // profileService.getRelativeTableData(username);
    // .then((res) => {
    // setData([]);
    // });
  }, []);

  const onRowClick = (record: (typeof data)[number]) => {
    navigate(`/${RoutesPath.Profile}/${record.username}`);
  };

  return (
    <Table
      columns={columns}
      rowKey="id"
      dataSource={data}
      pagination={false}
      onRow={(row) => {
        return {
          onClick: () => row.username !== username && onRowClick(row),
          style:
            row.username === username
              ? { backgroundColor: "#f5f5f5" }
              : { cursor: "pointer" },
        };
      }}
    />
  );
};

export default RelativeTable;
