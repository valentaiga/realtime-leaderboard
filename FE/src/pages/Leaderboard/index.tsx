import React, { useCallback, useEffect, useRef, useState } from "react";
import { Flex, message, Space, Table, Tag } from "antd";
import { useNavigate } from "react-router-dom";
import { RoutesPath } from "../../shared/router/routes.ts";
import { useLeaderboardTable } from "../../shared/hooks/useLeaderboardTable.ts";

const Leaderboard: React.FC = () => {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);
  const { data, setData, columns } = useLeaderboardTable();
  const [isConnected, setIsConnected] = useState(false);
  const ws = useRef<WebSocket | null>(null);
  const reconnectTimeout = useRef<ReturnType<typeof setTimeout> | null>(null);

  // Функция для обработки входящих сообщений
  const handleWebSocketMessage = useCallback((event: MessageEvent) => {
    try {
      const { type, data } = JSON.parse(event.data);

      switch (type) {
        case "initial_data":
        case "update":
          setData(data);
          break;

        default:
          console.log("Unknown message type:", type);
      }
    } catch (error) {
      console.error("Error parsing WebSocket message:", error);
    }
  }, []);

  // Функция подключения к WebSocket
  const connectWebSocket = useCallback(() => {
    try {
      // Закрываем предыдущее соединение если есть
      if (ws.current) {
        ws.current.close();
      }

      // Определяем URL WebSocket (можно вынести в .env)
      const wsUrl = import.meta.env.VITE_WS_BASE_URL;

      ws.current = new WebSocket(wsUrl + "/leaderboard");

      ws.current.onopen = () => {
        console.log("WebSocket Connected to leaderboard");
        setIsConnected(true);
        message.success("Подключено к серверу лидерборда");

        // Очищаем таймаут переподключения если был
        if (reconnectTimeout.current) {
          clearTimeout(reconnectTimeout.current);
        }
      };

      ws.current.onclose = (event) => {
        console.log("WebSocket Disconnected from leaderboard", event);
        setIsConnected(false);

        // Пытаемся переподключиться через 3 секунды
        reconnectTimeout.current = setTimeout(() => {
          console.log("Attempting to reconnect...");
          connectWebSocket();
        }, 3000);
      };

      ws.current.onerror = (error) => {
        console.error("WebSocket Error:", error);
        message.error("Ошибка подключения к серверу");
      };

      ws.current.onmessage = handleWebSocketMessage;
    } catch (error) {
      console.error("Error creating WebSocket connection:", error);
    }
  }, [handleWebSocketMessage]);

  // Инициализация WebSocket соединения
  useEffect(() => {
    // connectWebSocket();

    // Очистка при размонтировании компонента
    return () => {
      if (ws.current) {
        ws.current.close();
      }
      if (reconnectTimeout.current) {
        clearTimeout(reconnectTimeout.current);
      }
    };
  }, [connectWebSocket]);

  // Загрузка начальных данных через WebSocket
  useEffect(() => {
    if (isConnected && ws.current) {
      setLoading(true);
      // Запрашиваем начальные данные
      ws.current.send(
        JSON.stringify({
          type: "get_initial_data",
        }),
      );
    }
  }, [isConnected]);
  const onRowClick = (record: (typeof data)[number]) => {
    navigate(`/${RoutesPath.Profile}/${record.username}`);
  };

  return (
    <>
      <Flex justify="space-between" align="center" style={{ marginBottom: 16 }}>
        <Space>
          <span>Статус: {isConnected ? "🟢 Онлайн" : "🔴 Офлайн"}</span>
          {!isConnected && (
            <Tag color="warning">Пытаемся переподключиться...</Tag>
          )}
        </Space>
        <Space>
          <Tag color="blue">Обновлено: {new Date().toLocaleTimeString()}</Tag>
        </Space>
      </Flex>

      <Table
        columns={columns}
        loading={loading}
        rowKey="id"
        dataSource={data}
        pagination={false}
        onRow={(row) => {
          return {
            onClick: () => onRowClick(row),
            style: { cursor: "pointer" },
          };
        }}
      />
    </>
  );
};

export default Leaderboard;
