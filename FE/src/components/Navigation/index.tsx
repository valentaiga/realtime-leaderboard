import { Tabs } from "antd";
import type { TabsProps } from "antd";
import { useEffect, useState } from "react";
import { RoutesPath } from "../../shared/router/routes.ts";
import { useLocation, useNavigate } from "react-router-dom";

type RoutesPathOmitAuth = Omit<typeof RoutesPath, "Auth">;
type RoutePathOmitAuthKey = keyof RoutesPathOmitAuth;
type RoutePathOmitAuthValue = RoutesPathOmitAuth[RoutePathOmitAuthKey];

const routesPathAuthExcluded = Object.entries(RoutesPath).reduce(
  (acc, [key, value]) => {
    if (RoutesPath.Auth !== value) {
      acc[key as RoutePathOmitAuthKey] = value;
    }
    return acc;
  },
  {} as Record<RoutePathOmitAuthKey, RoutePathOmitAuthValue>,
);

const getCurrentPage = (
  pathname: string,
): RoutePathOmitAuthValue | undefined => {
  const currentRouteEntry = Object.entries(routesPathAuthExcluded).find(
    ([_, route]) => pathname.includes(route),
  );
  if (!currentRouteEntry) {
    return undefined;
  }

  return currentRouteEntry[1];
};

const navigationTabs: TabsProps["items"] = Object.entries(
  routesPathAuthExcluded,
).map(([label, key]) => ({
  key,
  label,
}));

const Navigation = () => {
  const navigate = useNavigate();
  let location = useLocation();
  const [currentPage, setCurrentPage] = useState<
    RoutePathOmitAuthValue | undefined
  >();

  useEffect(() => {
    if (!currentPage) {
      setCurrentPage(getCurrentPage(location.pathname));
    } else if (!location.pathname.includes(currentPage)) {
      setCurrentPage(getCurrentPage(location.pathname));
    }
  }, [location.pathname, currentPage]);

  const handleTabChange = (key: string) => {
    setCurrentPage(key as RoutePathOmitAuthValue);

    const isPathRequireUserData = ([RoutesPath.Profile] as string[]).includes(
      key,
    );
    if (isPathRequireUserData) {
      const userData = JSON.parse(localStorage.getItem("user")!);
      navigate(`/${key}/${userData.username}`);

      return;
    }
    navigate(`/${key}`);
  };

  return (
    <Tabs
      activeKey={currentPage}
      onChange={handleTabChange}
      items={navigationTabs}
    />
  );
};

export default Navigation;
