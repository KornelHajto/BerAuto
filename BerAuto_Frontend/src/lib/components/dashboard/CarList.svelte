<!-- src/lib/components/dashboard/CarList.svelte -->
<script lang="ts">
	import { createEventDispatcher } from 'svelte';
	import { CarStatus } from '$lib/types';

	export let cars = [];
	export let categories = [];
	export let loading = false;
	export let error = null;

	const dispatch = createEventDispatcher();

	function getCategoryName(categoryId) {
		const category = categories.find(c => c.id === categoryId);
		return category ? category.name : 'Uncategorized';
	}

	function editCar(car) {
		dispatch('edit', car);
	}

	function deleteCar(car) {
		dispatch('delete', car);
	}

	function getStatusLabel(status) {
		return status === CarStatus.Available ? 'Available' : 'Unavailable';
	}

	function getStatusColor(status) {
		return status === CarStatus.Available ?
			'bg-green-100 text-green-800' :
			'bg-red-100 text-red-800';
	}
</script>

<div class="bg-white rounded-lg shadow overflow-hidden">
	<div class="flex justify-between items-center p-6 border-b">
		<h3 class="text-lg font-medium text-gray-900">Cars</h3>
		<button
			on:click={() => dispatch('add')}
			class="px-4 py-2 text-sm font-medium text-white bg-blue-600 rounded-md hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
		>
			Add Car
		</button>
	</div>

	{#if loading}
		<div class="p-6 text-center text-gray-500">Loading cars...</div>
	{:else if error}
		<div class="p-6 text-center text-red-500">{error}</div>
	{:else if cars.length === 0}
		<div class="p-6 text-center text-gray-500">No cars found</div>
	{:else}
		<div class="overflow-x-auto">
			<table class="min-w-full divide-y divide-gray-200">
				<thead class="bg-gray-50">
				<tr>
					<th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
						Plate Number
					</th>
					<th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
						Type
					</th>
					<th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
						Category
					</th>
					<th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
						Odometer
					</th>
					<th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
						Status
					</th>
					<th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
						Actions
					</th>
				</tr>
				</thead>
				<tbody class="bg-white divide-y divide-gray-200">
				{#each cars as car}
					<tr>
						<td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
							{car.plateNumber}
						</td>
						<td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
							{car.type}
						</td>
						<td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
							{getCategoryName(car.categoryId)}
						</td>
						<td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
							{car.odometer.toLocaleString()} km
						</td>
						<td class="px-6 py-4 whitespace-nowrap">
                                <span class={`px-2 inline-flex text-xs leading-5 font-semibold rounded-full ${getStatusColor(car.status)}`}>
                                    {getStatusLabel(car.status)}
                                </span>
						</td>
						<td class="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
							<button
								on:click={() => editCar(car)}
								class="text-blue-600 hover:text-blue-900 mr-3"
							>
								Edit
							</button>
							<button
								on:click={() => deleteCar(car)}
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