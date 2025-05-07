import React from "react";
import { Link, useNavigate } from "react-router-dom";

export default function Navbar() {
    const navigate = useNavigate();
    const token = localStorage.getItem("accessToken");

    const handleSignOut = () => {
        localStorage.clear();
        navigate("/login");
    };

    return (
        <header className="bg-gray-900 text-white shadow-lg">
            <div className="container mx-auto flex justify-between items-center p-4">
                <h1 className="text-2xl font-semibold">
                    <Link to={token ? "/" : "/login"} className="hover:text-gray-300">
                        Car Rental System
                    </Link>
                </h1>
                <nav className="space-x-4">
                    {token && (
                        <>
                            <Link to="/" className="hover:text-gray-300">
                                Cars
                            </Link>
                            <Link to="/categories" className="hover:text-gray-300">
                                Categories
                            </Link>
                            <Link to="/users" className="hover:text-gray-300">
                                Users
                            </Link>
                                                            <Link to="/rentals" className="py-1 px-2 hover:bg-blue-700 rounded">
                                Rentals
                                                            </Link>
                            <button
                                onClick={handleSignOut}
                                className="text-red-500 hover:text-red-400 font-medium"
                            >
                                Sign Out
                            </button>
                        </>
                    )}
                    {!token && (
                        <>
                            <Link to="/login" className="hover:text-gray-300">
                                Login
                            </Link>
                            <Link to="/register" className="hover:text-gray-300">
                                Register
                            </Link>
                        </>
                    )}
                </nav>
            </div>
        </header>
    );
}