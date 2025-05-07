<!-- src/lib/components/shared/DeleteConfirmationModal.svelte -->
<script lang="ts">
	import { createEventDispatcher } from 'svelte';

	export let item = null;
	export let type = '';

	const dispatch = createEventDispatcher();

	function confirm() {
		dispatch('confirm', item);
	}

	function cancel() {
		dispatch('cancel');
	}

	function getTypeName() {
		switch (type) {
			case 'car': return 'Car';
			case 'category': return 'Category';
			case 'user': return 'User';
			default: return 'Item';
		}
	}

	function getItemName() {
		if (!item) return '';

		switch (type) {
			case 'car': return `${item.type} (${item.plateNumber})`;
			case 'category': return item.name;
			case 'user': return item.name;
			default: return 'this item';
		}
	}
</script>

{#if item}
	<div class="fixed inset-0 bg-gray-600 bg-opacity-50 flex items-center justify-center z-50">
		<div class="bg-white rounded-lg shadow-xl p-6 max-w-md w-full">
			<h3 class="text-lg font-medium text-gray-900 mb-4">Confirm Deletion</h3>
			<p class="text-sm text-gray-500 mb-6">
				Are you sure you want to delete the {getTypeName().toLowerCase()} <span class="font-semibold">{getItemName()}</span>?
				This action cannot be undone.
			</p>
			<div class="flex justify-end space-x-3">
				<button
					on:click={cancel}
					class="px-4 py-2 text-sm font-medium text-gray-700 bg-white border border-gray-300 rounded-md shadow-sm hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
				>
					Cancel
				</button>
				<button
					on:click={confirm}
					class="px-4 py-2 text-sm font-medium text-white bg-red-600 border border-transparent rounded-md shadow-sm hover:bg-red-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-red-500"
				>
					Delete
				</button>
			</div>
		</div>
	</div>
{/if}