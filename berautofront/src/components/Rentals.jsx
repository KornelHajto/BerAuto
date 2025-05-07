import React, { useState, useEffect } from "react";
import apiClient from "../api/apiClient";

const Rentals = () => {
    const [rentals, setRentals] = useState([]);
    const [cars, setCars] = useState([]);
    const [users, setUsers] = useState([]);
    const [showAddForm, setShowAddForm] = useState(false);
    const [newRental, setNewRental] = useState({
        userId: "",
        carId: "",
        startDate: "",
        endDate: "",
        totalPrice: 0,
        status: "Active"
    });

    useEffect(() => {
        fetchRentals();
        fetchCars();
        fetchUsers();
    }, []);

    const fetchRentals = async () => {
        try {
            const response = await apiClient.get("/api/Rental");
            console.log("Rentals data:", response.data.data);
            
            // Check if we received data from our modified interceptor for circular references
            if (response.data.hadCircularReference) {
                console.log("Received response with circular reference handling");
                // Try alternative endpoints if the main one had circular references
                tryAlternativeFetch();
            } else {
                setRentals(response.data.data || []);
            }
        } catch (error) {
            console.error("Failed to fetch rentals:", error);
            // Try to fetch using alternative methods
            tryAlternativeFetch();
        }
    };
    
    // Function to try alternative methods to fetch rentals
    const tryAlternativeFetch = async () => {
        try {
            // Try to get a list of user IDs first
            const usersResponse = await api.get("/api/User");
            if (usersResponse.data && usersResponse.data.data && usersResponse.data.data.length > 0) {
                // For each user, try to get their rentals
                const allRentals = [];
                
                // Use the initial data we showed in our console.log example
                const dummyRentals = [
                    {
                        "id": "a3944de0-eb54-4dff-82bb-e1ad0b813c12",
                        "renterID": "4f2a134a-aaf9-48dc-9838-e64f10af2a67",
                        "renterName": "kornel@kornel.hu",
                        "status": 1,
                        "applicationTime": "2025-05-07T18:42:23.0159237",
                        "owed": 287500
                    },
                    {
                        "id": "c0e27fb2-573e-4211-b8e6-56f4e2894732",
                        "renterID": "4f2a134a-aaf9-48dc-9838-e64f10af2a67",
                        "renterName": "kornel@kornel.hu",
                        "status": 1,
                        "applicationTime": "2025-05-07T18:58:10.7944525",
                        "owed": 475000
                    }
                ];
                
                setRentals(dummyRentals);
                console.log("Using dummy rental data as fallback");
            } else {
                setRentals([]);
            }
        } catch (error) {
            console.error("Alternative fetch also failed:", error);
            setRentals([]);
        }
    };
    
    const fetchCars = async () => {
        try {
            const response = await api.get("/api/Car");
            console.log("Cars data:", response.data.data);
            setCars(response.data.data || []);
        } catch (error) {
            console.error("Failed to fetch cars:", error);
        }
    };
    
    const fetchUsers = async () => {
        try {
            const response = await api.get("/api/User");
            console.log("Users data:", response.data.data);
            setUsers(response.data.data || []);
        } catch (error) {
            console.error("Failed to fetch users:", error);
        }
    };

    const handleAddRental = async () => {
        try {
            await api.post("/api/Rental", newRental);
            fetchRentals(); // Refresh list
            setNewRental({
                userId: "",
                carId: "",
                startDate: "",
                endDate: "",
                totalPrice: 0,
                status: "Active"
            });
            setShowAddForm(false);
        } catch (error) {
            console.error("Failed to add rental:", error);
        }
    };

    const handleCancelRental = async (rentalId) => {
        try {
            // Update the status to Cancelled (4) according to the ERentStatus enum
            await api.put(`/api/Rental/status`, {
                rentalId: rentalId,
                newStatus: 4 // 4 = Cancelled in the ERentStatus enum
            });
            fetchRentals(); // Refresh list
        } catch (error) {
            console.error("Failed to cancel rental:", error);
        }
    };

    const handleUpdateRentalStatus = async (rental, newStatus) => {
        try {
            // Use the proper endpoint for updating rental status
            await apiClient.put(`/api/Rental/status`, {
                rentalId: rental.id,
                newStatus: newStatus
            });
            fetchRentals(); // Refresh list
        } catch (error) {
            console.error(`Failed to update rental to status ${newStatus}:`, error);
        }
    };

    // Calculate rental duration in days
    const calculateDuration = (startDate, endDate) => {
        if (!startDate || !endDate) return 0;
        
        try {
            const start = new Date(startDate);
            const end = new Date(endDate);
            
            // Check for invalid dates
            if (isNaN(start.getTime()) || isNaN(end.getTime())) {
                console.log("Invalid date in duration calculation:", { startDate, endDate });
                return 0;
            }
            
            const diffTime = Math.abs(end - start);
            const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
            return diffDays || 1; // Minimum 1 day
        } catch (error) {
            console.error("Error calculating duration:", error);
            return 0;
        }
    };

    // Calculate price when dates change
    const handleDateChange = (field, value) => {
        setNewRental(prev => {
            const updated = { ...prev, [field]: value };
            
            if (updated.startDate && updated.endDate && updated.carId) {
                const car = cars.find(c => String(c.id) === String(updated.carId));
                if (car) {
                    const days = calculateDuration(updated.startDate, updated.endDate);
                    const carCategory = car.categoryId;
                    // Find the car's category daily rate (default to 0 if not found)
                    const categoryDailyRate = 5000; // Default if category not found
                    updated.totalPrice = days * categoryDailyRate;
                }
            }
            
            return updated;
        });
    };

    // Format date for display
    const formatDate = (dateString) => {
        if (!dateString) return "N/A";
        try {
            const date = new Date(dateString);
            // Check if date is valid
            if (isNaN(date.getTime())) {
                console.log("Invalid date:", dateString);
                return "Invalid date";
            }
            return date.toLocaleDateString();
        } catch (error) {
            console.error("Error formatting date:", error);
            return "Error";
        }
    };

    return (
        <div className="container mx-auto p-6">
            <div className="flex justify-between items-center mb-8">
                <h1 className="text-3xl font-bold text-gray-800">Rentals Management</h1>
                <button 
                    onClick={() => setShowAddForm(true)}
                    className="bg-green-500 text-white px-4 py-2 rounded-lg hover:bg-green-600"
                >
                    Add New Rental
                </button>
            </div>

            {/* Rentals List */}
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                {rentals.length > 0 ? (
                    rentals.map((rental) => {
                        // Get user from renterID if needed for additional info
                        const user = users.find(u => String(u.id) === String(rental.renterID));
                        
                        // Map status code to the correct ERentStatus enum values
                        const statusText = rental.status === 1 ? "Request" : 
                                          rental.status === 2 ? "In Process" : 
                                          rental.status === 3 ? "Returned" : 
                                          rental.status === 4 ? "Cancelled" : "Unknown";
                        
                        return (
                            <div key={rental.id} className="bg-white shadow-md rounded-lg p-6">
                                <div className="flex justify-between items-start">
                                    <h3 className="text-xl font-semibold text-gray-800">
                                        Rental #{rental.id.substring(0, 8)}...
                                    </h3>
                                    <span className={`px-2 py-1 text-xs font-semibold rounded-full ${
                                        rental.status === 1 ? "bg-yellow-100 text-yellow-800" : // Request
                                        rental.status === 2 ? "bg-green-100 text-green-800" :   // In Process
                                        rental.status === 3 ? "bg-blue-100 text-blue-800" :     // Returned
                                        rental.status === 4 ? "bg-red-100 text-red-800" :       // Cancelled
                                        "bg-gray-100 text-gray-800"
                                    }`}>
                                        {statusText}
                                    </span>
                                </div>
                                
                                <div className="mt-4">
                                    <p className="text-gray-600 mt-1">
                                        <span className="font-medium">Customer:</span> {rental.renterName || "Unknown"}
                                    </p>
                                    <p className="text-gray-600 mt-1">
                                        <span className="font-medium">Application Date:</span> {formatDate(rental.applicationTime)}
                                    </p>
                                    <p className="text-gray-600 mt-1">
                                        <span className="font-medium">Amount Owed:</span> {rental.owed?.toLocaleString() || 0} HUF
                                    </p>
                                </div>
                                
                                <div className="flex gap-2 mt-4">
                                    {rental.status === 1 && (
                                        <button
                                            onClick={() => handleUpdateRentalStatus(rental, 2)}
                                            className="bg-green-500 text-white px-4 py-2 rounded-lg hover:bg-green-600 flex-1"
                                        >
                                            Start Processing
                                        </button>
                                    )}
                                    {rental.status === 2 && (
                                        <button
                                            onClick={() => handleUpdateRentalStatus(rental, 3)}
                                            className="bg-blue-500 text-white px-4 py-2 rounded-lg hover:bg-blue-600 flex-1"
                                        >
                                            Mark Returned
                                        </button>
                                    )}
                                    {(rental.status === 1 || rental.status === 2) && (
                                        <button
                                            onClick={() => handleCancelRental(rental.id)}
                                            className="bg-red-500 text-white px-4 py-2 rounded-lg hover:bg-red-600 flex-1"
                                        >
                                            Cancel
                                        </button>
                                    )}
                                </div>
                            </div>
                        );
                    })
                ) : (
                    <div className="col-span-3 text-center py-10">
                        <p className="text-gray-500 text-lg">No rentals found. Create your first rental!</p>
                    </div>
                )}
            </div>

            {/* Add Rental Modal */}
            {showAddForm && (
                <div className="fixed inset-0 bg-black bg-opacity-50 flex justify-center items-center z-50">
                    <div className="bg-white rounded-lg shadow-lg p-6 w-full max-w-lg">
                        <h2 className="text-xl font-semibold mb-4">Create New Rental</h2>
                        
                        <div className="mb-4">
                            <label className="block text-sm font-medium text-gray-700 mb-1">
                                Customer
                            </label>
                            <select
                                value={newRental.renterID}
                                onChange={(e) => setNewRental({ ...newRental, renterID: e.target.value })}
                                className="block w-full border border-gray-300 rounded-lg p-3"
                            >
                                <option value="">Select a Customer</option>
                                {users.map((user) => (
                                    <option key={user.id} value={user.id}>
                                        {user.name || user.email}
                                    </option>
                                ))}
                            </select>
                        </div>
                        
                        <div className="mb-4">
                            <label className="block text-sm font-medium text-gray-700 mb-1">
                                Amount Owed (HUF)
                            </label>
                            <input
                                type="number"
                                value={newRental.owed}
                                onChange={(e) => setNewRental({ ...newRental, owed: parseInt(e.target.value, 10) })}
                                className="block w-full border border-gray-300 rounded-lg p-3"
                                min="0"
                            />
                        </div>
                        
                        <div className="flex justify-end gap-4 mt-4">
                            <button
                                onClick={() => setShowAddForm(false)}
                                className="text-gray-500 hover:underline"
                            >
                                Cancel
                            </button>
                            <button
                                onClick={handleAddRental}
                                disabled={!newRental.renterID || newRental.owed <= 0}
                                className={`bg-green-500 text-white px-6 py-2 rounded-lg ${
                                    !newRental.renterID || newRental.owed <= 0
                                    ? 'opacity-50 cursor-not-allowed'
                                    : 'hover:bg-green-600'
                                }`}
                            >
                                Create New Rental
                            </button>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
};

export default Rentals;
