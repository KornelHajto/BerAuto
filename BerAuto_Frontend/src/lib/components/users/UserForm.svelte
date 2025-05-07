<!-- src/lib/components/users/UserForm.svelte -->
<script lang="ts">
	import { createEventDispatcher } from 'svelte';
	import { userService } from '$lib/services/api';
	import { UserType } from '$lib/types';

	export let user = {
		id: null,
		name: "",
		email: "",
		address: "",
		phoneNumber: "",
		description: "",
		userType: UserType.User
	};
	export let editMode = false;

	let error = null;
	let success = null;
	const dispatch = createEventDispatcher();

	// User type options for dropdown
	const userTypeOptions = [
		{ value: UserType.Administrator, label: 'Administrator' },
		{ value: UserType.Worker, label: 'Worker' },
		{ value: UserType.User, label: 'User' },
		{ value: UserType.System, label: 'System' }
	];

	async function handleSubmit() {
		error = null;
		success = null;

		if (!user.name) {
			error = "Name is required";
			return;
		}

		if (!user.email) {
			error = "Email is required";
			return;
		}

		try {
			const result = await userService.updateUser(user);
			if (result) {
				success = `User "${user.name}" updated successfully!`;
				dispatch('success');
			} else {
				error = "Failed to update user. Please try again.";
			}
		} catch (err) {
			console.error('User update error:', err);
			error = "An unexpected error occurred. Please try again.";
		}
	}

	function cancel() {
		dispatch('cancel');
	}
</script>

<div class="bg-white p-6 rounded-lg shadow-md">
	<h3 class="text-xl font-semibold mb-4">Edit User</h3>

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
			<label class="block text-sm font-medium text-gray-700">Name</label>
			<input
				type="text"
				bind:value={user.name}
				class="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
			/>
		</div>

		<div>
			<label class="block text-sm font-medium text-gray-700">Email</label>
			<input
				type="email"
				bind:value={user.email}
				class="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
			/>
		</div>

		<div>
			<label class="block text-sm font-medium text-gray-700">Phone Number</label>
			<input
				type="text"
				bind:value={user.phoneNumber}
				class="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
			/>
		</div>

		<div>
			<label class="block text-sm font-medium text-gray-700">Address</label>
			<input
				type="text"
				bind:value={user.address}
				class="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
			/>
		</div>

		<div>
			<label class="block text-sm font-medium text-gray-700">User Type</label>
			<select
				bind:value={user.userType}
				class="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
			>
				{#each userTypeOptions as option}
					<option value={option.value}>{option.label}</option>
				{/each}
			</select>
		</div>

		<div>
			<label class="block text-sm font-medium text-gray-700">Description</label>
			<textarea
				bind:value={user.description}
				class="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
			></textarea>
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
				Update User
			</button>
		</div>
	</form>
</div>