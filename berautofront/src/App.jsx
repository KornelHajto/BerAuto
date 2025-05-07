import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import Navbar from "./components/Navbar";
import Cars from "./components/Cars";
import Categories from "./components/Categories";
import Users from "./components/Users";
import Rentals from "./components/Rentals";
import Login from "./components/Login";
import Register from "./components/Register";
import ProtectedRoute from "./components/ProtectedRoute";

function App() {
    return (
        <Router>
            <div className="min-h-screen flex flex-col bg-gradient-to-b from-secondary-50 to-secondary-100">
                {/* Navbar sits above all pages */}
                <Navbar />
                <div className="flex-grow px-4 py-6 md:px-8 lg:px-12 max-w-screen-2xl mx-auto w-full">
                    <Routes>
                        <Route path="/login" element={<Login />} />
                        <Route path="/register" element={<Register />} />
                        <Route path="/" element={<ProtectedRoute element={Cars} />} />
                        <Route path="/categories" element={<ProtectedRoute element={Categories} />} />
                        <Route path="/users" element={<ProtectedRoute element={Users} />} />
                        <Route path="/rentals" element={<ProtectedRoute element={Rentals} />} />
                    </Routes>
                </div>
                <footer className="py-4 bg-white border-t border-secondary-200">
                    <div className="max-w-screen-2xl mx-auto px-4 md:px-8 text-center text-secondary-500 text-sm">
                        © {new Date().getFullYear()} Car Rental System. All rights reserved.
                    </div>
                </footer>
            </div>
        </Router>
    );
}

export default App;