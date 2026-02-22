import { useEffect, useState } from "react";
import type { User } from "../../shared/api/types.ts";
import { Avatar, Button } from "antd";
import { LogoutOutlined } from "@ant-design/icons";
import styles from "./UserProfile.module.scss";
import { RoutesPath } from "../../shared/router/routes.ts";
import { useNavigate } from "react-router-dom";
import { authService } from "../../shared/api/auth.service.ts";

const UserProfile = () => {
  const navigate = useNavigate();
  const [user, setUser] = useState<User | null>(null);
  useEffect(() => {
    const userDataString = localStorage.getItem("user");
    if (userDataString) {
      const userData = JSON.parse(userDataString) satisfies User;
      setUser(userData);
    }
  }, []);

  if (!user) {
    return null;
  }

  const onUsernameClick = () => {
    navigate(`/${RoutesPath.Profile}/${user.username}`);
  };

  return (
    <div className={styles.userProfileContainer}>
      <div className={styles.userProfile}>
        <Avatar style={{ backgroundColor: "#6c03ad", color: "#f56a00" }}>
          {user.username[0]}
        </Avatar>
        <span className={styles.username} onClick={onUsernameClick}>
          {user.username}
        </span>
        <Button
          title="Logout"
          type="text"
          icon={<LogoutOutlined />}
          onClick={authService.logout}
        />
      </div>
    </div>
  );
};

export default UserProfile;
