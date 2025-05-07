<!-- src/lib/components/dashboard/CategoryList.svelte -->
<script lang="ts">
	import { createEventDispatcher } from 'svelte';

	export let categories = [];
	export let loading = false;
	export let error = null;

	const dispatch = createEventDispatcher();

	function editCategory(category) {
		dispatch('edit', category);
	}

	function deleteCategory(category) {
		dispatch('delete', category);
	}
</script>

<div class="bg-white rounded-lg shadow overflow-hidden">
	<div class="flex justify-between items-center p-6 border-b">
		<h3 class="text-lg font-medium text-gray-900">Categories</h3>
		<button
			on:click={() => dispatch('add')}
			class="px-4 py-2 text-sm font-medium text-white bg-blue-600 rounded-md hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
		>
			Add Category
		</button>
	</div>

	{#if loading}
		<div class="p-6 text-center text-gray-500">Loading categories...</div>
	{:else if error}
		<div class="p-6 text-center text-red-500">{error}</div>
	{:else if categories.length === 0}
		<div class="p-6 text-center text-gray-500">No categories found</div>
	{:else}
		<div class="overflow-x-auto">
			<table class="min-w-full divide-y divide-gray-200">
				<thead class="bg-gray-50">
				<tr>
					<th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
						Category Name
					</th>
					<th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
						Daily Rate
					</th>
					<th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
						Cars
					</th>
					<th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
						Actions
					</th>
				</tr>
				</thead>
				<tbody class="bg-white divide-y divide-gray-200">
				{#each categories as category}
					<tr>
						<td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
							{category.name}
						</td>
						<td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
							${category.dailyRate.toFixed(2)}
						</td>
						<td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
							{category.carCount || 0}
						</td>
						<td class="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
							<button
								on:click={() => editCategory(category)}
								class="text-blue-600 hover:text-blue-900 mr-3"
							>
								Edit
							</button>
							<button
								on:click={() => deleteCategory(category)}
								class="text-red-600 hover:text-red-900"
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