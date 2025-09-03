# StreamDingo Example Web Application

A comprehensive full-stack example demonstrating StreamDingo's event sourcing capabilities with a React frontend and .NET backend.

## Overview

This example application showcases how to build a production-quality event sourcing system using StreamDingo. It manages Users, Businesses, and their Relationships, demonstrating:

- **Event-driven architecture** with pure event handlers
- **Cross-entity relationships** managed through event sourcing
- **Hash-based integrity verification** for data consistency
- **Modern React frontend** with TypeScript and Tailwind CSS
- **RESTful API design** with proper CRUD operations

## Architecture

### Backend (.NET 9.0 + ASP.NET Core)
- **StreamDingo Event Sourcing**: Core event sourcing library
- **Domain Models**: User, Business, Relationship with complex relationships
- **Events**: UserCreated, BusinessUpdated, RelationshipDeleted, etc.
- **Event Handlers**: Pure functions transforming snapshots
- **REST Controllers**: Full CRUD API with proper HTTP verbs
- **In-Memory Storage**: For demonstration purposes

### Frontend (React + TypeScript + Vite)
- **React 18** with TypeScript for type safety
- **Vite** for fast development and building  
- **Tailwind CSS** for modern styling
- **shadcn/ui-style components** for consistent UI
- **Axios** for HTTP client with error handling
- **React Router** for navigation

## Domain Model

### Entities
1. **Users**: Individuals with names, emails, and phone numbers
2. **Businesses**: Companies with industry, address, and website details  
3. **Relationships**: Connections between users and businesses

### Relationship Types
- Employee
- Partner  
- Contractor
- Consultant
- Owner
- Investor

### Event Sourcing Flow
1. User action (e.g., "Create User") triggers event creation
2. Event handler applies the event to current snapshot
3. New snapshot is stored with hash verification
4. All changes are immutably recorded as events
5. State can be replayed from any point in time

## Getting Started

### Prerequisites
- .NET 9.0 SDK
- Node.js 18+ 
- Git

### Running the Application

1. **Start the Backend API**:
   ```bash
   cd Backend
   dotnet run --urls "http://localhost:5000"
   ```

2. **Start the Frontend** (in a new terminal):
   ```bash
   cd Frontend  
   npm install
   npm run dev
   ```

3. **Open your browser** to `http://localhost:5173`

### API Endpoints

#### Users
- `GET /api/users` - List all users
- `POST /api/users` - Create a new user
- `PUT /api/users/{id}` - Update a user
- `DELETE /api/users/{id}` - Soft delete a user

#### Businesses  
- `GET /api/businesses` - List all businesses
- `POST /api/businesses` - Create a new business
- `PUT /api/businesses/{id}` - Update a business
- `DELETE /api/businesses/{id}` - Soft delete a business

#### Relationships
- `GET /api/relationships` - List all relationships
- `GET /api/relationships/user/{userId}` - Get relationships for a user
- `GET /api/relationships/business/{businessId}` - Get relationships for a business
- `POST /api/relationships` - Create a new relationship
- `PUT /api/relationships/{id}` - Update a relationship
- `DELETE /api/relationships/{id}` - Soft delete a relationship

## Key Features Demonstrated

### StreamDingo Event Sourcing
- **Immutable Event Log**: All changes stored as events
- **Pure Event Handlers**: Deterministic state transformations
- **Snapshot Management**: Current state derived from events
- **Hash Verification**: Data integrity through cryptographic hashes
- **Event Replay**: Rebuild state from any point in history

### Advanced Functionality
- **Soft Deletes**: Entities marked as deleted, not removed
- **Cross-Entity Validation**: Relationships validate referenced entities
- **Status Management**: Active/inactive relationship tracking
- **Audit Trail**: Complete history of all changes
- **Type Safety**: Full TypeScript integration across stack

### User Interface
- **Responsive Design**: Works on desktop and mobile
- **Form Validation**: Client-side and server-side validation
- **Real-time Updates**: Automatic refresh after operations
- **Error Handling**: User-friendly error messages
- **Loading States**: Visual feedback during API calls

## Event Sourcing Benefits Demonstrated

1. **Complete Audit Trail**: Every change is recorded with timestamp
2. **Temporal Queries**: Can ask "what was the state at any time?"  
3. **Debugging**: Full event history makes debugging easier
4. **Scalability**: Events can be processed asynchronously
5. **Consistency**: Hash verification ensures data hasn't been tampered with
6. **Flexibility**: New projections can be created from existing events

## Learning Points

This example teaches:
- How to design event-sourced domain models
- Implementing pure event handlers with StreamDingo
- Building RESTful APIs with event sourcing backends
- Creating responsive UIs that work with event-driven systems
- Managing cross-entity relationships in event sourcing
- Handling concurrency and data consistency

## Production Considerations

For production use, consider:
- **Persistent Storage**: Replace in-memory stores with SQL Server, PostgreSQL, or EventStore
- **Authentication & Authorization**: Add identity management
- **Validation**: Enhance input validation and business rules
- **Error Handling**: Implement comprehensive error handling
- **Logging**: Add structured logging and monitoring
- **Performance**: Implement caching and pagination
- **Testing**: Add comprehensive unit and integration tests

## License

This example is part of the StreamDingo project and follows the same MIT license.