import React from "react";
import { Navigate } from "react-router-dom";

export default function ProtectedRoute({ element: Component }) {
    const token = localStorage.getItem("accessToken");
    return token ? <Component /> : <Navigate to="/login" />;
}