import React, { useState, useEffect } from "react";
import api from "../api";

const Cars = () => {
    const [cars, setCars] = useState([]);
    const [categories, setCategories] = useState([]);
    const [selectedCar, setSelectedCar] = useState(null); // Car currently being updated
    const [updateType, setUpdateType] = useState(""); // Field to update (category, odometer, etc.)
    const [updateValue, setUpdateValue] = useState(""); // New value for the field
    const [newCar, setNewCar] = useState({ plateNumber: "", categoryId: "", odometer: 0, description: "", available: true, type: "" });
    const [showAddForm, setShowAddForm] = useState(false);

    useEffect(() => {
        fetchCars();
        fetchCategories();
    }, []);

    const fetchCars = async () => {
        try {
            const response = await api.get("/api/Car");
            setCars(response.data.data);
        } catch (error) {
            console.error("Failed to fetch cars:", error);
        }
    };

    const fetchCategories = async () => {
        try {
            const response = await api.get("/api/Category");
            setCategories(response.data.data);
        } catch (error) {
            console.error("Failed to fetch categories:", error);
        }
    };

    const handleAddCar = async () => {
        try {
            await api.post("/api/Car", newCar);
            setShowAddForm(false);
            setNewCar({ plateNumber: "", categoryId: "", odometer: 0, description: "", available: true, type: "" });
            fetchCars(); // Refresh car list
        } catch (error) {
            console.error("Failed to add car:", error);
        }
    };

    const handleDeleteCar = async (id) => {
        try {
            await api.delete(`/api/Car/${id}`);
            fetchCars(); // Refresh car list
        } catch (error) {
            console.error("Failed to delete car:", error);
        }
    };

    const handleUpdate = async () => {
        if (!selectedCar || !updateType) return;

        // Build the request body
        const requestBody = { id: selectedCar.id };
        if (updateType === "category") {
            requestBody.categoryId = updateValue;
        } else if (updateType === "odometer") {
            requestBody.odometer = parseInt(updateValue, 10);
        } else if (updateType === "type") {
            requestBody.type = updateValue;
        } else if (updateType === "description") {
            requestBody.description = updateValue;
        } else if (updateType === "available") {
            requestBody.available = updateValue === "true"; // Convert string to boolean
        }

        try {
            // Dynamically set the endpoint based on updateType
            await api.put(`/api/Car/${updateType}`, requestBody);
            fetchCars(); // Refresh car list
            setSelectedCar(null); // Close modal
            setUpdateType(""); // Reset form
            setUpdateValue(""); // Reset form
        } catch (error) {
            console.error(`Failed to update car for ${updateType}:`, error);
        }
    };

    return (
        <div className="container mx-auto p-6">
            <div className="flex justify-between items-center mb-8">
                <h1 className="text-3xl font-bold text-gray-800">Cars Management</h1>
                <button 
                    onClick={() => setShowAddForm(true)}
                    className="bg-green-500 text-white px-4 py-2 rounded-lg hover:bg-green-600"
                >
                    Add New Car
                </button>
            </div>

            {/* Car List */}
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                {cars.map((car) => (
                    <div key={car.id} className="bg-white shadow-md rounded-lg p-6">
                        <h3 className="text-xl font-semibold text-gray-800">{car.plateNumber}</h3>
                        <p className="text-gray-600 mt-1">
                            <span className="font-medium">Odometer:</span> {car.odometer} km
                        </p>
                        <p className="text-gray-600 mt-1">
                            <span className="font-medium">Category:</span>{" "}
                            {categories.find((c) => String(c.id) === String(car.categoryId))?.name || car.categoryName}
                        </p>
                        <p className="text-gray-600 mt-1">
                            <span className="font-medium">Make & Model:</span> {car.type}
                        </p>
                        <p className="text-gray-600 mt-1">
                            <span className="font-medium">Available:</span>{" "}
                            {car.available ? "Yes" : "No"}
                        </p>
                        <div className="flex gap-2 mt-4">
                            <button
                                onClick={() => setSelectedCar(car)}
                                className="bg-blue-500 text-white px-4 py-2 rounded-lg hover:bg-blue-600"
                            >
                                Update
                            </button>
                            <button
                                onClick={() => handleDeleteCar(car.id)}
                                className="bg-red-500 text-white px-4 py-2 rounded-lg hover:bg-red-600"
                            >
                                Delete
                            </button>
                        </div>
                    </div>
                ))}
            </div>

            {/* Add Car Modal */}
            {showAddForm && (
                <div className="fixed inset-0 bg-black bg-opacity-50 flex justify-center items-center z-50">
                    <div className="bg-white rounded-lg shadow-lg p-6 w-full max-w-lg">
                        <h2 className="text-xl font-semibold mb-4">Add New Car</h2>
                        
                        <div className="mb-4">
                            <label className="block text-sm font-medium text-gray-700 mb-1">
                                Plate Number
                            </label>
                            <input
                                type="text"
                                value={newCar.plateNumber}
                                onChange={(e) => setNewCar({ ...newCar, plateNumber: e.target.value })}
                                className="block w-full border border-gray-300 rounded-lg p-3"
                                placeholder="ABC123"
                            />
                        </div>
                        
                        <div className="mb-4">
                            <label className="block text-sm font-medium text-gray-700 mb-1">
                                Category
                            </label>
                            <select
                                value={newCar.categoryId}
                                onChange={(e) => setNewCar({ ...newCar, categoryId: e.target.value })}
                                className="block w-full border border-gray-300 rounded-lg p-3"
                            >
                                <option value="">Select a Category</option>
                                {categories.map((category) => (
                                    <option key={category.id} value={category.id}>
                                        {category.name}
                                    </option>
                                ))}
                            </select>
                        </div>
                        
                        <div className="mb-4">
                            <label className="block text-sm font-medium text-gray-700 mb-1">
                                Odometer
                            </label>
                            <input
                                type="number"
                                value={newCar.odometer}
                                onChange={(e) => setNewCar({ ...newCar, odometer: parseInt(e.target.value, 10) })}
                                className="block w-full border border-gray-300 rounded-lg p-3"
                                placeholder="0"
                            />
                        </div>
                        
                        <div className="mb-4">
                            <label className="block text-sm font-medium text-gray-700 mb-1">
                                Description
                            </label>
                            <textarea
                                value={newCar.description}
                                onChange={(e) => setNewCar({ ...newCar, description: e.target.value })}
                                className="block w-full border border-gray-300 rounded-lg p-3"
                                placeholder="Car description..."
                            ></textarea>
                        </div>
                        
                        <div className="mb-4">
                            <label className="block text-sm font-medium text-gray-700 mb-1">
                                Make & Model (Type)
                            </label>
                            <input
                                type="text"
                                value={newCar.type}
                                onChange={(e) => setNewCar({ ...newCar, type: e.target.value })}
                                className="block w-full border border-gray-300 rounded-lg p-3"
                                placeholder="e.g., Volkswagen Golf"
                            />
                            <p className="text-xs text-gray-500 mt-1">Enter the manufacturer and model name</p>
                        </div>
                        
                        <div className="mb-4">
                            <label className="block text-sm font-medium text-gray-700 mb-1">
                                Availability
                            </label>
                            <select
                                value={newCar.available}
                                onChange={(e) => setNewCar({ ...newCar, available: e.target.value === "true" })}
                                className="block w-full border border-gray-300 rounded-lg p-3"
                            >
                                <option value="true">Available</option>
                                <option value="false">Not Available</option>
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
                                onClick={handleAddCar}
                                className="bg-green-500 text-white px-6 py-2 rounded-lg hover:bg-green-600"
                            >
                                Add Car
                            </button>
                        </div>
                    </div>
                </div>
            )}

            {/* Update Modal */}
            {selectedCar && (
                <div className="fixed inset-0 bg-black bg-opacity-50 flex justify-center items-center z-50">
                    <div className="bg-white rounded-lg shadow-lg p-6 w-full max-w-lg">
                        <h2 className="text-xl font-semibold mb-4">Update Car</h2>

                        <div className="mb-4">
                            <label htmlFor="updateType" className="block text-sm font-medium text-gray-700">
                                Choose what to update:
                            </label>
                            <select
                                id="updateType"
                                value={updateType}
                                onChange={(e) => setUpdateType(e.target.value)}
                                className="mt-1 block w-full border border-gray-300 rounded-lg p-3"
                            >
                                <option value="">Select an option</option>
                                <option value="categoryId">Category</option>
                                <option value="odometer">Odometer</option>
                                <option value="type">Make & Model</option>
                                <option value="description">Description</option>
                                <option value="available">Availability</option>
                            </select>
                        </div>

                        {/* Dynamic Input Field Based on Update Type */}
                        {updateType && (
                            <div className="mb-4">
                                {updateType === "categoryId" && (
                                    <>
                                        <label className="block text-sm font-medium text-gray-700">Category</label>
                                        <select
                                            value={updateValue}
                                            onChange={(e) => setUpdateValue(e.target.value)}
                                            className="mt-1 block w-full border border-gray-300 rounded-lg p-3"
                                        >
                                            <option value="">Select a Category</option>
                                            {categories.map((category) => (
                                                <option key={category.id} value={category.id}>
                                                    {category.name}
                                                </option>
                                            ))}
                                        </select>
                                    </>
                                )}
                                {updateType === "odometer" && (
                                    <>
                                        <label className="block text-sm font-medium text-gray-700">Odometer</label>
                                        <input
                                            type="number"
                                            value={updateValue}
                                            onChange={(e) => setUpdateValue(e.target.value)}
                                            className="mt-1 block w-full border border-gray-300 rounded-lg p-3"
                                        />
                                    </>
                                )}
                                {updateType === "type" && (
                                    <>
                                        <label className="block text-sm font-medium text-gray-700">Make & Model</label>
                                        <input
                                            type="text"
                                            value={updateValue}
                                            onChange={(e) => setUpdateValue(e.target.value)}
                                            className="mt-1 block w-full border border-gray-300 rounded-lg p-3"
                                            placeholder="e.g., Volkswagen Golf"
                                        />
                                        <p className="text-xs text-gray-500 mt-1">Enter the manufacturer and model name</p>
                                    </>
                                )}
                                {updateType === "description" && (
                                    <>
                                        <label className="block text-sm font-medium text-gray-700">Description</label>
                                        <textarea
                                            value={updateValue}
                                            onChange={(e) => setUpdateValue(e.target.value)}
                                            className="mt-1 block w-full border border-gray-300 rounded-lg p-3"
                                        ></textarea>
                                    </>
                                )}
                                {updateType === "available" && (
                                    <>
                                        <label className="block text-sm font-medium text-gray-700">Availability</label>
                                        <select
                                            value={updateValue}
                                            onChange={(e) => setUpdateValue(e.target.value)}
                                            className="mt-1 block w-full border border-gray-300 rounded-lg p-3"
                                        >
                                            <option value="">Select Availability</option>
                                            <option value="true">Available</option>
                                            <option value="false">Not Available</option>
                                        </select>
                                    </>
                                )}
                            </div>
                        )}

                        {/* Modal Actions */}
                        <div className="flex justify-end gap-4 mt-4">
                            <button
                                onClick={() => {
                                    setSelectedCar(null);
                                    setUpdateType("");
                                    setUpdateValue("");
                                }}
                                className="text-gray-500 hover:underline"
                            >
                                Cancel
                            </button>
                            <button
                                onClick={handleUpdate}
                                className="bg-blue-500 text-white px-6 py-2 rounded-lg hover:bg-blue-600"
                            >
                                Save
                            </button>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
};

export default Cars;