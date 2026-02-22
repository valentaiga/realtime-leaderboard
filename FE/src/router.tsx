import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import Auth from './pages/Auth';
import Leaderboard from './pages/Leaderboard';
import Profile from './pages/Profile';
import ProtectedRoute from './components/ProtectedRoute';
import {RoutesPath} from "./shared/router/routes.ts";

export const AppRouter: React.FC = () => {
    return (
        <Router>
            <Routes>
                <Route path={`/${RoutesPath.Auth}`} element={<Auth />} />
                <Route element={<ProtectedRoute />}>
                    <Route path={`/${RoutesPath.Leaderboard}`} element={<Leaderboard />} />
                    <Route path={`/${RoutesPath.Profile}/:username`} element={<Profile />} />
                </Route>
                <Route path="/*" element={<Navigate to={`/${RoutesPath.Leaderboard}`} replace />} />
            </Routes>
        </Router>
    );
};
