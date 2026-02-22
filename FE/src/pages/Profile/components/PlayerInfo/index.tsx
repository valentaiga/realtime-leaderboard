import { Flex, Space, Typography } from "antd";
import styles from "./PlayerInfo.module.scss";

interface UserInfoProps {
  username: string;
  position: number;
  positionDelta: string;
  score: number;
  scoreDelta: string;
}
const PlayerInfo = ({
  username,
  position,
  positionDelta,
  score,
  scoreDelta,
}: UserInfoProps) => {
  const renderDelta = (value: number, delta?: string) => {
    if (!delta) {
      return <span className={styles.value}>{value}</span>;
    }
    const deltaNumber = Number(delta);
    return (
      <span className={styles.value}>
        {value}
        <Typography.Text type={deltaNumber > 0 ? "success" : "danger"}>
          {delta}
        </Typography.Text>
      </span>
    );
  };
  return (
    <Space
      orientation="vertical"
      size="middle"
      style={{ display: "flex", padding: "30px" }}
    >
      <Flex vertical align="start">
        <span className={styles.username}>{username}</span>
        <span>Position: {renderDelta(position, positionDelta)}</span>
        <span>Score: {renderDelta(score, scoreDelta)}</span>
      </Flex>
    </Space>
  );
};

export default PlayerInfo;
