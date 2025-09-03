import api from '../lib/api';
import type { User, CreateUserRequest, UpdateUserRequest } from '../types';

export const userService = {
  // Get all users
  async getAll(): Promise<User[]> {
    const response = await api.get<User[]>('/users');
    return response.data;
  },

  // Get user by ID
  async getById(id: string): Promise<User> {
    const response = await api.get<User>(`/users/${id}`);
    return response.data;
  },

  // Create user
  async create(user: CreateUserRequest): Promise<User> {
    const response = await api.post<User>('/users', user);
    return response.data;
  },

  // Update user
  async update(id: string, user: UpdateUserRequest): Promise<User> {
    const response = await api.put<User>(`/users/${id}`, user);
    return response.data;
  },

  // Delete user
  async delete(id: string): Promise<void> {
    await api.delete(`/users/${id}`);
  },
};