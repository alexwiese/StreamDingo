import axios from 'axios';

// Create an axios instance with the base URL of your API
const api = axios.create({
  baseURL: 'http://localhost:5000/api', // Using HTTP for demo to avoid SSL issues
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