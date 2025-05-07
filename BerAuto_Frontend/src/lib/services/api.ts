// src/lib/services/api.ts
import axios from 'axios';

// API base URL
const API_BASE_URL = 'http://localhost:5291';

// Create axios instance with auth token
export const createAxiosInstance = () => {
	const instance = axios.create({
		baseURL: API_BASE_URL,
		headers: {
			'Content-Type': 'application/json'
		}
	});

	// Add auth token if available
	if (typeof window !== 'undefined') {
		const token = localStorage.getItem('accessToken');
		if (token) {
			instance.defaults.headers.common['Authorization'] = `Bearer ${token}`;
		}
	}

	return instance;
};

export const authService = {
	async checkAuthentication() {
		try {
			const api = createAxiosInstance();
			await api.get('/api/Auth/test');
			return true;
		} catch (error) {
			console.error('Auth check error:', error);
			return false;
		}
	}
};

export const carService = {
	async fetchCars() {
		try {
			const api = createAxiosInstance();
			const response = await api.get('/api/Car');
			return {
				data: response.data.data || [],
				error: null
			};
		} catch (error) {
			console.error('Cars fetch error:', error);
			return {
				data: [],
				error: "Failed to load cars."
			};
		}
	},

	async deleteCar(carId) {
		try {
			const api = createAxiosInstance();
			await api.delete(`/api/Car/${carId}`);
			return true;
		} catch (error) {
			console.error('Delete car error:', error);
			return false;
		}
	},

	async updateCar(car) {
		try {
			const api = createAxiosInstance();
			// Create a car DTO with the correct structure
			const carDTO = {
				id: car.id,
				plateNumber: car.plateNumber,
				type: car.type,
				odometer: car.odometer,
				status: car.status,
				categoryId: car.categoryId,
				description: car.description
			};

			await api.post('/api/Car', carDTO);

			// Update category if provided
			if (car.categoryId) {
				await api.put(`/api/Car/category?categoryId=${car.categoryId}`, JSON.stringify(car.id));
			}

			// Update description if provided
			if (car.description) {
				await api.put(`/api/Car/description?description=${encodeURIComponent(car.description)}`, JSON.stringify(car.id));
			}

			// Update status if needed
			await api.put(`/api/Car/status?carStatus=${car.status}`, JSON.stringify(car.id));

			return true;
		} catch (error) {
			console.error('Update car error:', error);
			return false;
		}
	},

	async createCar(car) {
		try {
			const api = createAxiosInstance();
			// Create a car DTO with the correct structure
			const carDTO = {
				id: car.id,
				plateNumber: car.plateNumber,
				type: car.type,
				odometer: car.odometer,
				status: car.status,
				categoryId: car.categoryId,
				description: car.description
			};

			await api.post('/api/Car', carDTO);
			return { success: true, error: null };
		} catch (error) {
			console.error('Create car error:', error);
			return { success: false, error: "Failed to create car. Please try again." };
		}
	}
};


export const categoryService = {
	async fetchCategories() {
		try {
			const api = createAxiosInstance();
			const response = await api.get('/api/Category');
			return {
				data: response.data.data || [],
				error: null
			};
		} catch (error) {
			console.error('Categories fetch error:', error);
			return {
				data: [],
				error: "Failed to load categories."
			};
		}
	},

	async updateCategory(category) {
		try {
			const api = createAxiosInstance();
			await api.put(`/api/Category/name?NewName=${encodeURIComponent(category.name)}`, JSON.stringify(category.id));
			await api.put(`/api/Category/rate?NewRate=${category.dailyRate}`, JSON.stringify(category.id));
			return true;
		} catch (error) {
			console.error('Update category error:', error);
			return false;
		}
	},

	async createCategory(category) {
		try {
			const api = createAxiosInstance();
			await api.post('/api/Category', category);
			return { success: true, error: null };
		} catch (error) {
			console.error('Create category error:', error);
			return { success: false, error: "Failed to create category. Please try again." };
		}
	}
};

export const rentalService = {
	async fetchRentals() {
		try {
			const api = createAxiosInstance();
			const response = await api.get('/api/Rental');
			return {
				data: response.data.data || [],
				error: null
			};
		} catch (error) {
			console.error('Rentals fetch error:', error);
			return {
				data: [],
				error: "Failed to load rentals."
			};
		}
	},

	async createRental(rental) {
		try {
			const api = createAxiosInstance();
			await api.post('/api/Rental/withDetails', rental);
			return { success: true, error: null };
		} catch (error) {
			console.error('Create rental error:', error);
			return { success: false, error: "Failed to create rental. Please try again." };
		}
	}
};

export const userService = {
	async fetchUsers() {
		try {
			const api = createAxiosInstance();
			const response = await api.get('/api/User');
			return {
				data: response.data.data || [],
				error: null
			};
		} catch (error) {
			console.error('Users fetch error:', error);
			return {
				data: [],
				error: "Failed to load users."
			};
		}
	},

	async deleteUser(userId) {
		try {
			const api = createAxiosInstance();
			await api.delete(`/api/User/${userId}`);
			return true;
		} catch (error) {
			console.error('Delete user error:', error);
			return false;
		}
	},

	async updateUser(user) {
		try {
			const api = createAxiosInstance();
			const userDTO = {
				userID: user.id,
				name: user.name,
				email: user.email,
				address: user.address,
				phoneNumber: user.phoneNumber,
				description: user.description,
				override: true
			};
			await api.put('/api/User', userDTO);
			return true;
		} catch (error) {
			console.error('Update user error:', error);
			return false;
		}
	}
};