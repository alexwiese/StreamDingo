# Event Handlers

Event handlers are the core building blocks of StreamDingo. They are pure functions that transform aggregate state by applying events.

## Handler Fundamentals

### Pure Functions
Event handlers must be pure functions with no side effects:

```csharp
// ✅ Good: Pure function
public static UserAggregate Handle(UserAggregate? state, UserCreated @event)
    => new UserAggregate(
        Id: @event.UserId,
        Name: @event.Name,
        Email: @event.Email,
        Version: (state?.Version ?? 0) + 1
    );

// ❌ Bad: Has side effects
public static UserAggregate Handle(UserAggregate? state, UserCreated @event)
{
    // Side effect: logging
    Console.WriteLine($"User created: {@event.UserId}");
    
    // Side effect: external service call
    emailService.SendWelcomeEmail(@event.Email);
    
    return new UserAggregate(/*...*/);
}
```

### Handler Signature
All event handlers must follow this signature:

```csharp
public static TAggregate Handle(TAggregate? previousState, TEvent @event)
{
    // Handler logic
    return newState;
}
```

Where:
- `TAggregate`: The aggregate type being modified
- `TEvent`: The specific event type being handled
- `previousState`: Can be null for the first event in a stream
- Return value: The new aggregate state

## Registration Patterns

### Explicit Registration
Register handlers explicitly with the DI container:

```csharp
services.AddEventHandler<UserAggregate, UserCreated>(UserEventHandlers.Handle);
services.AddEventHandler<UserAggregate, UserEmailChanged>(UserEventHandlers.Handle);
services.AddEventHandler<UserAggregate, UserDeactivated>(UserEventHandlers.Handle);
```

### Convention-Based Registration
Use reflection to auto-register handlers:

```csharp
services.AddEventHandlersFromAssembly(typeof(UserEventHandlers).Assembly);

// Or with naming convention
services.AddEventHandlersWhere(type => 
    type.Name.EndsWith("EventHandlers") && 
    type.IsPublic && 
    !type.IsAbstract);
```

### Attribute-Based Registration
Use attributes to mark handlers for auto-registration:

```csharp
public class UserEventHandlers
{
    [EventHandler]
    public static UserAggregate Handle(UserAggregate? state, UserCreated @event)
        => /* handler logic */;
    
    [EventHandler]
    public static UserAggregate Handle(UserAggregate state, UserEmailChanged @event)
        => /* handler logic */;
}
```

## Advanced Handler Patterns

### Null State Handling
Always handle the case where previous state is null:

```csharp
public static UserAggregate Handle(UserAggregate? state, UserCreated @event)
{
    // First event - state will be null
    if (state == null)
    {
        return new UserAggregate(
            Id: @event.UserId,
            Name: @event.Name,
            Email: @event.Email,
            Version: 1
        );
    }
    
    // Subsequent events - should not happen for UserCreated
    throw new InvalidOperationException("User already exists");
}

public static UserAggregate Handle(UserAggregate state, UserEmailChanged @event)
{
    // State should never be null for update events
    if (state == null)
        throw new InvalidOperationException("Cannot update non-existent user");
    
    return state with 
    { 
        Email = @event.NewEmail,
        Version = state.Version + 1
    };
}
```

### Validation in Handlers
Include business rule validation:

```csharp
public static AccountAggregate Handle(AccountAggregate state, MoneyWithdrawn @event)
{
    if (state == null)
        throw new InvalidOperationException("Cannot withdraw from non-existent account");
    
    if (@event.Amount <= 0)
        throw new ArgumentException("Withdrawal amount must be positive");
    
    var newBalance = state.Balance - @event.Amount;
    if (newBalance < 0 && !state.AllowOverdraft)
        throw new InvalidOperationException("Insufficient funds");
    
    return state with 
    {
        Balance = newBalance,
        Version = state.Version + 1,
        LastTransactionAt = @event.WithdrawnAt
    };
}
```

### Handling Multiple Event Types
Use pattern matching for multiple event types:

```csharp
public static UserAggregate Handle(UserAggregate? state, object @event)
    => @event switch
    {
        UserCreated e => HandleUserCreated(state, e),
        UserEmailChanged e => HandleUserEmailChanged(state, e),
        UserDeactivated e => HandleUserDeactivated(state, e),
        UserReactivated e => HandleUserReactivated(state, e),
        _ => state ?? throw new InvalidOperationException("Unknown event type")
    };

private static UserAggregate HandleUserCreated(UserAggregate? state, UserCreated @event)
{
    if (state != null)
        throw new InvalidOperationException("User already exists");
    
    return new UserAggregate(/*...*/);
}
```

## Handler Organization

### Single Handler Class per Aggregate
Organize handlers by aggregate:

```csharp
public static class UserEventHandlers
{
    public static UserAggregate Handle(UserAggregate? state, UserCreated @event) { /*...*/ }
    public static UserAggregate Handle(UserAggregate state, UserEmailChanged @event) { /*...*/ }
    public static UserAggregate Handle(UserAggregate state, UserDeactivated @event) { /*...*/ }
}

public static class OrderEventHandlers  
{
    public static OrderAggregate Handle(OrderAggregate? state, OrderPlaced @event) { /*...*/ }
    public static OrderAggregate Handle(OrderAggregate state, OrderShipped @event) { /*...*/ }
    public static OrderAggregate Handle(OrderAggregate state, OrderCancelled @event) { /*...*/ }
}
```

### Nested Handler Classes
Group related handlers:

```csharp
public static class UserEventHandlers
{
    public static class Creation
    {
        public static UserAggregate Handle(UserAggregate? state, UserCreated @event) { /*...*/ }
    }
    
    public static class ProfileManagement
    {
        public static UserAggregate Handle(UserAggregate state, UserNameChanged @event) { /*...*/ }
        public static UserAggregate Handle(UserAggregate state, UserEmailChanged @event) { /*...*/ }
    }
    
    public static class AccountManagement
    {
        public static UserAggregate Handle(UserAggregate state, UserDeactivated @event) { /*...*/ }
        public static UserAggregate Handle(UserAggregate state, UserReactivated @event) { /*...*/ }
    }
}
```

## Error Handling

### Handler Exceptions
When handlers throw exceptions, replay will stop at that point:

```csharp
public static UserAggregate Handle(UserAggregate state, UserEmailChanged @event)
{
    if (string.IsNullOrWhiteSpace(@event.NewEmail))
        throw new ArgumentException("Email cannot be empty", nameof(@event.NewEmail));
    
    if (!IsValidEmail(@event.NewEmail))
        throw new ArgumentException("Invalid email format", nameof(@event.NewEmail));
    
    return state with { Email = @event.NewEmail, Version = state.Version + 1 };
}
```

### Graceful Degradation
For non-critical validation, consider logging instead of throwing:

```csharp
public static UserAggregate Handle(UserAggregate state, UserProfileUpdated @event, ILogger logger)
{
    var newState = state with 
    {
        Name = @event.NewName,
        Version = state.Version + 1
    };
    
    // Validate but don't fail
    if (string.IsNullOrWhiteSpace(@event.NewName))
    {
        logger.LogWarning("User {UserId} updated with empty name", state.Id);
    }
    
    return newState;
}
```

## Performance Considerations

### Avoid Heavy Computations
Keep handlers lightweight:

```csharp
// ❌ Avoid: Heavy computation in handler
public static UserAggregate Handle(UserAggregate state, UserScoreCalculated @event)
{
    // This should be done before creating the event
    var complexScore = CalculateComplexUserScore(@event.UserData);
    
    return state with { Score = complexScore };
}

// ✅ Better: Pre-computed values in events
public static UserAggregate Handle(UserAggregate state, UserScoreCalculated @event)
{
    return state with { Score = @event.CalculatedScore };
}
```

### Use Records for Immutability
Records provide efficient immutable updates:

```csharp
// ✅ Efficient with records
return state with 
{ 
    Email = @event.NewEmail,
    Version = state.Version + 1
};

// ❌ Less efficient object creation
return new UserAggregate(
    state.Id,
    state.Name,
    @event.NewEmail, // Only this changed
    state.Status,
    state.CreatedAt,
    state.Version + 1
);
```

## Testing Event Handlers

### Unit Testing
Test handlers in isolation:

```csharp
[Test]
public void UserCreated_Should_Create_New_User()
{
    // Arrange
    var @event = new UserCreated("user-1", "John Doe", "john@example.com");
    
    // Act
    var result = UserEventHandlers.Handle(null, @event);
    
    // Assert
    Assert.That(result.Id, Is.EqualTo("user-1"));
    Assert.That(result.Name, Is.EqualTo("John Doe"));
    Assert.That(result.Email, Is.EqualTo("john@example.com"));
    Assert.That(result.Version, Is.EqualTo(1));
}

[Test]
public void UserEmailChanged_Should_Update_Email()
{
    // Arrange
    var state = new UserAggregate("user-1", "John", "old@example.com", UserStatus.Active, 1);
    var @event = new UserEmailChanged("user-1", "new@example.com");
    
    // Act
    var result = UserEventHandlers.Handle(state, @event);
    
    // Assert
    Assert.That(result.Email, Is.EqualTo("new@example.com"));
    Assert.That(result.Version, Is.EqualTo(2));
    Assert.That(result.Name, Is.EqualTo("John")); // Unchanged
}
```

### Property-Based Testing
Use property-based testing for handler invariants:

```csharp
[Property]
public Property Handler_Should_Always_Increment_Version(UserAggregate state, UserEmailChanged @event)
{
    var result = UserEventHandlers.Handle(state, @event);
    return (result.Version == state.Version + 1).ToProperty();
}

[Property]
public Property Handler_Should_Preserve_Id(UserAggregate state, UserEmailChanged @event)
{
    var result = UserEventHandlers.Handle(state, @event);
    return (result.Id == state.Id).ToProperty();
}
```

## Handler Versioning

### Handling Schema Evolution
When event schemas change, use versioned handlers:

```csharp
public static UserAggregate Handle(UserAggregate? state, UserCreatedV1 @event)
{
    // Handle old version
    return new UserAggregate(
        Id: @event.UserId,
        Name: @event.Name,
        Email: @event.Email,
        PhoneNumber: null, // New field in V2
        Version: (state?.Version ?? 0) + 1
    );
}

public static UserAggregate Handle(UserAggregate? state, UserCreatedV2 @event)  
{
    // Handle new version
    return new UserAggregate(
        Id: @event.UserId,
        Name: @event.Name,
        Email: @event.Email,
        PhoneNumber: @event.PhoneNumber, // New in V2
        Version: (state?.Version ?? 0) + 1
    );
}
```

### Migration Handlers
Create handlers to migrate between versions:

```csharp
public static UserCreatedV2 Migrate(UserCreatedV1 oldEvent)
{
    return new UserCreatedV2(
        UserId: oldEvent.UserId,
        Name: oldEvent.Name,
        Email: oldEvent.Email,
        PhoneNumber: null // Default for missing field
    );
}
```

## Hash-Based Change Detection

StreamDingo uses hash-based change detection to determine when handlers have changed:

### What Triggers Replay
- Handler method body changes
- Handler dependencies change (referenced methods, constants)
- Event type schema changes

### Optimizing for Hash Stability
Keep handlers stable to avoid unnecessary replays:

```csharp
// ✅ Stable: Logic unlikely to change
public static UserAggregate Handle(UserAggregate? state, UserCreated @event)
    => new UserAggregate(@event.UserId, @event.Name, @event.Email, 1);

// ❌ Unstable: Formatting/comments cause hash changes
public static UserAggregate Handle(UserAggregate? state, UserCreated @event)
{
    // TODO: Add validation
    return new UserAggregate(
        id: @event.UserId,      // Changed parameter name
        name: @event.Name,
        email: @event.Email,
        version: 1
    );
}
```

## Next Steps

- Learn about [Snapshots](snapshots.md) and their relationship to handlers
- Understand [Hash Integrity](hash-integrity.md) verification
- Explore [Event Replay](event-replay.md) strategies
- See [Storage Providers](storage-providers.md) for persistence options