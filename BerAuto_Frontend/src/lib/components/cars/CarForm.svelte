<!-- src/lib/components/cars/CarForm.svelte -->
<script lang="ts">
	import { createEventDispatcher } from 'svelte';
	import { v4 as uuidv4 } from 'uuid';
	import { carService } from '$lib/services/api';
	import { CarStatus } from '$lib/types';

	export let categories = [];
	export let car = {
		id: "",
		plateNumber: "",
		type: "",
		odometer: 0,
		status: CarStatus.Available,
		categoryId: "",
		description: ""
	};
	export let editMode = false;

	let error = null;
	let success = null;
	const dispatch = createEventDispatcher();

	// Define status options with correct values
	const statusOptions = [
		{ value: CarStatus.Available, label: 'Available' },
		{ value: CarStatus.Unavailable, label: 'Unavailable' }
	];

	async function handleSubmit() {
		error = null;
		success = null;

		if (!car.plateNumber) {
			error = "Plate number is required";
			return;
		}

		if (!car.type) {
			error = "Car type is required";
			return;
		}

		if (car.odometer < 0) {
			error = "Odometer must be a positive number";
			return;
		}

		if (!car.categoryId) {
			error = "Category is required";
			return;
		}

		try {
			if (editMode) {
				await carService.updateCar(car);
				success = `Car "${car.type}" updated successfully!`;
			} else {
				// Generate a UUID if not provided
				if (!car.id) {
					car.id = uuidv4();
				}

				const result = await carService.createCar(car);
				if (result.success) {
					success = `Car "${car.type}" with plate "${car.plateNumber}" created successfully!`;
					car = {
						id: "",
						plateNumber: "",
						type: "",
						odometer: 0,
						status: CarStatus.Available,
						categoryId: "",
						description: ""
					};
				} else {
					error = result.error;
				}
			}

			dispatch('success');
		} catch (err) {
			console.error('Car operation error:', err);
			error = "An unexpected error occurred. Please try again.";
		}
	}

	function cancel() {
		dispatch('cancel');
	}
</script>

<div class="bg-white p-6 rounded-lg shadow-md">
	<h3 class="text-xl font-semibold mb-4">{editMode ? 'Edit Car' : 'Add New Car'}</h3>

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
			<label class="block text-sm font-medium text-gray-700">Plate Number</label>
			<input
				type="text"
				bind:value={car.plateNumber}
				class="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
			/>
		</div>

		<div>
			<label class="block text-sm font-medium text-gray-700">Car Type</label>
			<input
				type="text"
				bind:value={car.type}
				class="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
			/>
		</div>

		<div>
			<label class="block text-sm font-medium text-gray-700">Odometer (km)</label>
			<input
				type="number"
				bind:value={car.odometer}
				class="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
			/>
		</div>

		<div>
			<label class="block text-sm font-medium text-gray-700">Category</label>
			<select
				bind:value={car.categoryId}
				class="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
			>
				<option value="">Select a category</option>
				{#each categories as category}
					<option value={category.id}>{category.name}</option>
				{/each}
			</select>
		</div>

		<div>
			<label class="block text-sm font-medium text-gray-700">Status</label>
			<select
				bind:value={car.status}
				class="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
			>
				{#each statusOptions as option}
					<option value={option.value}>{option.label}</option>
				{/each}
			</select>
		</div>

		<div>
			<label class="block text-sm font-medium text-gray-700">Description</label>
			<textarea
				bind:value={car.description}
				class="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
				rows="3"
			></textarea>
		</div>

		<div class="flex justify-end space-x-3 pt-4">
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
				{editMode ? 'Update Car' : 'Add Car'}
			</button>
		</div>
	</form>
</div>