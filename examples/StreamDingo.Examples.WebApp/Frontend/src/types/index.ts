export interface User {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber: string;
  createdAt: string;
  updatedAt: string;
  isDeleted: boolean;
  fullName: string;
}

export interface CreateUserRequest {
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber: string;
}

export interface UpdateUserRequest {
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber: string;
}

export interface Business {
  id: string;
  name: string;
  description: string;
  industry: string;
  address: string;
  website: string;
  createdAt: string;
  updatedAt: string;
  isDeleted: boolean;
}

export interface CreateBusinessRequest {
  name: string;
  description: string;
  industry: string;
  address: string;
  website: string;
}

export interface UpdateBusinessRequest {
  name: string;
  description: string;
  industry: string;
  address: string;
  website: string;
}

export const RelationshipType = {
  Employee: 0,
  Partner: 1,
  Contractor: 2,
  Consultant: 3,
  Owner: 4,
  Investor: 5,
} as const;

export type RelationshipType = typeof RelationshipType[keyof typeof RelationshipType];

export interface Relationship {
  id: string;
  userId: string;
  businessId: string;
  type: RelationshipType;
  title: string;
  description: string;
  startDate: string;
  endDate?: string;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
  userName: string;
  businessName: string;
}

export interface CreateRelationshipRequest {
  userId: string;
  businessId: string;
  type: RelationshipType;
  title: string;
  description: string;
  startDate: string;
}

export interface UpdateRelationshipRequest {
  type: RelationshipType;
  title: string;
  description: string;
  startDate: string;
  endDate?: string;
  isActive: boolean;
}

// Helper function to get relationship type display name
export function getRelationshipTypeName(type: RelationshipType): string {
  switch (type) {
    case RelationshipType.Employee:
      return 'Employee';
    case RelationshipType.Partner:
      return 'Partner';
    case RelationshipType.Contractor:
      return 'Contractor';
    case RelationshipType.Consultant:
      return 'Consultant';
    case RelationshipType.Owner:
      return 'Owner';
    case RelationshipType.Investor:
      return 'Investor';
    default:
      return 'Unknown';
  }
}