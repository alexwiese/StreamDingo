using StreamDingo;
using StreamDingo.Examples.WebApp.Events;
using StreamDingo.Examples.WebApp.Handlers;
using StreamDingo.Examples.WebApp.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Configure CORS for React frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Register StreamDingo services
builder.Services.AddSingleton<IEventStore, InMemoryEventStore>();
builder.Services.AddSingleton<IHashProvider, BasicHashProvider>();

// Register aggregate-specific services
builder.Services.AddSingleton<ISnapshotStore<DomainSnapshot>, InMemorySnapshotStore<DomainSnapshot>>();
builder.Services.AddSingleton<IEventStreamManager<DomainSnapshot>, EventStreamManager<DomainSnapshot>>();

builder.Services.AddSingleton<ISnapshotStore<UserAggregate>, InMemorySnapshotStore<UserAggregate>>();
builder.Services.AddSingleton<IEventStreamManager<UserAggregate>, EventStreamManager<UserAggregate>>();

builder.Services.AddSingleton<ISnapshotStore<BusinessAggregate>, InMemorySnapshotStore<BusinessAggregate>>();
builder.Services.AddSingleton<IEventStreamManager<BusinessAggregate>, EventStreamManager<BusinessAggregate>>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Comment out HTTPS redirection for demo purposes to avoid SSL certificate issues
// app.UseHttpsRedirection();
app.UseCors("AllowReactApp");
app.UseRouting();
app.MapControllers();

// Register event handlers for legacy domain snapshot (for backward compatibility)
var streamManager = app.Services.GetRequiredService<IEventStreamManager<DomainSnapshot>>();

// User event handlers
streamManager.RegisterHandler(new UserCreatedHandler());
streamManager.RegisterHandler(new UserUpdatedHandler());
streamManager.RegisterHandler(new UserDeletedHandler());

// Business event handlers
streamManager.RegisterHandler(new BusinessCreatedHandler());
streamManager.RegisterHandler(new BusinessUpdatedHandler());
streamManager.RegisterHandler(new BusinessDeletedHandler());

// Relationship event handlers
streamManager.RegisterHandler(new RelationshipCreatedHandler());
streamManager.RegisterHandler(new RelationshipUpdatedHandler());
streamManager.RegisterHandler(new RelationshipDeletedHandler());

// Register aggregate-specific event handlers
var userStreamManager = app.Services.GetRequiredService<IEventStreamManager<UserAggregate>>();
var businessStreamManager = app.Services.GetRequiredService<IEventStreamManager<BusinessAggregate>>();

// User aggregate handlers
userStreamManager.RegisterHandler(new UserAggregateCreatedHandler());
userStreamManager.RegisterHandler(new UserAggregateUpdatedHandler());
userStreamManager.RegisterHandler(new UserAggregateDeletedHandler());
userStreamManager.RegisterHandler(new UserAggregateRelationshipCreatedHandler());
userStreamManager.RegisterHandler(new UserAggregateRelationshipUpdatedHandler());
userStreamManager.RegisterHandler(new UserAggregateRelationshipDeletedHandler());

// Business aggregate handlers
businessStreamManager.RegisterHandler(new BusinessAggregateCreatedHandler());
businessStreamManager.RegisterHandler(new BusinessAggregateUpdatedHandler());
businessStreamManager.RegisterHandler(new BusinessAggregateDeletedHandler());
businessStreamManager.RegisterHandler(new BusinessAggregateRelationshipCreatedHandler());
businessStreamManager.RegisterHandler(new BusinessAggregateRelationshipUpdatedHandler());
businessStreamManager.RegisterHandler(new BusinessAggregateRelationshipDeletedHandler());

Console.WriteLine("StreamDingo Example Web API is starting...");
Console.WriteLine($"API will be available at: {builder.Configuration["urls"] ?? "https://localhost:7002"}");
Console.WriteLine("Registered event handlers for Users, Businesses, and Relationships");
Console.WriteLine("Aggregate-based event sourcing enabled:");
Console.WriteLine("  - Users: user-{userId} streams");
Console.WriteLine("  - Businesses: business-{businessId} streams");
Console.WriteLine("  - Legacy domain-stream maintained for backward compatibility");

app.Run();
