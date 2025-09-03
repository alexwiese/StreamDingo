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
builder.Services.AddSingleton<ISnapshotStore<DomainSnapshot>, InMemorySnapshotStore<DomainSnapshot>>();
builder.Services.AddSingleton<IHashProvider, BasicHashProvider>();
builder.Services.AddSingleton<IEventStreamManager<DomainSnapshot>, EventStreamManager<DomainSnapshot>>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("AllowReactApp");
app.UseRouting();
app.MapControllers();

// Register event handlers
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

Console.WriteLine("StreamDingo Example Web API is starting...");
Console.WriteLine($"API will be available at: {builder.Configuration["urls"] ?? "https://localhost:7002"}");
Console.WriteLine("Registered event handlers for Users, Businesses, and Relationships");

app.Run();
