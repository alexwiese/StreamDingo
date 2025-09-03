import api from '../lib/api';
import type { Business, CreateBusinessRequest, UpdateBusinessRequest } from '../types';

export const businessService = {
  // Get all businesses
  async getAll(): Promise<Business[]> {
    const response = await api.get<Business[]>('/businesses');
    return response.data;
  },

  // Get business by ID
  async getById(id: string): Promise<Business> {
    const response = await api.get<Business>(`/businesses/${id}`);
    return response.data;
  },

  // Create business
  async create(business: CreateBusinessRequest): Promise<Business> {
    const response = await api.post<Business>('/businesses', business);
    return response.data;
  },

  // Update business
  async update(id: string, business: UpdateBusinessRequest): Promise<Business> {
    const response = await api.put<Business>(`/businesses/${id}`, business);
    return response.data;
  },

  // Delete business
  async delete(id: string): Promise<void> {
    await api.delete(`/businesses/${id}`);
  },
};