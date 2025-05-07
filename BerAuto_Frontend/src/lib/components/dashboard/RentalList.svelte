<!-- src/lib/components/dashboard/RentalList.svelte -->
<script lang="ts">
	import { createEventDispatcher } from 'svelte';
	import { RentStatus } from '$lib/types';

	export let rentals = [];
	export let cars = [];
	export let users = [];
	export let loading = false;
	export let error = null;

	const dispatch = createEventDispatcher();

	function getUserName(userId) {
		const user = users.find(u => u.id === userId);
		return user ? user.name : 'Unknown User';
	}

	function getCarDetails(carId) {
		const car = cars.find(c => c.id === carId);
		return car ? `${car.type} (${car.plateNumber})` : 'Unknown Car';
	}

	function formatDate(dateString) {
		if (!dateString) return '';
		const date = new Date(dateString);
		return date.toLocaleDateString();
	}

	function getRentStatusName(status) {
		switch (status) {
			case RentStatus.Request:
				return 'Request';
			case RentStatus.InProcess:
				return 'In Process';
			case RentStatus.Returned:
				return 'Returned';
			case RentStatus.Cancelled:
				return 'Cancelled';
			default:
				return 'Unknown';
		}
	}

	function getStatusColor(status) {
		switch (status) {
			case RentStatus.Request:
				return 'bg-yellow-100 text-yellow-800';
			case RentStatus.InProcess:
				return 'bg-green-100 text-green-800';
			case RentStatus.Returned:
				return 'bg-blue-100 text-blue-800';
			case RentStatus.Cancelled:
				return 'bg-red-100 text-red-800';
			default:
				return 'bg-gray-100 text-gray-800';
		}
	}
</script>

<div class="bg-white rounded-lg shadow overflow-hidden">
	<div class="flex justify-between items-center p-6 border-b">
		<h3 class="text-lg font-medium text-gray-900">Rentals</h3>
		<button
			on:click={() => dispatch('add')}
			class="px-4 py-2 text-sm font-medium text-white bg-blue-600 rounded-md hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
		>
			Add Rental
		</button>
	</div>

	{#if loading}
		<div class="p-6 text-center text-gray-500">Loading rentals...</div>
	{:else if error}
		<div class="p-6 text-center text-red-500">{error}</div>
	{:else if rentals.length === 0}
		<div class="p-6 text-center text-gray-500">No rentals found</div>
	{:else}
		<div class="overflow-x-auto">
			<table class="min-w-full divide-y divide-gray-200">
				<thead class="bg-gray-50">
				<tr>
					<th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
						Renter
					</th>
					<th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
						Car
					</th>
					<th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
						Start Date
					</th>
					<th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
						End Date
					</th>
					<th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
						Status
					</th>
					<th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
						Actions
					</th>
				</tr>
				</thead>
				<tbody class="bg-white divide-y divide-gray-200">
				{#each rentals as rental}
					<tr>
						<td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
							{getUserName(rental.renterId)}
						</td>
						<td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
							{getCarDetails(rental.carId)}
						</td>
						<td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
							{formatDate(rental.startDate)}
						</td>
						<td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
							{formatDate(rental.endDate)}
						</td>
						<td class="px-6 py-4 whitespace-nowrap">
                                <span class={`px-2 inline-flex text-xs leading-5 font-semibold rounded-full ${getStatusColor(rental.status)}`}>
                                    {getRentStatusName(rental.status)}
                                </span>
						</td>
						<td class="px-6 py-4 whitespace-nowrap text-sm font-medium">
							{#if rental.status === RentStatus.Request || rental.status === RentStatus.InProcess}
								<button
									on:click={() => dispatch('cancel', rental)}
									class="text-red-600 hover:text-red-900 mr-3"
								>
									Cancel
								</button>
							{/if}

							{#if rental.status === RentStatus.InProcess}
								<button
									on:click={() => dispatch('return', rental)}
									class="text-green-600 hover:text-green-900"
								>
									Return
								</button>
							{/if}

							{#if rental.status === RentStatus.Request}
								<button
									on:click={() => dispatch('process', rental)}
									class="text-blue-600 hover:text-blue-900"
								>
									Process
								</button>
							{/if}
						</td>
					</tr>
				{/each}
				</tbody>
			</table>
		</div>
	{/if}
</div>