import axios from "axios";

// Create a recursive function that detects and handles circular references
const safeStringify = (obj) => {
  const seen = new WeakSet();
  return JSON.stringify(obj, (key, value) => {
    // Skip CarRents, Car, and other potentially circular properties
    if (key === 'carRents' || key === 'car' || key === 'cars' || key === 'category') {
      // Return a simplified version without the circular reference
      if (typeof value === 'object' && value !== null) {
        // For arrays, return only their IDs
        if (Array.isArray(value)) {
          return value.map(item => (item && item.id) ? { id: item.id } : item);
        }
        // For objects, return only the ID
        else if (value.id) {
          return { id: value.id };
        }
      }
    }
    
    // Handle other circular references
    if (typeof value === 'object' && value !== null) {
      if (seen.has(value)) {
        return undefined; // Avoid circular reference
      }
      seen.add(value);
    }
    return value;
  });
};

const apiClient = axios.create({
  baseURL: "http://localhost:5291",
  headers: {
    "Content-Type": "application/json",
  },
});

// Override the default JSON.stringify in axios
apiClient.defaults.transformRequest = [
  (data) => {
    if (data === undefined) return data;
    try {
      return safeStringify(data);
    } catch (e) {
      console.error("Error in transform request:", e);
      return JSON.stringify(data);
    }
  }
];

// Attach Authorization header with token
apiClient.interceptors.request.use((config) => {
  const token = localStorage.getItem("accessToken");
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// Handle 401 (Unauthorized) globally and also handle circular references
apiClient.interceptors.response.use(
  (response) => response,
  async (error) => {
    // Handle circular references error (status 400)
    if (error.response && error.response.status === 400 && 
        error.response.data?.message?.includes("Self referencing loop")) {
      console.warn("Circular reference detected in API response");
      
      // Instead of failing, we can return a modified version of the response
      // with a flag indicating it had a circular reference
      return {
        ...error.response,
        data: {
          ...error.response.data,
          data: [], // Empty data array instead of null
          hadCircularReference: true
        },
        status: 200 // Override the status to treat it as "OK" for our code
      };
    }
    
    // Handle unauthorized as before
    const originalRequest = error.config;
    if (error.response.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true;
      try {
        const refreshToken = localStorage.getItem("refreshToken");
        const res = await apiClient.post("/api/Auth/refresh", {
          refreshToken,
        });
        localStorage.setItem("accessToken", res.data.data.accessToken);
        localStorage.setItem("refreshToken", res.data.data.refreshToken);
        originalRequest.headers.Authorization = `Bearer ${res.data.data.accessToken}`;
        return apiClient(originalRequest);
      } catch (err) {
        localStorage.clear();
        window.location.href = "/login";
      }
    }
    return Promise.reject(error);
  }
);

export default apiClient;