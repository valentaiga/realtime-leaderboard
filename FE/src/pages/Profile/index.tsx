// import { useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { Flex } from "antd";
import PlayerInfo from "./components/PlayerInfo";
import RelativeTable from "./components/RelativeTable";
import { RoutesPath } from "../../shared/router/routes.ts";
import { useNotification } from "../../shared/hooks/useNotification.tsx";
import PlayerHistory from "./components/PlayerHistory";

const Profile = () => {
  const navigate = useNavigate();
  const { username } = useParams<{ username: string }>();
  const { showError } = useNotification();

  if (!username) {
    showError("Something went wrong.");
    navigate(`/${RoutesPath.Leaderboard}`);
    return null;
  }
  // const [loading, setLoading] = useState(false);

  return (
    <Flex vertical>
      <PlayerInfo
        position={1}
        positionDelta={"+3"}
        score={2}
        scoreDelta={"+400"}
        username={username}
      />
      <RelativeTable username={username} />
      <PlayerHistory username={username} />
    </Flex>
  );
};

export default Profile;
