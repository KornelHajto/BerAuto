<!-- src/lib/components/dashboard/DashboardSummary.svelte -->
<script lang="ts">
	export let cars = [];
	export let categories = [];
	export let rentals = [];
	export let users = [];
	export let loading = false;

	$: availableCars = cars.filter(car => car.available).length;
	$: activeRentals = rentals.filter(rental => {
		const today = new Date();
		const startDate = new Date(rental.startDate);
		const endDate = new Date(rental.endDate);
		return startDate <= today && endDate >= today;
	}).length;
</script>

<div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
	<div class="bg-white rounded-lg shadow p-6">
		<div class="flex items-center">
			<div class="flex-shrink-0 bg-blue-500 rounded-md p-3">
				<svg class="h-6 w-6 text-white" fill="none" viewBox="0 0 24 24" stroke="currentColor">
					<path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2" />
				</svg>
			</div>
			<div class="ml-5">
				<p class="text-sm font-medium text-gray-500 truncate">Total Cars</p>
				<p class="mt-1 text-3xl font-semibold text-gray-900">
					{loading ? '-' : cars.length}
				</p>
			</div>
		</div>
	</div>

	<div class="bg-white rounded-lg shadow p-6">
		<div class="flex items-center">
			<div class="flex-shrink-0 bg-green-500 rounded-md p-3">
				<svg class="h-6 w-6 text-white" fill="none" viewBox="0 0 24 24" stroke="currentColor">
					<path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7" />
				</svg>
			</div>
			<div class="ml-5">
				<p class="text-sm font-medium text-gray-500 truncate">Available Cars</p>
				<p class="mt-1 text-3xl font-semibold text-gray-900">
					{loading ? '-' : availableCars}
				</p>
			</div>
		</div>
	</div>

	<div class="bg-white rounded-lg shadow p-6">
		<div class="flex items-center">
			<div class="flex-shrink-0 bg-purple-500 rounded-md p-3">
				<svg class="h-6 w-6 text-white" fill="none" viewBox="0 0 24 24" stroke="currentColor">
					<path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
				</svg>
			</div>
			<div class="ml-5">
				<p class="text-sm font-medium text-gray-500 truncate">Active Rentals</p>
				<p class="mt-1 text-3xl font-semibold text-gray-900">
					{loading ? '-' : activeRentals}
				</p>
			</div>
		</div>
	</div>

	<div class="bg-white rounded-lg shadow p-6">
		<div class="flex items-center">
			<div class="flex-shrink-0 bg-yellow-500 rounded-md p-3">
				<svg class="h-6 w-6 text-white" fill="none" viewBox="0 0 24 24" stroke="currentColor">
					<path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z" />
				</svg>
			</div>
			<div class="ml-5">
				<p class="text-sm font-medium text-gray-500 truncate">Total Users</p>
				<p class="mt-1 text-3xl font-semibold text-gray-900">
					{loading ? '-' : users.length}
				</p>
			</div>
		</div>
	</div>
</div>