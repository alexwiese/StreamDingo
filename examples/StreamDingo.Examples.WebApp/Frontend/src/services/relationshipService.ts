import api from '../lib/api';
import type { Relationship, CreateRelationshipRequest, UpdateRelationshipRequest } from '../types';

export const relationshipService = {
  // Get all relationships
  async getAll(): Promise<Relationship[]> {
    const response = await api.get<Relationship[]>('/relationships');
    return response.data;
  },

  // Get relationship by ID
  async getById(id: string): Promise<Relationship> {
    const response = await api.get<Relationship>(`/relationships/${id}`);
    return response.data;
  },

  // Get relationships for a user
  async getByUserId(userId: string): Promise<Relationship[]> {
    const response = await api.get<Relationship[]>(`/relationships/user/${userId}`);
    return response.data;
  },

  // Get relationships for a business
  async getByBusinessId(businessId: string): Promise<Relationship[]> {
    const response = await api.get<Relationship[]>(`/relationships/business/${businessId}`);
    return response.data;
  },

  // Create relationship
  async create(relationship: CreateRelationshipRequest): Promise<Relationship> {
    const response = await api.post<Relationship>('/relationships', relationship);
    return response.data;
  },

  // Update relationship
  async update(id: string, relationship: UpdateRelationshipRequest): Promise<Relationship> {
    const response = await api.put<Relationship>(`/relationships/${id}`, relationship);
    return response.data;
  },

  // Delete relationship
  async delete(id: string): Promise<void> {
    await api.delete(`/relationships/${id}`);
  },
};