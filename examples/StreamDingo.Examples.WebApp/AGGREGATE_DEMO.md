# StreamDingo WebApp - Aggregate-Based Event Sourcing Demo

This document demonstrates the new aggregate-based event sourcing architecture implemented in the StreamDingo WebApp.

## Key Architectural Changes

### Before (Issues)
- All events stored in single "domain-stream"
- Querying any user required replaying ALL domain events
- No aggregate boundaries
- Poor scalability for large datasets

### After (Solution)
- Events partitioned by aggregate ID:
  - User events: `user-{userId}` streams
  - Business events: `business-{businessId}` streams
- Only relevant events replayed per query
- Proper aggregate boundaries
- Scalable architecture

## API Examples

### Efficient User Queries
```http
# Get specific user (only replays user-{userId} events)
GET /api/users/{userId}

# Get user's relationships (from user aggregate)
GET /api/users/{userId}/relationships
```

### Efficient Business Queries
```http
# Get specific business (only replays business-{businessId} events)
GET /api/businesses/{businessId}

# Get business relationships (from business aggregate)
GET /api/businesses/{businessId}/relationships
```

### Cross-Aggregate Operations
```http
# Create relationship (updates both user and business aggregates)
POST /api/relationships
{
  "userId": "user-guid",
  "businessId": "business-guid",
  "type": "Employee",
  "title": "Software Engineer"
}
```

## Stream Partitioning Examples

### User Stream: `user-12345`
```
Event 0: UserCreated { Name: "Alice", Email: "alice@example.com" }
Event 1: RelationshipCreated { UserId: "12345", BusinessId: "67890", Type: "Employee" }
Event 2: UserUpdated { Email: "alice.new@example.com" }
```

### Business Stream: `business-67890`
```
Event 0: BusinessCreated { Name: "Tech Corp", Industry: "Software" }
Event 1: RelationshipCreated { UserId: "12345", BusinessId: "67890", Type: "Employee" }
Event 2: BusinessUpdated { Name: "Tech Corp Inc" }
```

## Benefits Demonstrated

1. **Efficient Querying**: Getting Alice's data only replays 3 events from `user-12345`, not all domain events
2. **Scalability**: Each aggregate can be horizontally partitioned
3. **Performance**: No unnecessary event replay
4. **Consistency**: Relationships maintained in both aggregates for fast queries
5. **Backward Compatibility**: Legacy domain-stream still available for list operations

## Testing Validation

The implementation includes comprehensive tests that prove:
- Stream independence (user events don't affect business streams)
- Aggregate isolation (only relevant events replayed)
- Cross-aggregate consistency (relationships properly maintained)
- Backward compatibility (existing functionality preserved)

This architecture directly addresses the issue requirement: "We don't want to replay every event in the whole domain every time you lookup a given user" ✅