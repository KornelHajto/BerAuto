<script lang="ts">
	import axios from 'axios';
	import { goto } from '$app/navigation';

	let name: string = '';
	let email: string = '';
	let password: string = '';
	let confirmPassword: string = '';
	let phoneNumber: string = '';
	let address: string = '';
	let description: string = '';

	function HandleRegister() {
		if (password !== confirmPassword) {
			console.error('Passwords do not match');
			return;
		}

		let response = axios.post('http://localhost:5291/api/Auth/register', {
			name: name,
			email: email,
			password: password,
			phoneNumber: phoneNumber,
			address: address,
			description: description
		});
		response.then((res) => {
			console.log(res.data);
			localStorage.setItem('token', res.data.token);
			localStorage.setItem('refreshtoken', res.data.refreshToken);
			goto('/login');
		}).catch((error) => {
			console.error('Error:', error);
		});
	}

	function goToLogin() {
		goto('/login');
	}
</script>

<div class="min-h-screen bg-gray-50 flex items-center justify-center">
	<div class="w-full max-w-lg p-10 space-y-8 bg-white rounded-xl shadow-lg border border-gray-200">
		<h2 class="text-3xl font-extrabold text-center text-gray-900">Register</h2>
		<p class="text-center text-gray-600">Create your account to get started</p>
		<div class="space-y-6">
			<input
				type="text"
				placeholder="Name"
				bind:value={name}
				class="w-full px-5 py-3 text-base border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition"
			/>
			<input
				type="email"
				placeholder="Email"
				bind:value={email}
				class="w-full px-5 py-3 text-base border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition"
			/>
			<input
				type="password"
				placeholder="Password"
				bind:value={password}
				class="w-full px-5 py-3 text-base border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition"
			/>
			<input
				type="password"
				placeholder="Confirm Password"
				bind:value={confirmPassword}
				class="w-full px-5 py-3 text-base border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition"
			/>
			<input
				type="text"
				placeholder="Phone Number"
				bind:value={phoneNumber}
				class="w-full px-5 py-3 text-base border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition"
			/>
			<input
				type="text"
				placeholder="Address"
				bind:value={address}
				class="w-full px-5 py-3 text-base border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition"
			/>
			<textarea
				placeholder="Description"
				bind:value={description}
				class="w-full px-5 py-3 text-base border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition"
			></textarea>
			<button
				on:click={HandleRegister}
				class="w-full px-5 py-3 text-base font-medium text-white bg-blue-600 rounded-lg hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 transition"
			>
				Register
			</button>
			<div
				class="text-center text-sm text-blue-600 cursor-pointer hover:underline"
				on:click={goToLogin}
			>
				Already have an account? <span class="font-medium">Login here</span>
			</div>
		</div>
	</div>
</div>