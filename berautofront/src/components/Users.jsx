import React, { useState, useEffect } from "react";
import api from "../api";

export default function Users() {
    const [users, setUsers] = useState([]);
    const [newUser, setNewUser] = useState({ 
        name: "", 
        email: "", 
        password: "", 
        address: "", 
        phoneNumber: "",
        userType: 3 // 3 = User in the enum
    });
    const [showAddForm, setShowAddForm] = useState(false);

    // Fetch all users on component load
    useEffect(() => {
        fetchUsers();
    }, []);

    const fetchUsers = async () => {
        try {
            const response = await api.get("/api/User");
            console.log("Users data from API:", response.data.data);
            setUsers(response.data.data);
        } catch (error) {
            console.error("Failed to fetch users", error);
        }
    };

    // Add a new user
    const handleAddUser = async () => {
        try {
            await api.post("/api/auth/register", newUser);
            fetchUsers();
            setNewUser({ name: "", email: "", password: "", address: "", userType: 3 });
            setShowAddForm(false);
        } catch (error) {
            console.error("Failed to add user", error);
        }
    };

    // Update a user
    const handleUpdateUser = async () => {
        try {
            await api.put(`/api/User/dsad`, editingUser);
            fetchUsers();
            setEditingUser(null); // Exit editing mode
        } catch (error) {
            console.error("Failed to update user", error);
        }
    };

    // Delete a user
    const handleDeleteUser = async (id) => {
        try {
            await api.delete(`/api/User/${id}`);
            fetchUsers();
        } catch (error) {
            console.error("Failed to delete user", error);
        }
    };

    return (
        <div className="container mx-auto p-6">
            <div className="flex justify-between items-center mb-8">
                <h1 className="text-3xl font-bold text-gray-800">Users Management</h1>
                <button 
                    onClick={() => setShowAddForm(true)}
                    className="bg-green-500 text-white px-4 py-2 rounded-lg hover:bg-green-600"
                >
                    Add New User
                </button>
            </div>

            {/* Users List */}
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                {users.map((user) => (
                    <div key={user.id} className="bg-white shadow-md rounded-lg p-6">
                        <h3 className="text-xl font-semibold text-gray-800">{user.name}</h3>
                        <p className="text-gray-600 mt-1">
                            <span className="font-medium">Email:</span> {user.email}
                        </p>
                        <p className="text-gray-600 mt-1">
                            <span className="font-medium">Address:</span> {user.address}
                        </p>
                        <p className="text-gray-600 mt-1">
                            <span className="font-medium">Phone:</span> {user.phoneNumber || "N/A"}
                        </p>
                        <p className="text-gray-600 mt-1">
                            <span className="font-medium">User Type:</span> {
                                String(user.accesLevel) === "1" || user.accesLevel === 1 ? "Administrator" :
                                String(user.accesLevel) === "2" || user.accesLevel === 2 ? "Worker" :
                                String(user.accesLevel) === "3" || user.accesLevel === 3 ? "User" :
                                String(user.accesLevel) === "4" || user.accesLevel === 4 ? "System" :
                                // Log the value to console for debugging
                                ((console.log("Unknown userType value:", user.accesLevel), "Unknown"))
                            }
                        </p>
                        <div className="flex gap-2 mt-4">
                            <button
                                onClick={() => handleDeleteUser(user.id)}
                                className="bg-red-500 text-white px-4 py-2 rounded-lg hover:bg-red-600 w-full"
                            >
                                Delete
                            </button>
                        </div>
                    </div>
                ))}
                
                {users.length === 0 && (
                    <div className="col-span-3 text-center py-8 text-gray-500">
                        No users found. Add a new user to get started.
                    </div>
                )}
            </div>

            {/* Add User Modal */}
            {showAddForm && (
                <div className="fixed inset-0 bg-black bg-opacity-50 flex justify-center items-center z-50">
                    <div className="bg-white rounded-lg shadow-lg p-6 w-full max-w-lg">
                        <h2 className="text-xl font-semibold mb-4">Add New User</h2>
                        
                        <div className="mb-4">
                            <label className="block text-sm font-medium text-gray-700 mb-1">
                                Name
                            </label>
                            <input
                                type="text"
                                value={newUser.name}
                                onChange={(e) => setNewUser({ ...newUser, name: e.target.value })}
                                className="block w-full border border-gray-300 rounded-lg p-3"
                                placeholder="Full Name"
                            />
                        </div>
                        
                        <div className="mb-4">
                            <label className="block text-sm font-medium text-gray-700 mb-1">
                                Email
                            </label>
                            <input
                                type="email"
                                value={newUser.email}
                                onChange={(e) => setNewUser({ ...newUser, email: e.target.value })}
                                className="block w-full border border-gray-300 rounded-lg p-3"
                                placeholder="email@example.com"
                            />
                        </div>
                        
                        <div className="mb-4">
                            <label className="block text-sm font-medium text-gray-700 mb-1">
                                Password
                            </label>
                            <input
                                type="password"
                                value={newUser.password}
                                onChange={(e) => setNewUser({ ...newUser, password: e.target.value })}
                                className="block w-full border border-gray-300 rounded-lg p-3"
                                placeholder="Enter a secure password"
                            />
                        </div>
                        
                        <div className="mb-4">
                            <label className="block text-sm font-medium text-gray-700 mb-1">
                                Address
                            </label>
                            <input
                                type="text"
                                value={newUser.address}
                                onChange={(e) => setNewUser({ ...newUser, address: e.target.value })}
                                className="block w-full border border-gray-300 rounded-lg p-3"
                                placeholder="Address"
                            />
                        </div>
                        
                        <div className="mb-4">
                            <label className="block text-sm font-medium text-gray-700 mb-1">
                                Phone Number
                            </label>
                            <input
                                type="text"
                                value={newUser.phoneNumber}
                                onChange={(e) => setNewUser({ ...newUser, phoneNumber: e.target.value })}
                                className="block w-full border border-gray-300 rounded-lg p-3"
                                placeholder="Enter your phone number"
                            />
                        </div>
                        
                        <div className="mb-4">
                            <label className="block text-sm font-medium text-gray-700 mb-1">
                                User Type
                            </label>
                            <select
                                value={newUser.userType}
                                onChange={(e) => setNewUser({ ...newUser, userType: parseInt(e.target.value) })}
                                className="block w-full border border-gray-300 rounded-lg p-3"
                            >
                                <option value="1">Administrator</option>
                                <option value="2">Worker</option>
                                <option value="3">User</option>
                                <option value="4">System</option>
                            </select>
                        </div>
                        
                        <div className="flex justify-end gap-4 mt-4">
                            <button
                                onClick={() => setShowAddForm(false)}
                                className="text-gray-500 hover:underline"
                            >
                                Cancel
                            </button>
                            <button
                                onClick={handleAddUser}
                                className="bg-green-500 text-white px-6 py-2 rounded-lg hover:bg-green-600"
                            >
                                Add User
                            </button>
                        </div>
                    </div>
                </div>
            )}

            {/* No Edit User Modal */}
        </div>
    );
}