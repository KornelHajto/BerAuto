import { useState, useEffect } from "react";
import api from "../api";

function Categories() {
    const [categories, setCategories] = useState([]);
    const [newCategory, setNewCategory] = useState({ name: "", dailyRate: 0 });
    const [editingCategory, setEditingCategory] = useState(null);
    const [showAddForm, setShowAddForm] = useState(false);

    // Fetch categories on component load
    useEffect(() => {
        fetchCategories();
    }, []);

    // Fetch categories from API
    const fetchCategories = async () => {
        try {
            const response = await api.get("/api/Category");
            setCategories(response.data.data);
        } catch (error) {
            console.error("Failed to fetch categories:", error);
        }
    };

    // Add a new category
    const handleAddCategory = async () => {
        try {
            await api.post("/api/Category", newCategory);
            fetchCategories(); // Refresh list
            setNewCategory({ name: "", dailyRate: 0 });
            setShowAddForm(false);
        } catch (error) {
            console.error("Failed to add category:", error);
        }
    };

    // Update a category
    const handleUpdateCategory = async () => {
        try {
            await api.put(`/api/Category/${editingCategory.id}`, editingCategory);
            fetchCategories(); // Refresh list
            setEditingCategory(null);
        } catch (error) {
            console.error("Failed to update category:", error);
        }
    };

    // Delete a category
    const handleDeleteCategory = async (id) => {
        try {
            await api.delete(`/api/Category/${id}`);
            fetchCategories(); // Refresh list
        } catch (error) {
            console.error("Failed to delete category:", error);
        }
    };

    return (
        <div className="container mx-auto p-6">
            <div className="flex justify-between items-center mb-8">
                <h1 className="text-3xl font-bold text-gray-800">Categories Management</h1>
                <button 
                    onClick={() => setShowAddForm(true)}
                    className="bg-green-500 text-white px-4 py-2 rounded-lg hover:bg-green-600"
                >
                    Add New Category
                </button>
            </div>

            {/* Categories List */}
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                {categories.map((category) => (
                    <div key={category.id} className="bg-white shadow-md rounded-lg p-6">
                        <h3 className="text-xl font-semibold text-gray-800">{category.name}</h3>
                        <p className="text-gray-600 mt-1">
                            <span className="font-medium">Daily Rate:</span> {category.dailyRate} HUF/day
                        </p>
                        <div className="flex gap-2 mt-4">
                            <button
                                onClick={() => setEditingCategory(category)}
                                className="bg-blue-500 text-white px-4 py-2 rounded-lg hover:bg-blue-600"
                            >
                                Edit
                            </button>
                            <button
                                onClick={() => handleDeleteCategory(category.id)}
                                className="bg-red-500 text-white px-4 py-2 rounded-lg hover:bg-red-600"
                            >
                                Delete
                            </button>
                        </div>
                    </div>
                ))}
            </div>

            {/* Add Category Modal */}
            {showAddForm && (
                <div className="fixed inset-0 bg-black bg-opacity-50 flex justify-center items-center z-50">
                    <div className="bg-white rounded-lg shadow-lg p-6 w-full max-w-lg">
                        <h2 className="text-xl font-semibold mb-4">Add New Category</h2>
                        
                        <div className="mb-4">
                            <label className="block text-sm font-medium text-gray-700 mb-1">
                                Category Name
                            </label>
                            <input
                                type="text"
                                value={newCategory.name}
                                onChange={(e) => setNewCategory({ ...newCategory, name: e.target.value })}
                                className="block w-full border border-gray-300 rounded-lg p-3"
                                placeholder="e.g., Economy, Luxury, SUV"
                            />
                        </div>
                        
                        <div className="mb-4">
                            <label className="block text-sm font-medium text-gray-700 mb-1">
                                Daily Rate (HUF)
                            </label>
                            <input
                                type="number"
                                value={newCategory.dailyRate}
                                onChange={(e) => setNewCategory({ ...newCategory, dailyRate: parseFloat(e.target.value) })}
                                className="block w-full border border-gray-300 rounded-lg p-3"
                                placeholder="e.g., 10000"
                                step="1"
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
                                onClick={handleAddCategory}
                                className="bg-green-500 text-white px-6 py-2 rounded-lg hover:bg-green-600"
                            >
                                Add Category
                            </button>
                        </div>
                    </div>
                </div>
            )}

            {/* Edit Category Modal */}
            {editingCategory && (
                <div className="fixed inset-0 bg-black bg-opacity-50 flex justify-center items-center z-50">
                    <div className="bg-white rounded-lg shadow-lg p-6 w-full max-w-lg">
                        <h2 className="text-xl font-semibold mb-4">Edit Category</h2>
                        
                        <div className="mb-4">
                            <label className="block text-sm font-medium text-gray-700 mb-1">
                                Category Name
                            </label>
                            <input
                                type="text"
                                value={editingCategory.name}
                                onChange={(e) => setEditingCategory({ ...editingCategory, name: e.target.value })}
                                className="block w-full border border-gray-300 rounded-lg p-3"
                            />
                        </div>
                        
                        <div className="mb-4">
                            <label className="block text-sm font-medium text-gray-700 mb-1">
                                Daily Rate (HUF)
                            </label>
                            <input
                                type="number"
                                value={editingCategory.dailyRate}
                                onChange={(e) => setEditingCategory({ ...editingCategory, dailyRate: parseFloat(e.target.value) })}
                                className="block w-full border border-gray-300 rounded-lg p-3"
                                step="1"
                                min="0"
                            />
                        </div>
                        
                        <div className="flex justify-end gap-4 mt-4">
                            <button
                                onClick={() => setEditingCategory(null)}
                                className="text-gray-500 hover:underline"
                            >
                                Cancel
                            </button>
                            <button
                                onClick={handleUpdateCategory}
                                className="bg-blue-500 text-white px-6 py-2 rounded-lg hover:bg-blue-600"
                            >
                                Save Changes
                            </button>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
}

export default Categories;