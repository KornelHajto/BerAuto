<!-- src/lib/components/categories/CategoryForm.svelte -->
<script lang="ts">
	import { createEventDispatcher } from 'svelte';
	import { categoryService } from '$lib/services/api';

	export let category = {
		id: null,
		name: "",
		dailyRate: 0
	};
	export let editMode = false;

	let error = null;
	let success = null;
	const dispatch = createEventDispatcher();

	async function handleSubmit() {
		error = null;
		success = null;

		if (!category.name) {
			error = "Category name is required";
			return;
		}

		if (category.dailyRate <= 0) {
			error = "Daily rate must be greater than 0";
			return;
		}

		try {
			if (editMode) {
				await categoryService.updateCategory(category);
				success = `Category "${category.name}" updated successfully!`;
			} else {
				const result = await categoryService.createCategory(category);
				if (result.success) {
					success = `Category "${category.name}" created successfully!`;
					category = {
						id: null,
						name: "",
						dailyRate: 0
					};
				} else {
					error = result.error;
				}
			}

			dispatch('success');
		} catch (err) {
			console.error('Category operation error:', err);
			error = "An unexpected error occurred. Please try again.";
		}
	}

	function cancel() {
		dispatch('cancel');
	}
</script>

<div class="bg-white p-6 rounded-lg shadow-md">
	<h3 class="text-xl font-semibold mb-4">{editMode ? 'Edit Category' : 'Add New Category'}</h3>

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
			<label class="block text-sm font-medium text-gray-700">Category Name</label>
			<input
				type="text"
				bind:value={category.name}
				class="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
			/>
		</div>

		<div>
			<label class="block text-sm font-medium text-gray-700">Daily Rate</label>
			<input
				type="number"
				bind:value={category.dailyRate}
				step="0.01"
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
				{editMode ? 'Update Category' : 'Add Category'}
			</button>
		</div>
	</form>
</div>