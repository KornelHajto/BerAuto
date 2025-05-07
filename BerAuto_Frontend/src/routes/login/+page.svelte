<!-- src/routes/+page.svelte -->
<script lang="ts">
	import { onMount } from 'svelte';
	import { goto } from '$app/navigation';
	import { v4 as uuidv4 } from 'uuid';

	// Import services
	import {
		authService,
		carService,
		categoryService,
		rentalService,
		userService
	} from '$lib/services/api';

	// Import components
	import DashboardSummary from '$lib/components/dashboard/DashboardSummary.svelte';
	import CarList from '$lib/components/dashboard/CarList.svelte';
	import CategoryList from '$lib/components/dashboard/CategoryList.svelte';
	import RentalList from '$lib/components/dashboard/RentalList.svelte';
	import UserList from '$lib/components/dashboard/UserList.svelte';
	import CarForm from '$lib/components/cars/CarForm.svelte';
	import CategoryForm from '$lib/components/categories/CategoryForm.svelte';
	import RentalForm from '$lib/components/rentals/RentalForm.svelte';
	import UserForm from '$lib/components/users/UserForm.svelte';
	import DeleteConfirmationModal from '$lib/components/shared/DeleteConfirmationModal.svelte';

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

	// Redirect to login if not authenticated
	async function redirectIfNotAuthenticated() {
		const isAuth = await authService.checkAuthentication();
		if (!isAuth) {
			goto('/login');
		}
		isAuthenticated = isAuth;
		return isAuth;
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

	// Fetch cars
	async function fetchCars() {
		carsLoading = true;
		try {
			const result = await carService.fetchCars();
			cars = result.data;
			carsError = result.error;
		} finally {
			carsLoading = false;
		}
	}

	// Fetch categories with cars
	async function fetchCategories() {
		categoriesLoading = true;
		try {
			const result = await categoryService.fetchCategories();
			categories = result.data;

			// Map car counts to categories
			categories = categories.map(category => {
				const carsInCategory = cars.filter(car => car.categoryId === category.id);
				return {
					...category,
					carCount: carsInCategory.length
				};
			});

			categoriesError = result.error;
		} finally {
			categoriesLoading = false;
		}
	}

	// Fetch rentals
	async function fetchRentals() {
		rentalsLoading = true;
		try {
			const result = await rentalService.fetchRentals();
			rentals = result.data;
			rentalsError = result.error;
		} finally {
			rentalsLoading = false;
		}
	}

	// Fetch users
	async function fetchUsers() {
		usersLoading = true;
		try {
			const result = await userService.fetchUsers();
			users = result.data;
			usersError = result.error;
		} finally {
			usersLoading = false;
		}
	}

	// Handle deleting a car
	async function handleDeleteCar(car) {
		if (await carService.deleteCar(car.id)) {
			await fetchCars();
			await fetchCategories();
		}
		showDeleteConfirmation = false;
		itemToDelete = null;
	}

	// Handle deleting a user
	async function handleDeleteUser(user) {
		if (await userService.deleteUser(user.id)) {
			await fetchUsers();
		}
		showDeleteConfirmation = false;
		itemToDelete = null;
	}

	// When component mounts, check auth and load data
	onMount(async () => {
		const isAuth = await redirectIfNotAuthenticated();
		if (isAuth) {
			fetchAllData();
		}
	});

	// Show delete confirmation modal
	function confirmDelete(item, type) {
		itemToDelete = item;
		deleteType = type;
		showDeleteConfirmation = true;
	}

	// Handle delete confirmation
	function handleDeleteConfirmation(event) {
		const item = event.detail;
		if (deleteType === 'car') {
			handleDeleteCar(item);
		} else if (deleteType === 'user') {
			handleDeleteUser(item);
		}
	}
</script>

<div class="min-h-screen bg-gray-100">
	<div class="py-10">
		<header class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
			<h1 class="text-3xl font-bold leading-tight text-gray-900">Dashboard</h1>
		</header>
		<main class="max-w-7xl mx-auto sm:px-6 lg:px-8">
			<div class="px-4 py-8 sm:px-0">
				<!-- Dashboard summary cards -->
				<DashboardSummary
					{cars}
					{categories}
					{rentals}
					{users}
					loading={dashboardLoading}
				/>

				<!-- Delete confirmation modal -->
				<DeleteConfirmationModal
					item={itemToDelete}
					type={deleteType}
					on:confirm={handleDeleteConfirmation}
					on:cancel={() => { showDeleteConfirmation = false; itemToDelete = null; }}
				/>

				<div class="space-y-8">
					<!-- Car section -->
					<div>
						{#if showAddCar || showEditCar}
							<CarForm
								car={showEditCar ? carToEdit : undefined}
								categories={categories}
								editMode={showEditCar}
								on:success={() => {
                                    fetchCars();
                                    fetchCategories();
                                    showAddCar = false;
                                    showEditCar = false;
                                    carToEdit = null;
                                }}
								on:cancel={() => {
                                    showAddCar = false;
                                    showEditCar = false;
                                    carToEdit = null;
                                }}
							/>
						{:else}
							<CarList
								{cars}
								{categories}
								loading={carsLoading}
								error={carsError}
								on:add={() => showAddCar = true}
								on:edit={(e) => {
                                    carToEdit = e.detail;
                                    showEditCar = true;
                                }}
								on:delete={(e) => confirmDelete(e.detail, 'car')}
							/>
						{/if}
					</div>

					<!-- Category section -->
					<div>
						{#if showAddCategory || showEditCategory}
							<CategoryForm
								category={showEditCategory ? categoryToEdit : undefined}
								editMode={showEditCategory}
								on:success={() => {
                                    fetchCategories();
                                    showAddCategory = false;
                                    showEditCategory = false;
                                    categoryToEdit = null;
                                }}
								on:cancel={() => {
                                    showAddCategory = false;
                                    showEditCategory = false;
                                    categoryToEdit = null;
                                }}
							/>
						{:else}
							<CategoryList
								{categories}
								loading={categoriesLoading}
								error={categoriesError}
								on:add={() => showAddCategory = true}
								on:edit={(e) => {
                                    categoryToEdit = e.detail;
                                    showEditCategory = true;
                                }}
								on:delete={(e) => confirmDelete(e.detail, 'category')}
							/>
						{/if}
					</div>

					<!-- Rental section -->
					<div>
						{#if showAddRental}
							<RentalForm
								{cars}
								{users}
								on:success={() => {
                                    fetchRentals();
                                    fetchCars(); // Refresh cars to update availability
                                    showAddRental = false;
                                }}
								on:cancel={() => {
                                    showAddRental = false;
                                }}
							/>
						{:else}
							<RentalList
								{rentals}
								{cars}
								{users}
								loading={rentalsLoading}
								error={rentalsError}
								on:add={() => showAddRental = true}
							/>
						{/if}
					</div>

					<!-- Users section -->
					<div>
						{#if showEditUser}
							<UserForm
								user={userToEdit}
								editMode={true}
								on:success={() => {
                                    fetchUsers();
                                    showEditUser = false;
                                    userToEdit = null;
                                }}
								on:cancel={() => {
                                    showEditUser = false;
                                    userToEdit = null;
                                }}
							/>
						{:else}
							<UserList
								{users}
								loading={usersLoading}
								error={usersError}
								on:edit={(e) => {
                                    userToEdit = e.detail;
                                    showEditUser = true;
                                }}
								on:delete={(e) => confirmDelete(e.detail, 'user')}
							/>
						{/if}
					</div>
				</div>
			</div>
		</main>
	</div>
</div>