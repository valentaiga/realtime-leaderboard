import { notification } from "antd";

export const useNotification = () => {
  const [api, contextHolder] = notification.useNotification();

  const showSuccess = (description: string, duration: number | false = 3) => {
    console.log(duration);
    api.success({
      title: "Success",
      description,
      duration,
    });
  };
  const showError = (description: string, duration: number | false = 3) => {
    api.error({
      title: "Error",
      description,
      duration,
    });
  };
  return {
    contextHolder,
    showSuccess,
    showError,
  };
};
