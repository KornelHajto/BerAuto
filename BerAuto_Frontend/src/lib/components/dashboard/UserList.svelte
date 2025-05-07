<!-- src/lib/components/dashboard/UserList.svelte -->
<script lang="ts">
	import { createEventDispatcher } from 'svelte';
	import { UserType } from '$lib/types';

	export let users = [];
	export let loading = false;
	export let error = null;

	const dispatch = createEventDispatcher();

	function editUser(user) {
		dispatch('edit', user);
	}

	function deleteUser(user) {
		dispatch('delete', user);
	}

	function getUserTypeName(userType) {
		switch (userType) {
			case UserType.Administrator:
				return 'Administrator';
			case UserType.Worker:
				return 'Worker';
			case UserType.User:
				return 'User';
			case UserType.System:
				return 'System';
			default:
				return 'Unknown';
		}
	}

	function getUserTypeColor(userType) {
		switch (userType) {
			case UserType.Administrator:
				return 'bg-purple-100 text-purple-800';
			case UserType.Worker:
				return 'bg-blue-100 text-blue-800';
			case UserType.User:
				return 'bg-green-100 text-green-800';
			case UserType.System:
				return 'bg-gray-100 text-gray-800';
			default:
				return 'bg-gray-100 text-gray-800';
		}
	}
</script>

<div class="bg-white rounded-lg shadow overflow-hidden">
	<div class="flex justify-between items-center p-6 border-b">
		<h3 class="text-lg font-medium text-gray-900">Users</h3>
	</div>

	{#if loading}
		<div class="p-6 text-center text-gray-500">Loading users...</div>
	{:else if error}
		<div class="p-6 text-center text-red-500">{error}</div>
	{:else if users.length === 0}
		<div class="p-6 text-center text-gray-500">No users found</div>
	{:else}
		<div class="overflow-x-auto">
			<table class="min-w-full divide-y divide-gray-200">
				<thead class="bg-gray-50">
				<tr>
					<th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
						Name
					</th>
					<th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
						Email
					</th>
					<th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
						Phone
					</th>
					<th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
						User Type
					</th>
					<th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
						Actions
					</th>
				</tr>
				</thead>
				<tbody class="bg-white divide-y divide-gray-200">
				{#each users as user}
					<tr>
						<td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
							{user.name}
						</td>
						<td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
							{user.email}
						</td>
						<td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
							{user.phoneNumber || 'N/A'}
						</td>
						<td class="px-6 py-4 whitespace-nowrap">
                                <span class={`px-2 inline-flex text-xs leading-5 font-semibold rounded-full ${getUserTypeColor(user.userType || UserType.User)}`}>
                                    {getUserTypeName(user.userType || UserType.User)}
                                </span>
						</td>
						<td class="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
							<button
								on:click={() => editUser(user)}
								class="text-blue-600 hover:text-blue-900 mr-3"
							>
								Edit
							</button>
							<button
								on:click={() => deleteUser(user)}
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