import React, { useLayoutEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { LockOutlined, UserOutlined } from "@ant-design/icons";

import { Button, Input, Form } from "antd";

import styles from "./Auth.module.scss";
import { RoutesPath } from "../../shared/router/routes.ts";
import { authService } from "../../shared/api/auth.service.ts";
import { useNotification } from "../../shared/hooks/useNotification.tsx";

interface LoginForm {
  username: string;
  password: string;
}

const Auth: React.FC = () => {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);
  const { contextHolder, showError } = useNotification();
  const handleLogin = async (values: LoginForm) => {
    setLoading(true);
    //
    try {
      const { user } = await authService.login(values);
      // const user = {
      //   id: "1",
      //   username: "test",
      // };

      window.localStorage.setItem("user", JSON.stringify(user));

      navigate(`/${RoutesPath.Leaderboard}`);
    } catch (error) {
      showError("Something went wrong.");
    } finally {
      setLoading(false);
    }
  };

  useLayoutEffect(() => {
    if (authService.isAuthenticated()) {
      navigate(`/${RoutesPath.Leaderboard}`);
    }
  }, [navigate]);

  return (
    <div className={styles.authContainer}>
      {contextHolder}
      <Form
        name="auth"
        // style={{ maxWidth: 360 }}
        onFinish={handleLogin}
        clearOnDestroy
        className={styles.authForm}
      >
        <Form.Item>
          <h2 className={styles.title}>Welcome to Greatest Leaderboard!</h2>
        </Form.Item>
        <Form.Item
          name="username"
          rules={[{ required: true, message: "Fields is required" }]}
        >
          <Input
            prefix={<UserOutlined />}
            disabled={loading}
            placeholder="Username"
          />
        </Form.Item>
        <Form.Item
          name="password"
          rules={[{ required: true, message: "Fields is required" }]}
        >
          <Input
            prefix={<LockOutlined />}
            type="password"
            placeholder="Password"
            disabled={loading}
          />
        </Form.Item>

        <Form.Item>
          <Button loading={loading} block type="primary" htmlType="submit">
            Log in
          </Button>
        </Form.Item>
      </Form>
    </div>
  );
};

export default Auth;
