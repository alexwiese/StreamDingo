import React, { useState, useEffect } from 'react';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '../components/ui/card';
import { Button } from '../components/ui/button';
import { Input } from '../components/ui/input';
import { Textarea } from '../components/ui/textarea';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '../components/ui/table';
import { relationshipService } from '../services/relationshipService';
import { userService } from '../services/userService';
import { businessService } from '../services/businessService';
import { 
  RelationshipType,
  getRelationshipTypeName 
} from '../types';
import type {
  Relationship, 
  CreateRelationshipRequest, 
  UpdateRelationshipRequest, 
  User, 
  Business
} from '../types';

export default function RelationshipsPage() {
  const [relationships, setRelationships] = useState<Relationship[]>([]);
  const [users, setUsers] = useState<User[]>([]);
  const [businesses, setBusinesses] = useState<Business[]>([]);
  const [loading, setLoading] = useState(true);
  const [showForm, setShowForm] = useState(false);
  const [editingRelationship, setEditingRelationship] = useState<Relationship | null>(null);
  const [formData, setFormData] = useState<CreateRelationshipRequest>({
    userId: '',
    businessId: '',
    type: RelationshipType.Employee,
    title: '',
    description: '',
    startDate: new Date().toISOString().split('T')[0]
  });

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      const [relationshipsData, usersData, businessesData] = await Promise.all([
        relationshipService.getAll(),
        userService.getAll(),
        businessService.getAll()
      ]);
      setRelationships(relationshipsData);
      setUsers(usersData);
      setBusinesses(businessesData);
    } catch (error) {
      console.error('Failed to load data:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      if (editingRelationship) {
        const updateData: UpdateRelationshipRequest = {
          type: formData.type,
          title: formData.title,
          description: formData.description,
          startDate: formData.startDate,
          isActive: true
        };
        await relationshipService.update(editingRelationship.id, updateData);
      } else {
        await relationshipService.create(formData);
      }
      setShowForm(false);
      setEditingRelationship(null);
      setFormData({
        userId: '',
        businessId: '',
        type: RelationshipType.Employee,
        title: '',
        description: '',
        startDate: new Date().toISOString().split('T')[0]
      });
      loadData();
    } catch (error) {
      console.error('Failed to save relationship:', error);
    }
  };

  const handleEdit = (relationship: Relationship) => {
    setEditingRelationship(relationship);
    setFormData({
      userId: relationship.userId,
      businessId: relationship.businessId,
      type: relationship.type,
      title: relationship.title,
      description: relationship.description,
      startDate: relationship.startDate.split('T')[0]
    });
    setShowForm(true);
  };

  const handleDelete = async (id: string) => {
    if (confirm('Are you sure you want to delete this relationship?')) {
      try {
        await relationshipService.delete(id);
        loadData();
      } catch (error) {
        console.error('Failed to delete relationship:', error);
      }
    }
  };

  const handleCancel = () => {
    setShowForm(false);
    setEditingRelationship(null);
    setFormData({
      userId: '',
      businessId: '',
      type: RelationshipType.Employee,
      title: '',
      description: '',
      startDate: new Date().toISOString().split('T')[0]
    });
  };

  if (loading) {
    return <div className="flex justify-center items-center h-64">Loading...</div>;
  }

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <h1 className="text-3xl font-bold text-gray-900">Relationships</h1>
        <Button onClick={() => setShowForm(true)} disabled={users.length === 0 || businesses.length === 0}>
          Add Relationship
        </Button>
      </div>

      {(users.length === 0 || businesses.length === 0) && (
        <Card>
          <CardContent className="pt-6">
            <div className="text-center">
              <p className="text-gray-500">
                You need at least one user and one business to create relationships.
              </p>
              <div className="mt-2 space-x-2">
                {users.length === 0 && (
                  <span className="text-sm text-red-600">No users available</span>
                )}
                {businesses.length === 0 && (
                  <span className="text-sm text-red-600">No businesses available</span>
                )}
              </div>
            </div>
          </CardContent>
        </Card>
      )}

      {showForm && (
        <Card>
          <CardHeader>
            <CardTitle>{editingRelationship ? 'Edit Relationship' : 'Add New Relationship'}</CardTitle>
            <CardDescription>
              {editingRelationship ? 'Update relationship information' : 'Create a new relationship between a user and business'}
            </CardDescription>
          </CardHeader>
          <CardContent>
            <form onSubmit={handleSubmit} className="space-y-4">
              <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    User
                  </label>
                  <select
                    className="flex h-10 w-full rounded-md border border-gray-300 bg-white px-3 py-2 text-sm focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-blue-500 focus-visible:ring-offset-2"
                    value={formData.userId}
                    onChange={(e) => setFormData(prev => ({ ...prev, userId: e.target.value }))}
                    required
                    disabled={editingRelationship != null}
                  >
                    <option value="">Select a user</option>
                    {users.map((user) => (
                      <option key={user.id} value={user.id}>
                        {user.fullName} ({user.email})
                      </option>
                    ))}
                  </select>
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Business
                  </label>
                  <select
                    className="flex h-10 w-full rounded-md border border-gray-300 bg-white px-3 py-2 text-sm focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-blue-500 focus-visible:ring-offset-2"
                    value={formData.businessId}
                    onChange={(e) => setFormData(prev => ({ ...prev, businessId: e.target.value }))}
                    required
                    disabled={editingRelationship != null}
                  >
                    <option value="">Select a business</option>
                    {businesses.map((business) => (
                      <option key={business.id} value={business.id}>
                        {business.name}
                      </option>
                    ))}
                  </select>
                </div>
              </div>
              
              <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Relationship Type
                  </label>
                  <select
                    className="flex h-10 w-full rounded-md border border-gray-300 bg-white px-3 py-2 text-sm focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-blue-500 focus-visible:ring-offset-2"
                    value={formData.type}
                    onChange={(e) => setFormData(prev => ({ ...prev, type: parseInt(e.target.value) as RelationshipType }))}
                  >
                    {Object.values(RelationshipType).filter(v => typeof v === 'number').map((type) => (
                      <option key={type} value={type}>
                        {getRelationshipTypeName(type as RelationshipType)}
                      </option>
                    ))}
                  </select>
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Start Date
                  </label>
                  <Input
                    type="date"
                    value={formData.startDate}
                    onChange={(e) => setFormData(prev => ({ ...prev, startDate: e.target.value }))}
                    required
                  />
                </div>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Title/Position
                </label>
                <Input
                  value={formData.title}
                  onChange={(e) => setFormData(prev => ({ ...prev, title: e.target.value }))}
                  placeholder="e.g., Senior Developer, CEO, Marketing Consultant"
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Description
                </label>
                <Textarea
                  value={formData.description}
                  onChange={(e) => setFormData(prev => ({ ...prev, description: e.target.value }))}
                  placeholder="Additional details about this relationship"
                />
              </div>

              <div className="flex space-x-2">
                <Button type="submit">
                  {editingRelationship ? 'Update' : 'Create'} Relationship
                </Button>
                <Button type="button" variant="outline" onClick={handleCancel}>
                  Cancel
                </Button>
              </div>
            </form>
          </CardContent>
        </Card>
      )}

      <Card>
        <CardHeader>
          <CardTitle>All Relationships</CardTitle>
          <CardDescription>
            Manage relationships between users and businesses
          </CardDescription>
        </CardHeader>
        <CardContent>
          {relationships.length === 0 ? (
            <div className="text-center py-8">
              <p className="text-gray-500">No relationships found. Create your first relationship to get started.</p>
            </div>
          ) : (
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>User</TableHead>
                  <TableHead>Business</TableHead>
                  <TableHead>Type</TableHead>
                  <TableHead>Title</TableHead>
                  <TableHead>Start Date</TableHead>
                  <TableHead>Status</TableHead>
                  <TableHead className="text-right">Actions</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {relationships.map((relationship) => (
                  <TableRow key={relationship.id}>
                    <TableCell className="font-medium">{relationship.userName}</TableCell>
                    <TableCell>{relationship.businessName}</TableCell>
                    <TableCell>{getRelationshipTypeName(relationship.type)}</TableCell>
                    <TableCell>{relationship.title || '-'}</TableCell>
                    <TableCell>{new Date(relationship.startDate).toLocaleDateString()}</TableCell>
                    <TableCell>
                      <span className={`inline-flex px-2 py-1 text-xs rounded-full ${
                        relationship.isActive 
                          ? 'bg-green-100 text-green-800' 
                          : 'bg-gray-100 text-gray-800'
                      }`}>
                        {relationship.isActive ? 'Active' : 'Inactive'}
                      </span>
                    </TableCell>
                    <TableCell className="text-right space-x-2">
                      <Button 
                        size="sm" 
                        variant="outline" 
                        onClick={() => handleEdit(relationship)}
                      >
                        Edit
                      </Button>
                      <Button 
                        size="sm" 
                        variant="destructive" 
                        onClick={() => handleDelete(relationship.id)}
                      >
                        Delete
                      </Button>
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          )}
        </CardContent>
      </Card>
    </div>
  );
}