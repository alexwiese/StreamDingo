# Migration Strategies

Strategies for handling schema changes and system evolution in event-sourced systems.

## Event Schema Evolution

### Versioned Events
Use versioned events to handle schema changes:

```csharp
public record UserCreatedV1(string UserId, string Name, string Email);
public record UserCreatedV2(string UserId, string Name, string Email, string PhoneNumber);
```

### Migration Handlers
Create handlers to migrate between versions:

```csharp
public static UserCreatedV2 Migrate(UserCreatedV1 oldEvent)
{
    return new UserCreatedV2(
        oldEvent.UserId,
        oldEvent.Name, 
        oldEvent.Email,
        PhoneNumber: null // Default for new field
    );
}
```

## Aggregate Evolution

### Backward Compatibility
Design aggregates to handle old event versions:

```csharp
public static UserAggregate Handle(UserAggregate? state, UserCreatedV1 @event)
{
    return new UserAggregate(
        @event.UserId,
        @event.Name,
        @event.Email,
        PhoneNumber: null, // Handle missing field
        Version: (state?.Version ?? 0) + 1
    );
}
```

### Forward Compatibility
Consider future changes when designing events:

```csharp
public record UserCreated(
    string UserId,
    string Name, 
    string Email,
    Dictionary<string, object>? ExtensionData = null // For future fields
);
```

## Best Practices

1. **Never delete events** - Mark as deprecated instead
2. **Add new fields as optional** - Use nullable types or default values
3. **Version your events** - Use clear versioning schemes
4. **Test migrations thoroughly** - Validate all migration paths
5. **Keep migration logic simple** - Minimize complexity in migration handlers

More detailed migration strategies and examples coming soon.