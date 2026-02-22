import React from "react";
import { Navigate, Outlet } from "react-router-dom";
import { authService } from "../../shared/api/auth.service.ts";
import { RoutesPath } from "../../shared/router/routes.ts";
import UserProfile from "../UserProfile";

const ProtectedRoute: React.FC = () => {
  return authService.isAuthenticated() ? (
    <>
      <UserProfile />
      <Outlet />
    </>
  ) : (
    <Navigate to={`/${RoutesPath.Auth}`} replace />
  );
};

export default ProtectedRoute;
