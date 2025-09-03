import axios from 'axios';

// Create an axios instance with the base URL of your API
const api = axios.create({
  baseURL: 'https://localhost:7002/api', // Adjust this URL to match your backend
  headers: {
    'Content-Type': 'application/json',
  },
});

// Response interceptor to handle errors globally
api.interceptors.response.use(
  (response) => response,
  (error) => {
    console.error('API Error:', error.response?.data || error.message);
    return Promise.reject(error);
  }
);

export default api;