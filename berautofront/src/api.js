import axios from "axios";

// Axios instance with custom configuration to handle circular references
const api = axios.create({
    baseURL: "http://localhost:5291", // Replace with your API base URL
    headers: {
        "Content-Type": "application/json",
    },
    // Try to handle circular references by modifying request/response
    transformRequest: [(data, headers) => {
        if (data) {
            try {
                return JSON.stringify(data, (key, value) => {
                    // Handle circular references by converting to a simple reference
                    if (key === 'carRents' || key === 'rents' || key === 'cars') {
                        return undefined; // Skip these properties to avoid circular references
                    }
                    return value;
                });
            } catch (e) {
                console.error('Error serializing request data:', e);
                return JSON.stringify(data);
            }
        }
        return data;
    }],
});

// Automatically attach authorization token to requests
api.interceptors.request.use((config) => {
    const token = localStorage.getItem("accessToken");
    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
});

export default api;