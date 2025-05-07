<!-- src/lib/components/rentals/RentalForm.svelte -->
<script lang="ts">
	import { createEventDispatcher } from 'svelte';
	import { rentalService } from '$lib/services/api';
	import { RentStatus, CarStatus } from '$lib/types';

	export let cars = [];
	export let users = [];

	let rental = {
		renterId: "",
		carId: "",
		startDate: "",
		endDate: "",
		status: RentStatus.Request
	};

	let error = null;
	let success = null;
	const dispatch = createEventDispatcher();

	async function handleSubmit() {
		error = null;
		success = null;

		if (!rental.renterId) {
			error = "Renter is required";
			return;
		}

		if (!rental.carId) {
			error = "Car is required";
			return;
		}

		if (!rental.startDate || !rental.endDate) {
			error = "Start and end dates are required";
			return;
		}

		// Validate dates
		const startDate = new Date(rental.startDate);
		const endDate = new Date(rental.endDate);

		if (startDate >= endDate) {
			error = "End date must be after start date";
			return;
		}

		try {
			const result = await rentalService.createRental(rental);
			if (result.success) {
				success = "Rental created successfully!";
				rental = {
					renterId: "",
					carId: "",
					startDate: "",
					endDate: "",
					status: RentStatus.Request
				};
				dispatch('success');
			} else {
				error = result.error;
			}
		} catch (err) {
			console.error('Rental creation error:', err);
			error = "An unexpected error occurred. Please try again.";
		}
	}

	function cancel() {
		dispatch('cancel');
	}
</script>

<div class="bg-white p-6 rounded-lg shadow-md">
	<h3 class="text-xl font-semibold mb-4">Add New Rental</h3>

	{#if error}
		<div class="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded mb-4">
			{error}
		</div>
	{/if}

	{#if success}
		<div class="bg-green-100 border border-green-400 text-green-700 px-4 py-3 rounded mb-4">
			{success}
		</div>
	{/if}

	<form on:submit|preventDefault={handleSubmit} class="space-y-4">
		<div>
			<label class="block text-sm font-medium text-gray-700">Renter</label>
			<select
				bind:value={rental.renterId}
				class="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
			>
				<option value="">Select a renter</option>
				{#each users.filter(user => user.userType === 3) as user}
					<option value={user.id}>{user.name}</option>
				{/each}
			</select>
		</div>

		<div>
			<label class="block text-sm font-medium text-gray-700">Car</label>
			<select
				bind:value={rental.carId}
				class="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
			>
				<option value="">Select a car</option>
				{#each cars.filter(car => car.status === CarStatus.Available) as car}
					<option value={car.id}>{car.type} - {car.plateNumber}</option>
				{/each}
			</select>
		</div>

		<div>
			<label class="block text-sm font-medium text-gray-700">Start Date</label>
			<input
				type="date"
				bind:value={rental.startDate}
				class="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
			/>
		</div>

		<div>
			<label class="block text-sm font-medium text-gray-700">End Date</label>
			<input
				type="date"
				bind:value={rental.endDate}
				class="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
			/>
		</div>

		<div class="flex justify-end space-x-3">
			<button
				type="button"
				on:click={cancel}
				class="px-4 py-2 text-sm font-medium text-gray-700 bg-white border border-gray-300 rounded-md shadow-sm hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
			>
				Cancel
			</button>
			<button
				type="submit"
				class="px-4 py-2 text-sm font-medium text-white bg-blue-600 border border-transparent rounded-md shadow-sm hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
			>
				Create Rental
			</button>
		</div>
	</form>
</div>