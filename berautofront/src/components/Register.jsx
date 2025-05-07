import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import api from "../api";
import './Register.css'; // Import the stylesheet

export default function Register() {
    const [form, setForm] = useState({ name: "", email: "", password: "", address: "", phoneNumber: "" });
    const [error, setError] = useState(null);
    const navigate = useNavigate();

    const handleChange = (e) => {
        const { name, value } = e.target;
        setForm((prev) => ({ ...prev, [name]: value }));
    };

    const handleRegister = async (e) => {
        e.preventDefault();
        try {
            await api.post("/api/Auth/register", form);
            navigate("/login"); // Redirect to login on success
        } catch (err) {
            setError("Failed to register");
        }
    };

    return (
        <div className="register-container">
            <div className="register-box">
                <h1 className="register-title">Create Account</h1>
                <form className="register-form" onSubmit={handleRegister}>
                    <div className="register-input-group">
                        <label htmlFor="name">Name</label>
                        <input
                            type="text"
                            id="name"
                            name="name"
                            value={form.name}
                            onChange={handleChange}
                            required
                            placeholder="Enter your name"
                        />
                    </div>
                    <div className="register-input-group">
                        <label htmlFor="email">Email</label>
                        <input
                            type="email"
                            id="email"
                            name="email"
                            value={form.email}
                            onChange={handleChange}
                            required
                            placeholder="Enter your email"
                        />
                    </div>
                    <div className="register-input-group">
                        <label htmlFor="password">Password</label>
                        <input
                            type="password"
                            id="password"
                            name="password"
                            value={form.password}
                            onChange={handleChange}
                            required
                            placeholder="Enter a secure password"
                        />
                    </div>
                    <div className="register-input-group">
                        <label htmlFor="address">Address</label>
                        <input
                            type="text"
                            id="address"
                            name="address"
                            value={form.address}
                            onChange={handleChange}
                            placeholder="Enter your address"
                        />
                    </div>
                    <div className="register-input-group">
                        <label htmlFor="phoneNumber">Phone Number</label>
                        <input
                            type="text"
                            id="phoneNumber"
                            name="phoneNumber"
                            value={form.phoneNumber}
                            onChange={handleChange}
                            placeholder="Enter your phone number"
                        />
                    </div>
                    {error && <p className="register-error">{error}</p>}
                    <button className="register-button" type="submit">
                        Register
                    </button>
                </form>
                <p className="register-login-link">
                    Already have an account? <a href="/login">Login here</a>.
                </p>
            </div>
        </div>
    );
}