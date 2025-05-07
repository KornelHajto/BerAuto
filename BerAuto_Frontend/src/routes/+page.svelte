<script lang="ts">
	import { onMount } from 'svelte';
	import axios from 'axios';
	import { goto } from '$app/navigation';
	import { v4 as uuidv4 } from 'uuid';

	// API base URL
	const API_BASE_URL = 'http://localhost:5291';

	// Create axios instance with auth token
	const createAxiosInstance = () => {
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

	// Data state
	let isAuthenticated = false;
	let cars = [];
	let categories = [];
	let rentals = [];
	let users = [];

	// Loading states
	let dashboardLoading = true;
	let carsLoading = true;
	let categoriesLoading = true;
	let rentalsLoading = true;
	let usersLoading = true;

	// Error states
	let carsError = null;
	let categoriesError = null;
	let rentalsError = null;
	let usersError = null;

	// Form visibility states
	let showAddCategory = false;
	let showAddCar = false;
	let showAddRental = false;
	let showEditCar = false;
	let showEditCategory = false;
	let showEditUser = false;

	// Edit states
	let carToEdit = null;
	let categoryToEdit = null;
	let userToEdit = null;

	// Modal state
	let showDeleteConfirmation = false;
	let itemToDelete = null;
	let deleteType = '';

	// Check if user is authenticated
	async function checkAuthentication() {
		try {
			const api = createAxiosInstance();
			await api.get('/api/Auth/test');
			isAuthenticated = true;
			return true;
		} catch (error) {
			console.error('Auth check error:', error);
			return false;
		}
	}

	// Redirect to login if not authenticated
	async function redirectIfNotAuthenticated() {
		const isAuth = await checkAuthentication();
		if (!isAuth) {
			goto('/login');
		}
		return isAuth;
	}

	// Fetch cars
	async function fetchCars() {
		carsLoading = true;
		try {
			const api = createAxiosInstance();
			const response = await api.get('/api/Car');
			// Extract data from standard response format
			cars = response.data.data || [];
			carsError = null;
		} catch (error) {
			cars = [];
			carsError = "Failed to load cars.";
			console.error('Cars fetch error:', error);
		} finally {
			carsLoading = false;
		}
	}

	// Fetch categories with cars
	async function fetchCategories() {
		categoriesLoading = true;
		try {
			const api = createAxiosInstance();
			const response = await api.get('/api/Category');
			// Extract data from standard response format
			categories = response.data.data || [];

			// Map car counts to categories
			categories = categories.map(category => {
				const carsInCategory = cars.filter(car => car.categoryId === category.id);
				return {
					...category,
					carCount: carsInCategory.length
				};
			});

			categoriesError = null;
		} catch (error) {
			categories = [];
			categoriesError = "Failed to load categories.";
			console.error('Categories fetch error:', error);
		} finally {
			categoriesLoading = false;
		}
	}

	// Fetch rentals
	async function fetchRentals() {
		rentalsLoading = true;
		try {
			const api = createAxiosInstance();
			const response = await api.get('/api/Rental');
			// Extract data from standard response format
			rentals = response.data.data || [];
			rentalsError = null;
		} catch (error) {
			rentals = [];
			rentalsError = "Failed to load rentals.";
			console.error('Rentals fetch error:', error);
		} finally {
			rentalsLoading = false;
		}
	}

	// Fetch users
	async function fetchUsers() {
		usersLoading = true;
		try {
			const api = createAxiosInstance();
			const response = await api.get('/api/User');
			// Extract data from standard response format
			users = response.data.data || [];
			usersError = null;
		} catch (error) {
			users = [];
			usersError = "Failed to load users.";
			console.error('Users fetch error:', error);
		} finally {
			usersLoading = false;
		}
	}

	// Fetch all data at once
	async function fetchAllData() {
		dashboardLoading = true;
		try {
			await Promise.all([
				fetchCars(),
				fetchUsers(),
				fetchRentals()
			]);
			// Fetch categories after cars to count cars per category
			await fetchCategories();
		} finally {
			dashboardLoading = false;
		}
	}

	// Delete a car
	async function deleteCar(carId) {
		try {
			const api = createAxiosInstance();
			await api.delete(`/api/Car/${carId}`);
			await fetchCars();
			await fetchCategories();
			showDeleteConfirmation = false;
			return true;
		} catch (error) {
			console.error('Delete car error:', error);
			return false;
		}
	}

	// Delete a user
	async function deleteUser(userId) {
		try {
			const api = createAxiosInstance();
			await api.delete(`/api/User/${userId}`);
			await fetchUsers();
			showDeleteConfirmation = false;
			return true;
		} catch (error) {
			console.error('Delete user error:', error);
			return false;
		}
	}

	// Update car
	async function updateCar(car) {
		try {
			const api = createAxiosInstance();

			// Update basic car info
			await api.post('/api/Car', car);

			// Update category if needed
			if (car.categoryId) {
				await api.put(`/api/Car/category?categoryId=${car.categoryId}`, JSON.stringify(car.id));
			}

			// Update description if provided
			if (car.description) {
				await api.put(`/api/Car/description?description=${encodeURIComponent(car.description)}`, JSON.stringify(car.id));
			}

			await fetchCars();
			await fetchCategories();

			showEditCar = false;
			carToEdit = null;

			return true;
		} catch (error) {
			console.error('Update car error:', error);
			return false;
		}
	}

	// Update category
	async function updateCategory(category) {
		try {
			const api = createAxiosInstance();

			// Update category name
			await api.put(`/api/Category/name?NewName=${encodeURIComponent(category.name)}`, JSON.stringify(category.id));

			// Update daily rate
			await api.put(`/api/Category/rate?NewRate=${category.dailyRate}`, JSON.stringify(category.id));

			await fetchCategories();

			showEditCategory = false;
			categoryToEdit = null;

			return true;
		} catch (error) {
			console.error('Update category error:', error);
			return false;
		}
	}

	// Update user
	async function updateUser(user) {
		try {
			const api = createAxiosInstance();

			// Create user data DTO
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

			await fetchUsers();

			showEditUser = false;
			userToEdit = null;

			return true;
		} catch (error) {
			console.error('Update user error:', error);
			return false;
		}
	}

	// When component mounts, check auth and load data
	onMount(async () => {
		const isAuth = await redirectIfNotAuthenticated();
		if (isAuth) {
			fetchAllData();
		}
	});

	// Category form data
	let newCategory = {
		name: "",
		dailyRate: 0
	};
	let categoryError = null;
	let categorySuccess = null;

	// Car form data
	let newCar = {
		id: "",
		plateNumber: "",
		type: "",
		odometer: 0,
		available: true,
		categoryId: "",
		description: ""
	};
	let carError = null;
	let carSuccess = null;

	// Rental form data
	let newRental = {
		renterId: "",
		carId: "",
		startDate: "",
		endDate: ""
	};
	let rentalError = null;
	let rentalSuccess = null;

	// Create category
	async function createCategory() {
		categoryError = null;
		categorySuccess = null;

		if (!newCategory.name) {
			categoryError = "Category name is required";
			return;
		}

		if (newCategory.dailyRate <= 0) {
			categoryError = "Daily rate must be greater than 0";
			return;
		}

		try {
			const api = createAxiosInstance();
			await api.post('/api/Category', newCategory);

			categorySuccess = `Category "${newCategory.name}" created successfully!`;

			// Reset form
			newCategory = {
				name: "",
				dailyRate: 0
			};

			// Refresh categories
			await fetchCategories();
		} catch (error) {
			console.error('Create category error:', error);
			categoryError = "Failed to create category. Please try again.";
		}
	}

	// Create car
	async function createCar() {
		carError = null;
		carSuccess = null;

		if (!newCar.plateNumber) {
			carError = "Plate number is required";
			return;
		}

		if (!newCar.type) {
			carError = "Car type is required";
			return;
		}

		if (newCar.odometer < 0) {
			carError = "Odometer must be a positive number";
			return;
		}

		try {
			const api = createAxiosInstance();

			// Generate a UUID if not provided
			if (!newCar.id) {
				newCar.id = uuidv4();
			}

			await api.post('/api/Car', newCar);

			carSuccess = `Car "${newCar.type}" with plate "${newCar.plateNumber}" created successfully!`;

			// Reset form
			newCar = {
				id: "",
				plateNumber: "",
				type: "",
				odometer: 0,
				available: true,
				categoryId: "",
				description: ""
			};

			// Refresh cars and categories
			await fetchCars();
			await fetchCategories();
		} catch (error) {
			console.error('Create car error:', error);
			carError = "Failed to create car. Please try again.";
		}
	}

	// Create rental
	async function createRental() {
		rentalError = null;
		rentalSuccess = null;

		if (!newRental.renterId) {
			rentalError = "Renter is required";
			return;
		}

		if (!newRental.carId) {
			rentalError = "Car is required";
			return;
		}

		if (!newRental.startDate || !newRental.endDate) {
			rentalError = "Start and end dates are required";
			return;
		}

		// Validate dates
		const startDate = new Date(newRental.startDate);
		const endDate = new Date(newRental.endDate);

		if (startDate >= endDate) {
			rentalError = "End date must be after start date";
			return;
		}

		try {
			const api = createAxiosInstance();
			await api.post('/api/Rental/withDetails', newRental);

			rentalSuccess = "Rental created successfully!";

			// Reset form
			newRental = {
				renterId: "",
				carId: "",
				startDate: "",
				endDate: ""
			};

			// Refresh rentals
			await fetchRentals();
		} catch (error) {
			console.error('Create rental error:', error);
			rentalError = "Failed to create rental. Please try again.";
		}
	}

	// Update rental status
	async function updateRentalStatus(rentalId, newStatus) {
		try {
			const api = createAxiosInstance();
			await api.put('/api/Rental/status', {
				rentalId,
				newStatus
			});

			// Refresh rentals after update
			await fetchRentals();
			return true;
		} catch (error) {
			console.error('Error updating rental status:', error);
			return false;
		}
	}

	function logout() {
		localStorage.removeItem('accessToken');
		localStorage.removeItem('refreshToken');
		goto('/login');
	}

	// Status text mapping for better readability
	function getRentalStatusText(status) {
		switch(status) {
			case 0: return 'Pending';
			case 1: return 'Active';
			case 2: return 'Completed';
			case 3: return 'Cancelled';
			default: return 'Unknown';
		}
	}

	// Get the action text for rental status button
	function getRentalActionText(status) {
		switch(status) {
			case 0: return 'Activate';
			case 1: return 'Complete';
			case 2: return 'Reopen';
			default: return 'Update';
		}
	}

	// Next status logic for rentals
	function getNextRentalStatus(status) {
		switch(status) {
			case 0: return 1; // Pending -> Active
			case 1: return 2; // Active -> Completed
			case 2: return 0; // Completed -> Pending (reopen)
			default: return 0;
		}
	}

	// Format date for display
	function formatDate(dateString) {
		if (!dateString) return 'N/A';

		const date = new Date(dateString);
		return date.toLocaleDateString('en-US', {
			year: 'numeric',
			month: 'short',
			day: 'numeric',
			hour: '2-digit',
			minute: '2-digit'
		});
	}

	// Toggle car availability
	async function toggleCarAvailability(carId, currentStatus) {
		try {
			const api = createAxiosInstance();
			const newStatus = !currentStatus;

			await api.put(
				`/api/Car/available?available=${newStatus}`,
				JSON.stringify(carId)
			);

			// Refresh cars data
			await fetchCars();
			return true;
		} catch (error) {
			console.error('Error updating car availability:', error);
			return false;
		}
	}

	// Open edit car form
	function openEditCar(car) {
		carToEdit = { ...car };
		showEditCar = true;
		showEditCategory = false;
		showEditUser = false;
		showAddCar = false;
		showAddCategory = false;
		showAddRental = false;
	}

	// Open edit category form
	function openEditCategory(category) {
		categoryToEdit = { ...category };
		showEditCategory = true;
		showEditCar = false;
		showEditUser = false;
		showAddCar = false;
		showAddCategory = false;
		showAddRental = false;
	}

	// Open edit user form
	function openEditUser(user) {
		userToEdit = { ...user };
		showEditUser = true;
		showEditCar = false;
		showEditCategory = false;
		showAddCar = false;
		showAddCategory = false;
		showAddRental = false;
	}

	// Show delete confirmation modal
	function confirmDelete(type, item) {
		deleteType = type;
		itemToDelete = item;
		showDeleteConfirmation = true;
	}

	// Handle delete confirmation
	async function handleDeleteConfirm() {
		if (deleteType === 'car' && itemToDelete) {
			await deleteCar(itemToDelete.id);
		} else if (deleteType === 'user' && itemToDelete) {
			await deleteUser(itemToDelete.id);
		}

		showDeleteConfirmation = false;
		itemToDelete = null;
		deleteType = '';
	}

	// Get role name by access level
	function getUserRoleName(accessLevel) {
		switch(accessLevel) {
			case 0: return 'Customer';
			case 1: return 'Staff';
			case 2: return 'Admin';
			default: return 'Unknown';
		}
	}

	// Hide all forms
	function hideAllForms() {
		showAddCar = false;
		showAddCategory = false;
		showAddRental = false;
		showEditCar = false;
		showEditCategory = false;
		showEditUser = false;
	}
</script>

<svelte:head>
	<title>BerAuto Management Portal</title>
</svelte:head>

{#if dashboardLoading}
	<div class="flex justify-center items-center min-h-screen bg-gray-100">
		<div class="flex flex-col items-center">
			<div class="w-12 h-12 border-4 border-gray-300 border-t-indigo-700 rounded-full animate-spin"></div>
			<p class="mt-4 text-gray-700 text-lg">Loading dashboard...</p>
		</div>
	</div>
{:else}
	<div class="min-h-screen bg-gray-100 flex">
		<!-- Sidebar -->
		<div class="bg-indigo-900 text-white w-64 flex-shrink-0 hidden md:block shadow-lg">
			<div class="p-6 border-b border-indigo-800">
				<h1 class="text-xl font-semibold">BerAuto Fleet</h1>
				<p class="text-indigo-300 text-sm">Management Portal</p>
			</div>

			<nav class="mt-4">
				<div class="px-4 py-2 text-indigo-300 text-xs font-semibold">MAIN NAVIGATION</div>

				<a href="#dashboard" class="block px-6 py-3 text-indigo-100 hover:bg-indigo-800 hover:text-white">
          <span class="flex items-center">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-3" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 6a2 2 0 012-2h2a2 2 0 012 2v2a2 2 0 01-2 2H6a2 2 0 01-2-2V6zM14 6a2 2 0 012-2h2a2 2 0 012 2v2a2 2 0 01-2 2h-2a2 2 0 01-2-2V6zM4 16a2 2 0 012-2h2a2 2 0 012 2v2a2 2 0 01-2 2H6a2 2 0 01-2-2v-2zM14 16a2 2 0 012-2h2a2 2 0 012 2v2a2 2 0 01-2 2h-2a2 2 0 01-2-2v-2z" />
            </svg>
            Dashboard
          </span>
				</a>

				<a href="#cars" class="block px-6 py-3 text-indigo-100 hover:bg-indigo-800 hover:text-white">
          <span class="flex items-center">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-3" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7h12m0 0l-4-4m4 4l-4 4m0 6H4m0 0l4 4m-4-4l4-4" />
            </svg>
            Fleet Management
          </span>
				</a>

				<a href="#rentals" class="block px-6 py-3 text-indigo-100 hover:bg-indigo-800 hover:text-white">
          <span class="flex items-center">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-3" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2" />
            </svg>
            Rentals & Bookings
          </span>
				</a>

				<a href="#users" class="block px-6 py-3 text-indigo-100 hover:bg-indigo-800 hover:text-white">
          <span class="flex items-center">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-3" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197M13 7a4 4 0 11-8 0 4 4 0 018 0z" />
            </svg>
            User Management
          </span>
				</a>

				<div class="px-4 py-2 mt-6 text-indigo-300 text-xs font-semibold">SYSTEM</div>

				<button
					on:click={logout}
					class="block w-full text-left px-6 py-3 text-indigo-100 hover:bg-indigo-800 hover:text-white"
				>
          <span class="flex items-center">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-3" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1" />
            </svg>
            Logout
          </span>
				</button>
			</nav>
		</div>

		<!-- Main Content -->
		<div class="flex-1 flex flex-col">
			<!-- Top Navbar -->
			<header class="bg-white shadow-sm z-10">
				<div class="flex items-center justify-between h-16 px-6">
					<!-- Mobile menu button -->
					<button class="md:hidden text-gray-500 focus:outline-none">
						<svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
							<path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 6h16M4 12h16M4 18h16" />
						</svg>
					</button>

					<div class="md:hidden">
						<h1 class="text-lg font-semibold">BerAuto Fleet</h1>
					</div>

					<!-- User dropdown -->
					<div class="flex items-center">
						<div class="relative">
							<button
								class="flex items-center text-gray-700 focus:outline-none"
								on:click={logout}
							>
								<div class="h-8 w-8 rounded-full bg-indigo-600 flex items-center justify-center text-white font-semibold">
									A
								</div>
								<span class="ml-2 text-sm font-medium hidden md:block">Admin</span>
								<svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 ml-1 hidden md:block" fill="none" viewBox="0 0 24 24" stroke="currentColor">
									<path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7" />
								</svg>
							</button>
						</div>
					</div>
				</div>
			</header>

			<!-- Main dashboard content -->
			<main class="flex-1 overflow-y-auto bg-gray-100 p-6">
				<div class="mb-8">
					<h1 class="text-2xl font-semibold text-gray-800">Dashboard Overview</h1>
					<p class="text-gray-600">Welcome to the BerAuto Fleet Management portal.</p>
				</div>

				<!-- Quick Stats Cards -->
				<div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
					<div class="bg-white rounded-lg border border-gray-200 shadow-sm p-6">
						<div class="flex items-center">
							<div class="p-3 rounded-full bg-indigo-100 text-indigo-800">
								<svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
									<path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
								</svg>
							</div>
							<div class="ml-4">
								<h2 class="text-gray-600 text-sm font-semibold uppercase">Total Vehicles</h2>
								<p class="text-2xl font-bold text-gray-800">{carsLoading ? '...' : cars.length}</p>
								<p class="text-sm text-indigo-600">
									{carsLoading ? '' : `${cars.filter(car => car.available).length} available`}
								</p>
							</div>
						</div>
					</div>

					<div class="bg-white rounded-lg border border-gray-200 shadow-sm p-6">
						<div class="flex items-center">
							<div class="p-3 rounded-full bg-green-100 text-green-800">
								<svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
									<path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M7 7h.01M7 3h5c.512 0 1.024.195 1.414.586l7 7a2 2 0 010 2.828l-7 7a2 2 0 01-2.828 0l-7-7A1.994 1.994 0 013 12V7a4 4 0 014-4z" />
								</svg>
							</div>
							<div class="ml-4">
								<h2 class="text-gray-600 text-sm font-semibold uppercase">Categories</h2>
								<p class="text-2xl font-bold text-gray-800">{categoriesLoading ? '...' : categories.length}</p>
								<p class="text-sm text-green-600">Vehicle classifications</p>
							</div>
						</div>
					</div>

					<div class="bg-white rounded-lg border border-gray-200 shadow-sm p-6">
						<div class="flex items-center">
							<div class="p-3 rounded-full bg-purple-100 text-purple-800">
								<svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
									<path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2" />
								</svg>
							</div>
							<div class="ml-4">
								<h2 class="text-gray-600 text-sm font-semibold uppercase">Active Rentals</h2>
								<p class="text-2xl font-bold text-gray-800">
									{rentalsLoading ? '...' : rentals.filter(rental => rental.status === 1).length}
								</p>
								<p class="text-sm text-purple-600">
									{rentalsLoading ? '' : `${rentals.length} total rentals`}
								</p>
							</div>
						</div>
					</div>

					<div class="bg-white rounded-lg border border-gray-200 shadow-sm p-6">
						<div class="flex items-center">
							<div class="p-3 rounded-full bg-blue-100 text-blue-800">
								<svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
									<path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197M13 7a4 4 0 11-8 0 4 4 0 018 0z" />
								</svg>
							</div>
							<div class="ml-4">
								<h2 class="text-gray-600 text-sm font-semibold uppercase">Registered Users</h2>
								<p class="text-2xl font-bold text-gray-800">{usersLoading ? '...' : users.length}</p>
								<p class="text-sm text-blue-600">
									{usersLoading ? '' : `${users.filter(user => user.accesLevel === 0).length} customers`}
								</p>
							</div>
						</div>
					</div>
				</div>

				<!-- Action Buttons -->
				<div class="mb-8">
					<div class="flex flex-wrap gap-4">
						<button
							class="bg-indigo-700 hover:bg-indigo-800 text-white font-medium py-2 px-4 rounded-md shadow-sm flex items-center"
							on:click={() => {
                showAddCar = !showAddCar;
                showAddCategory = false;
                showAddRental = false;
                showEditCar = false;
                showEditCategory = false;
                showEditUser = false;
              }}
						>
							<svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-2" fill="none" viewBox="0 0 24 24" stroke="currentColor">
								<path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6" />
							</svg>
							{showAddCar ? 'Cancel' : 'Add New Vehicle'}
						</button>

						<button
							class="bg-green-700 hover:bg-green-800 text-white font-medium py-2 px-4 rounded-md shadow-sm flex items-center"
							on:click={() => {
                showAddCategory = !showAddCategory;
                showAddCar = false;
                showAddRental = false;
                showEditCar = false;
                showEditCategory = false;
                showEditUser = false;
              }}
						>
							<svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-2" fill="none" viewBox="0 0 24 24" stroke="currentColor">
								<path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6" />
							</svg>
							{showAddCategory ? 'Cancel' : 'Add Category'}
						</button>

						<button
							class="bg-purple-700 hover:bg-purple-800 text-white font-medium py-2 px-4 rounded-md shadow-sm flex items-center"
							on:click={() => {
                showAddRental = !showAddRental;
                showAddCar = false;
                showAddCategory = false;
                showEditCar = false;
                showEditCategory = false;
                showEditUser = false;
              }}
						>
							<svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-2" fill="none" viewBox="0 0 24 24" stroke="currentColor">
								<path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6" />
							</svg>
							{showAddRental ? 'Cancel' : 'Create Rental'}
						</button>

						<button
							class="bg-gray-700 hover:bg-gray-800 text-white font-medium py-2 px-4 rounded-md shadow-sm flex items-center"
							on:click={fetchAllData}
						>
							<svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 mr-2" fill="none" viewBox="0 0 24 24" stroke="currentColor">
								<path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15" />
							</svg>
							Refresh Data
						</button>
					</div>
				</div>

				<!-- Forms -->
				{#if showAddCar || showAddCategory || showAddRental || showEditCar || showEditCategory || showEditUser}
					<div class="bg-white rounded-lg border border-gray-200 shadow-sm mb-8 overflow-hidden">
						{#if showAddCar}
							<div class="border-b border-gray-200 px-6 py-4 bg-gray-50">
								<h2 class="text-lg font-semibold text-gray-800">Add New Vehicle</h2>
							</div>
							<div class="p-6">
								{#if carError}
									<div class="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded mb-4">
										<p>{carError}</p>
									</div>
								{/if}

								{#if carSuccess}
									<div class="bg-green-50 border border-green-200 text-green-700 px-4 py-3 rounded mb-4">
										<p>{carSuccess}</p>
									</div>
								{/if}

								<form on:submit|preventDefault={createCar} class="space-y-4">
									<div class="grid grid-cols-1 md:grid-cols-2 gap-4">
										<div>
											<label for="car-type" class="block text-sm font-medium text-gray-700 mb-1">Car Type/Model</label>
											<input
												type="text"
												id="car-type"
												bind:value={newCar.type}
												class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-indigo-500"
												required
											/>
										</div>

										<div>
											<label for="plate-number" class="block text-sm font-medium text-gray-700 mb-1">Plate Number</label>
											<input
												type="text"
												id="plate-number"
												bind:value={newCar.plateNumber}
												class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-indigo-500"
												required
											/>
										</div>
									</div>

									<div class="grid grid-cols-1 md:grid-cols-2 gap-4">
										<div>
											<label for="odometer" class="block text-sm font-medium text-gray-700 mb-1">Odometer (km)</label>
											<input
												type="number"
												id="odometer"
												bind:value={newCar.odometer}
												min="0"
												class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-indigo-500"
												required
											/>
										</div>

										<div>
											<label for="category" class="block text-sm font-medium text-gray-700 mb-1">Category</label>
											<select
												id="category"
												bind:value={newCar.categoryId}
												class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-indigo-500"
											>
												<option value="">-- Select Category --</option>
												{#each categories as category}
													<option value={category.id}>{category.name} (${category.dailyRate}/day)</option>
												{/each}
											</select>
										</div>
									</div>

									<div class="flex items-center">
										<input
											type="checkbox"
											id="available"
											bind:checked={newCar.available}
											class="h-4 w-4 text-indigo-600 focus:ring-indigo-500 border-gray-300 rounded"
										/>
										<label for="available" class="ml-2 block text-sm text-gray-700">Available for rent</label>
									</div>

									<div>
										<label for="description" class="block text-sm font-medium text-gray-700 mb-1">Description (optional)</label>
										<textarea
											id="description"
											bind:value={newCar.description}
											rows="3"
											class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-indigo-500"
										></textarea>
									</div>

									<div class="pt-2">
										<button
											type="submit"
											class="w-full bg-indigo-600 text-white py-2 px-4 rounded-md hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:ring-offset-2"
										>
											Add Vehicle
										</button>
									</div>
								</form>
							</div>
						{/if}

						{#if showEditCar}
							<div class="border-b border-gray-200 px-6 py-4 bg-gray-50">
								<h2 class="text-lg font-semibold text-gray-800">Edit Vehicle</h2>
							</div>
							<div class="p-6">
								<form on:submit|preventDefault={() => updateCar(carToEdit)} class="space-y-4">
									<div class="grid grid-cols-1 md:grid-cols-2 gap-4">
										<div>
											<label for="edit-car-type" class="block text-sm font-medium text-gray-700 mb-1">Car Type/Model</label>
											<input
												type="text"
												id="edit-car-type"
												bind:value={carToEdit.type}
												class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-indigo-500"
												required
											/>
										</div>

										<div>
											<label for="edit-plate-number" class="block text-sm font-medium text-gray-700 mb-1">Plate Number</label>
											<input
												type="text"
												id="edit-plate-number"
												bind:value={carToEdit.plateNumber}
												class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-indigo-500"
												required
											/>
										</div>
									</div>

									<div class="grid grid-cols-1 md:grid-cols-2 gap-4">
										<div>
											<label for="edit-odometer" class="block text-sm font-medium text-gray-700 mb-1">Odometer (km)</label>
											<input
												type="number"
												id="edit-odometer"
												bind:value={carToEdit.odometer}
												min="0"
												class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-indigo-500"
												required
											/>
										</div>

										<div>
											<label for="edit-category" class="block text-sm font-medium text-gray-700 mb-1">Category</label>
											<select
												id="edit-category"
												bind:value={carToEdit.categoryId}
												class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-indigo-500"
											>
												<option value="">-- Select Category --</option>
												{#each categories as category}
													<option value={category.id}>{category.name} (${category.dailyRate}/day)</option>
												{/each}
											</select>
										</div>
									</div>

									<div class="flex items-center">
										<input
											type="checkbox"
											id="edit-available"
											bind:checked={carToEdit.available}
											class="h-4 w-4 text-indigo-600 focus:ring-indigo-500 border-gray-300 rounded"
										/>
										<label for="edit-available" class="ml-2 block text-sm text-gray-700">Available for rent</label>
									</div>

									<div>
										<label for="edit-description" class="block text-sm font-medium text-gray-700 mb-1">Description (optional)</label>
										<textarea
											id="edit-description"
											bind:value={carToEdit.description}
											rows="3"
											class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-indigo-500"
										></textarea>
									</div>

									<div class="pt-2 flex space-x-4">
										<button
											type="submit"
											class="flex-1 bg-indigo-600 text-white py-2 px-4 rounded-md hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:ring-offset-2"
										>
											Save Changes
										</button>

										<button
											type="button"
											on:click={() => {
                        showEditCar = false;
                        carToEdit = null;
                      }}
											class="flex-1 bg-gray-200 text-gray-800 py-2 px-4 rounded-md hover:bg-gray-300 focus:outline-none focus:ring-2 focus:ring-gray-500 focus:ring-offset-2"
										>
											Cancel
										</button>
									</div>
								</form>
							</div>
						{/if}

						{#if showAddCategory}
							<div class="border-b border-gray-200 px-6 py-4 bg-gray-50">
								<h2 class="text-lg font-semibold text-gray-800">Add New Category</h2>
							</div>
							<div class="p-6">
								{#if categoryError}
									<div class="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded mb-4">
										<p>{categoryError}</p>
									</div>
								{/if}

								{#if categorySuccess}
									<div class="bg-green-50 border border-green-200 text-green-700 px-4 py-3 rounded mb-4">
										<p>{categorySuccess}</p>
									</div>
								{/if}

								<form on:submit|preventDefault={createCategory} class="space-y-4">
									<div>
										<label for="category-name" class="block text-sm font-medium text-gray-700 mb-1">Category Name</label>
										<input
											type="text"
											id="category-name"
											bind:value={newCategory.name}
											class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-green-500"
											required
										/>
									</div>

									<div>
										<label for="daily-rate" class="block text-sm font-medium text-gray-700 mb-1">Daily Rate ($/day)</label>
										<input
											type="number"
											id="daily-rate"
											bind:value={newCategory.dailyRate}
											min="1"
											class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-green-500"
											required
										/>
									</div>

									<div class="pt-2">
										<button
											type="submit"
											class="w-full bg-green-600 text-white py-2 px-4 rounded-md hover:bg-green-700 focus:outline-none focus:ring-2 focus:ring-green-500 focus:ring-offset-2"
										>
											Create Category
										</button>
									</div>
								</form>
							</div>
						{/if}

						{#if showEditCategory}
							<div class="border-b border-gray-200 px-6 py-4 bg-gray-50">
								<h2 class="text-lg font-semibold text-gray-800">Edit Category</h2>
							</div>
							<div class="p-6">
								<form on:submit|preventDefault={() => updateCategory(categoryToEdit)} class="space-y-4">
									<div>
										<label for="edit-category-name" class="block text-sm font-medium text-gray-700 mb-1">Category Name</label>
										<input
											type="text"
											id="edit-category-name"
											bind:value={categoryToEdit.name}
											class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-green-500"
											required
										/>
									</div>

									<div>
										<label for="edit-daily-rate" class="block text-sm font-medium text-gray-700 mb-1">Daily Rate ($/day)</label>
										<input
											type="number"
											id="edit-daily-rate"
											bind:value={categoryToEdit.dailyRate}
											min="1"
											class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-green-500"
											required
										/>
									</div>

									<div class="pt-2 flex space-x-4">
										<button
											type="submit"
											class="flex-1 bg-green-600 text-white py-2 px-4 rounded-md hover:bg-green-700 focus:outline-none focus:ring-2 focus:ring-green-500 focus:ring-offset-2"
										>
											Save Changes
										</button>

										<button
											type="button"
											on:click={() => {
                        showEditCategory = false;
                        categoryToEdit = null;
                      }}
											class="flex-1 bg-gray-200 text-gray-800 py-2 px-4 rounded-md hover:bg-gray-300 focus:outline-none focus:ring-2 focus:ring-gray-500 focus:ring-offset-2"
										>
											Cancel
										</button>
									</div>
								</form>
							</div>
						{/if}

						{#if showEditUser}
							<div class="border-b border-gray-200 px-6 py-4 bg-gray-50">
								<h2 class="text-lg font-semibold text-gray-800">Edit User</h2>
							</div>
							<div class="p-6">
								<form on:submit|preventDefault={() => updateUser(userToEdit)} class="space-y-4">
									<div class="grid grid-cols-1 md:grid-cols-2 gap-4">
										<div>
											<label for="edit-user-name" class="block text-sm font-medium text-gray-700 mb-1">Name</label>
											<input
												type="text"
												id="edit-user-name"
												bind:value={userToEdit.name}
												class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
												required
											/>
										</div>

										<div>
											<label for="edit-user-email" class="block text-sm font-medium text-gray-700 mb-1">Email</label>
											<input
												type="email"
												id="edit-user-email"
												bind:value={userToEdit.email}
												class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
												required
											/>
										</div>
									</div>

									<div class="grid grid-cols-1 md:grid-cols-2 gap-4">
										<div>
											<label for="edit-user-phone" class="block text-sm font-medium text-gray-700 mb-1">Phone Number</label>
											<input
												type="text"
												id="edit-user-phone"
												bind:value={userToEdit.phoneNumber}
												class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
												required
											/>
										</div>

										<div>
											<label for="edit-user-address" class="block text-sm font-medium text-gray-700 mb-1">Address</label>
											<input
												type="text"
												id="edit-user-address"
												bind:value={userToEdit.address}
												class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
												required
											/>
										</div>
									</div>

									<div>
										<label for="edit-user-description" class="block text-sm font-medium text-gray-700 mb-1">Description (optional)</label>
										<textarea
											id="edit-user-description"
											bind:value={userToEdit.description}
											rows="3"
											class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
										></textarea>
									</div>

									<div class="pt-2 flex space-x-4">
										<button
											type="submit"
											class="flex-1 bg-blue-600 text-white py-2 px-4 rounded-md hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2"
										>
											Save Changes
										</button>

										<button
											type="button"
											on:click={() => {
                        showEditUser = false;
                        userToEdit = null;
                      }}
											class="flex-1 bg-gray-200 text-gray-800 py-2 px-4 rounded-md hover:bg-gray-300 focus:outline-none focus:ring-2 focus:ring-gray-500 focus:ring-offset-2"
										>
											Cancel
										</button>
									</div>
								</form>
							</div>
						{/if}

						{#if showAddRental}
							<div class="border-b border-gray-200 px-6 py-4 bg-gray-50">
								<h2 class="text-lg font-semibold text-gray-800">Create New Rental</h2>
							</div>
							<div class="p-6">
								{#if rentalError}
									<div class="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded mb-4">
										<p>{rentalError}</p>
									</div>
								{/if}

								{#if rentalSuccess}
									<div class="bg-green-50 border border-green-200 text-green-700 px-4 py-3 rounded mb-4">
										<p>{rentalSuccess}</p>
									</div>
								{/if}

								<form on:submit|preventDefault={createRental} class="space-y-4">
									<div class="grid grid-cols-1 md:grid-cols-2 gap-4">
										<div>
											<label for="renter" class="block text-sm font-medium text-gray-700 mb-1">Renter</label>
											<select
												id="renter"
												bind:value={newRental.renterId}
												class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-purple-500"
												required
											>
												<option value="">-- Select Renter --</option>
												{#each users as user}
													<option value={user.id}>{user.name} ({user.email})</option>
												{/each}
											</select>
										</div>

										<div>
											<label for="rental-car" class="block text-sm font-medium text-gray-700 mb-1">Vehicle</label>
											<select
												id="rental-car"
												bind:value={newRental.carId}
												class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-purple-500"
												required
											>
												<option value="">-- Select Vehicle --</option>
												{#each cars.filter(car => car.available) as car}
													<option value={car.id}>{car.type} ({car.plateNumber})</option>
												{/each}
											</select>
										</div>
									</div>

									<div class="grid grid-cols-1 md:grid-cols-2 gap-4">
										<div>
											<label for="start-date" class="block text-sm font-medium text-gray-700 mb-1">Start Date</label>
											<input
												type="datetime-local"
												id="start-date"
												bind:value={newRental.startDate}
												class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-purple-500"
												required
											/>
										</div>

										<div>
											<label for="end-date" class="block text-sm font-medium text-gray-700 mb-1">End Date</label>
											<input
												type="datetime-local"
												id="end-date"
												bind:value={newRental.endDate}
												class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-purple-500"
												required
											/>
										</div>
									</div>

									<div class="pt-2">
										<button
											type="submit"
											class="w-full bg-purple-600 text-white py-2 px-4 rounded-md hover:bg-purple-700 focus:outline-none focus:ring-2 focus:ring-purple-500 focus:ring-offset-2"
										>
											Create Rental
										</button>
									</div>
								</form>
							</div>
						{/if}
					</div>
				{/if}

				<!-- Delete Confirmation Modal -->
				{#if showDeleteConfirmation}
					<div class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
						<div class="bg-white rounded-lg max-w-md w-full p-6 shadow-xl">
							<h3 class="text-lg font-semibold text-gray-900 mb-2">Confirm Delete</h3>

							<p class="text-gray-700 mb-6">
								Are you sure you want to delete this {deleteType}?
								{#if deleteType === 'car' && itemToDelete}
									<span class="font-medium">{itemToDelete.type} ({itemToDelete.plateNumber})</span>
								{:else if deleteType === 'user' && itemToDelete}
									<span class="font-medium">{itemToDelete.name}</span>
								{/if}
								This action cannot be undone.
							</p>

							<div class="flex space-x-3">
								<button
									class="flex-1 bg-red-600 text-white py-2 px-4 rounded-md hover:bg-red-700 focus:outline-none focus:ring-2 focus:ring-red-500 focus:ring-offset-2"
									on:click={handleDeleteConfirm}
								>
									Delete
								</button>

								<button
									class="flex-1 bg-gray-200 text-gray-800 py-2 px-4 rounded-md hover:bg-gray-300 focus:outline-none focus:ring-2 focus:ring-gray-500 focus:ring-offset-2"
									on:click={() => {
                    showDeleteConfirmation = false;
                    itemToDelete = null;
                    deleteType = '';
                  }}
								>
									Cancel
								</button>
							</div>
						</div>
					</div>
				{/if}

				<!-- Content Sections -->
				<div class="grid grid-cols-1 lg:grid-cols-2 gap-6 mb-8">
					<!-- Cars Section -->
					<div id="cars" class="bg-white rounded-lg border border-gray-200 shadow-sm overflow-hidden">
						<div class="border-b border-gray-200 px-6 py-4 bg-gray-50 flex justify-between items-center">
							<h2 class="text-lg font-semibold text-gray-800">Fleet Overview</h2>
							<button
								class="text-indigo-600 hover:text-indigo-800 text-sm font-medium flex items-center"
								on:click={() => {
                  showAddCar = true;
                  showAddCategory = false;
                  showAddRental = false;
                  showEditCar = false;
                  showEditCategory = false;
                  showEditUser = false;
                }}
							>
								<svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4 mr-1" fill="none" viewBox="0 0 24 24" stroke="currentColor">
									<path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6" />
								</svg>
								Add Vehicle
							</button>
						</div>
						<div class="p-6">
							{#if carsLoading}
								<div class="flex justify-center items-center h-32">
									<div class="w-8 h-8 border-4 border-gray-300 border-t-indigo-600 rounded-full animate-spin"></div>
								</div>
							{:else if carsError}
								<div class="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded mb-4">
									<p>{carsError}</p>
								</div>
							{:else if cars.length === 0}
								<div class="bg-gray-50 border border-gray-200 text-gray-600 px-4 py-5 rounded text-center">
									<p>No vehicles found in the fleet.</p>
									<button
										class="mt-3 text-indigo-600 hover:text-indigo-800 text-sm font-medium"
										on:click={() => {
                      showAddCar = true;
                      showAddCategory = false;
                      showAddRental = false;
                      showEditCar = false;
                      showEditCategory = false;
                      showEditUser = false;
                    }}
									>
										Add your first vehicle
									</button>
								</div>
							{:else}
								<div class="overflow-x-auto">
									<table class="min-w-full divide-y divide-gray-200">
										<thead>
										<tr>
											<th class="px-4 py-3 bg-gray-50 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Type</th>
											<th class="px-4 py-3 bg-gray-50 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Plate</th>
											<th class="px-4 py-3 bg-gray-50 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Odometer</th>
											<th class="px-4 py-3 bg-gray-50 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Status</th>
											<th class="px-4 py-3 bg-gray-50 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Actions</th>
										</tr>
										</thead>
										<tbody class="bg-white divide-y divide-gray-200">
										{#each cars.slice(0, 5) as car}
											<tr class="hover:bg-gray-50">
												<td class="px-4 py-3 whitespace-nowrap">
													<div class="text-sm font-medium text-gray-900">{car.type}</div>
													{#if car.categoryId}
														<div class="text-xs text-gray-500">
															{categories.find(c => c.id === car.categoryId)?.name || 'Unknown category'}
														</div>
													{/if}
												</td>
												<td class="px-4 py-3 whitespace-nowrap text-sm text-gray-900">{car.plateNumber}</td>
												<td class="px-4 py-3 whitespace-nowrap text-sm text-gray-900">{car.odometer} km</td>
												<td class="px-4 py-3 whitespace-nowrap">
                            <span class={`px-2 py-1 inline-flex text-xs leading-5 font-medium rounded-full ${car.available ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'}`}>
                              {car.available ? 'Available' : 'Unavailable'}
                            </span>
												</td>
												<td class="px-4 py-3 whitespace-nowrap text-sm flex space-x-3">
													<button
														class="text-indigo-600 hover:text-indigo-900"
														on:click={() => toggleCarAvailability(car.id, car.available)}
													>
														{car.available ? 'Mark Unavailable' : 'Make Available'}
													</button>

													<button
														class="text-blue-600 hover:text-blue-900"
														on:click={() => openEditCar(car)}
													>
														Edit
													</button>

													<button
														class="text-red-600 hover:text-red-900"
														on:click={() => confirmDelete('car', car)}
													>
														Delete
													</button>
												</td>
											</tr>
										{/each}
										</tbody>
									</table>
								</div>

								{#if cars.length > 5}
									<div class="mt-4 flex justify-center">
										<span class="text-sm text-gray-500">Showing 5 of {cars.length} vehicles</span>
									</div>
								{/if}
							{/if}
						</div>
					</div>

					<!-- Categories Section -->
					<div class="bg-white rounded-lg border border-gray-200 shadow-sm overflow-hidden">
						<div class="border-b border-gray-200 px-6 py-4 bg-gray-50 flex justify-between items-center">
							<h2 class="text-lg font-semibold text-gray-800">Vehicle Categories</h2>
							<button
								class="text-green-600 hover:text-green-800 text-sm font-medium flex items-center"
								on:click={() => {
                  showAddCategory = true;
                  showAddCar = false;
                  showAddRental = false;
                  showEditCar = false;
                  showEditCategory = false;
                  showEditUser = false;
                }}
							>
								<svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4 mr-1" fill="none" viewBox="0 0 24 24" stroke="currentColor">
									<path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6" />
								</svg>
								Add Category
							</button>
						</div>
						<div class="p-6">
							{#if categoriesLoading}
								<div class="flex justify-center items-center h-32">
									<div class="w-8 h-8 border-4 border-gray-300 border-t-green-600 rounded-full animate-spin"></div>
								</div>
							{:else if categoriesError}
								<div class="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded mb-4">
									<p>{categoriesError}</p>
								</div>
							{:else if categories.length === 0}
								<div class="bg-gray-50 border border-gray-200 text-gray-600 px-4 py-5 rounded text-center">
									<p>No vehicle categories defined.</p>
									<button
										class="mt-3 text-green-600 hover:text-green-800 text-sm font-medium"
										on:click={() => {
                      showAddCategory = true;
                      showAddCar = false;
                      showAddRental = false;
                      showEditCar = false;
                      showEditCategory = false;
                      showEditUser = false;
                    }}
									>
										Create your first category
									</button>
								</div>
							{:else}
								<div class="grid grid-cols-1 gap-4">
									{#each categories as category}
										<div class="border rounded-lg p-4 hover:bg-gray-50 shadow-sm">
											<div class="flex justify-between">
												<h3 class="font-semibold text-lg text-gray-800">{category.name}</h3>
												<span class="text-green-700 font-semibold">${category.dailyRate}/day</span>
											</div>

											<div class="mt-2 text-sm text-gray-600">
												{#if category.carCount > 0}
													{category.carCount} vehicle{category.carCount > 1 ? 's' : ''} in this category
												{:else}
													<span class="text-gray-500 italic">No vehicles assigned</span>
												{/if}
											</div>

											<div class="mt-3 flex space-x-3">
												<button
													class="text-blue-600 hover:text-blue-900 text-sm"
													on:click={() => openEditCategory(category)}
												>
													Edit
												</button>
											</div>
										</div>
									{/each}
								</div>
							{/if}
						</div>
					</div>
				</div>

				<!-- Rentals Section -->
				<div id="rentals" class="bg-white rounded-lg border border-gray-200 shadow-sm overflow-hidden mb-8">
					<div class="border-b border-gray-200 px-6 py-4 bg-gray-50 flex justify-between items-center">
						<h2 class="text-lg font-semibold text-gray-800">Rental Management</h2>
						<button
							class="text-purple-600 hover:text-purple-800 text-sm font-medium flex items-center"
							on:click={() => {
                showAddRental = true;
                showAddCar = false;
                showAddCategory = false;
                showEditCar = false;
                showEditCategory = false;
                showEditUser = false;
              }}
						>
							<svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4 mr-1" fill="none" viewBox="0 0 24 24" stroke="currentColor">
								<path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6" />
							</svg>
							Create Rental
						</button>
					</div>
					<div class="p-6">
						{#if rentalsLoading}
							<div class="flex justify-center items-center h-32">
								<div class="w-8 h-8 border-4 border-gray-300 border-t-purple-600 rounded-full animate-spin"></div>
							</div>
						{:else if rentalsError}
							<div class="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded mb-4">
								<p>{rentalsError}</p>
							</div>
						{:else if rentals.length === 0}
							<div class="bg-gray-50 border border-gray-200 text-gray-600 px-4 py-5 rounded text-center">
								<p>No rental records found.</p>
								<button
									class="mt-3 text-purple-600 hover:text-purple-800 text-sm font-medium"
									on:click={() => {
                    showAddRental = true;
                    showAddCar = false;
                    showAddCategory = false;
                    showEditCar = false;
                    showEditCategory = false;
                    showEditUser = false;
                  }}
								>
									Create your first rental
								</button>
							</div>
						{:else}
							<div class="overflow-x-auto">
								<table class="min-w-full divide-y divide-gray-200">
									<thead>
									<tr>
										<th class="px-4 py-3 bg-gray-50 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">ID</th>
										<th class="px-4 py-3 bg-gray-50 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Renter</th>
										<th class="px-4 py-3 bg-gray-50 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Created</th>
										<th class="px-4 py-3 bg-gray-50 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Status</th>
										<th class="px-4 py-3 bg-gray-50 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Amount</th>
										<th class="px-4 py-3 bg-gray-50 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Actions</th>
									</tr>
									</thead>
									<tbody class="bg-white divide-y divide-gray-200">
									{#each rentals as rental}
										<tr class="hover:bg-gray-50">
											<td class="px-4 py-3 whitespace-nowrap text-sm font-medium text-gray-900">
												{rental.id ? rental.id.substring(0, 8) + '...' : 'N/A'}
											</td>
											<td class="px-4 py-3 whitespace-nowrap">
												{#if rental.renter}
													<div class="text-sm font-medium text-gray-900">{rental.renter.name}</div>
													<div class="text-xs text-gray-500">{rental.renter.email}</div>
												{:else}
													<div class="text-sm text-gray-500">Unknown user</div>
												{/if}
											</td>
											<td class="px-4 py-3 whitespace-nowrap text-sm text-gray-500">
												{rental.applicationTime ? formatDate(rental.applicationTime) : 'N/A'}
											</td>
											<td class="px-4 py-3 whitespace-nowrap">
                          <span class={`px-2 py-1 inline-flex text-xs leading-5 font-medium rounded-full
                            ${rental.status === 0 ? 'bg-yellow-100 text-yellow-800' :
                              rental.status === 1 ? 'bg-green-100 text-green-800' :
                              rental.status === 2 ? 'bg-blue-100 text-blue-800' :
                              rental.status === 3 ? 'bg-red-100 text-red-800' : 'bg-gray-100 text-gray-800'}`}>
                            {getRentalStatusText(rental.status)}
                          </span>
											</td>
											<td class="px-4 py-3 whitespace-nowrap text-sm font-medium text-gray-900">
												${rental.owed ?? 0}
											</td>
											<td class="px-4 py-3 whitespace-nowrap text-sm text-gray-500">
												<button
													class="text-indigo-600 hover:text-indigo-900"
													on:click={() => updateRentalStatus(rental.id, getNextRentalStatus(rental.status))}
												>
													{getRentalActionText(rental.status)}
												</button>
											</td>
										</tr>
									{/each}
									</tbody>
								</table>
							</div>
						{/if}
					</div>
				</div>

				<!-- Users Section -->
				<div id="users" class="bg-white rounded-lg border border-gray-200 shadow-sm overflow-hidden">
					<div class="border-b border-gray-200 px-6 py-4 bg-gray-50">
						<h2 class="text-lg font-semibold text-gray-800">User Management</h2>
					</div>
					<div class="p-6">
						{#if usersLoading}
							<div class="flex justify-center items-center h-32">
								<div class="w-8 h-8 border-4 border-gray-300 border-t-blue-600 rounded-full animate-spin"></div>
							</div>
						{:else if usersError}
							<div class="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded mb-4">
								<p>{usersError}</p>
							</div>
						{:else if users.length === 0}
							<div class="bg-gray-50 border border-gray-200 text-gray-600 px-4 py-5 rounded text-center">
								<p>No users found in the system.</p>
							</div>
						{:else}
							<div class="overflow-x-auto">
								<table class="min-w-full divide-y divide-gray-200">
									<thead>
									<tr>
										<th class="px-4 py-3 bg-gray-50 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Name</th>
										<th class="px-4 py-3 bg-gray-50 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Email</th>
										<th class="px-4 py-3 bg-gray-50 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Contact</th>
										<th class="px-4 py-3 bg-gray-50 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Role</th>
										<th class="px-4 py-3 bg-gray-50 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Actions</th>
									</tr>
									</thead>
									<tbody class="bg-white divide-y divide-gray-200">
									{#each users as user}
										<tr class="hover:bg-gray-50">
											<td class="px-4 py-3 whitespace-nowrap">
												<div class="text-sm font-medium text-gray-900">{user.name}</div>
											</td>
											<td class="px-4 py-3 whitespace-nowrap text-sm text-gray-900">{user.email}</td>
											<td class="px-4 py-3 whitespace-nowrap">
												<div class="text-sm text-gray-900">{user.phoneNumber}</div>
												<div class="text-xs text-gray-500 truncate max-w-xs">{user.address}</div>
											</td>
											<td class="px-4 py-3 whitespace-nowrap">
												<div class="text-sm text-gray-900">
													{getUserRoleName(user.accesLevel)}
												</div>
												<span class={`inline-flex px-2 text-xs leading-5 font-medium rounded-full ${user.enabled ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'}`}>
                            {user.enabled !== false ? 'Active' : 'Disabled'}
                          </span>
											</td>
											<td class="px-4 py-3 whitespace-nowrap text-sm flex space-x-3">
												<button
													class="text-blue-600 hover:text-blue-900"
													on:click={() => openEditUser(user)}
												>
													Edit
												</button>

												<button
													class="text-red-600 hover:text-red-900"
													on:click={() => confirmDelete('user', user)}
												>
													Delete
												</button>
											</td>
										</tr>
									{/each}
									</tbody>
								</table>
							</div>
						{/if}
					</div>
				</div>
			</main>
		</div>
	</div>
{/if}