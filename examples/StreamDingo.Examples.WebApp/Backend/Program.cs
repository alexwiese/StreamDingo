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

// Register StreamDingo services for User aggregates
builder.Services.AddSingleton<IEventStore, InMemoryEventStore>();
builder.Services.AddSingleton<ISnapshotStore<UserAggregate>, InMemorySnapshotStore<UserAggregate>>();
builder.Services.AddSingleton<ISnapshotStore<BusinessAggregate>, InMemorySnapshotStore<BusinessAggregate>>();
builder.Services.AddSingleton<IHashProvider, BasicHashProvider>();
builder.Services.AddSingleton<IEventStreamManager<UserAggregate>, EventStreamManager<UserAggregate>>();
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

// Register event handlers for User aggregates
var userStreamManager = app.Services.GetRequiredService<IEventStreamManager<UserAggregate>>();

// User event handlers
userStreamManager.RegisterHandler(new UserCreatedHandler());
userStreamManager.RegisterHandler(new UserUpdatedHandler());
userStreamManager.RegisterHandler(new UserDeletedHandler());

// Relationship event handlers (stored with users)
userStreamManager.RegisterHandler(new RelationshipCreatedHandler());
userStreamManager.RegisterHandler(new RelationshipUpdatedHandler());
userStreamManager.RegisterHandler(new RelationshipDeletedHandler());

// Register event handlers for Business aggregates
var businessStreamManager = app.Services.GetRequiredService<IEventStreamManager<BusinessAggregate>>();

// Business event handlers
businessStreamManager.RegisterHandler(new BusinessCreatedHandler());
businessStreamManager.RegisterHandler(new BusinessUpdatedHandler());
businessStreamManager.RegisterHandler(new BusinessDeletedHandler());

Console.WriteLine("StreamDingo Example Web API is starting...");
Console.WriteLine($"API will be available at: {builder.Configuration["urls"] ?? "https://localhost:7002"}");
Console.WriteLine("Registered event handlers for User and Business aggregates");
Console.WriteLine("- User aggregates: Users and their Relationships");
Console.WriteLine("- Business aggregates: Businesses only");

app.Run();
