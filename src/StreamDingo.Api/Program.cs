using StreamDingo.Core.Models;
using StreamDingo.Core.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { 
        Title = "StreamDingo API", 
        Version = "v1",
        Description = "A streaming platform API built with .NET - optimized for GitHub Copilot development"
    });
});

// Add CORS for web clients
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// TODO: Register services here
// Copilot will help suggest service registrations
// builder.Services.AddScoped<IStreamService, StreamService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();

// Stream Management Endpoints
// These endpoints demonstrate patterns that GitHub Copilot can help expand and improve

app.MapGet("/api/streams", async () =>
{
    // TODO: Implement with actual service
    // Copilot will suggest proper implementation patterns
    var sampleStreams = new List<StreamDingo.Core.Models.Stream>
    {
        new() { Title = "Gaming Stream", StreamerId = "user1", Category = "Gaming", IsLive = true, ViewerCount = 150 },
        new() { Title = "Coding Tutorial", StreamerId = "user2", Category = "Education", IsLive = true, ViewerCount = 75 },
        new() { Title = "Music Performance", StreamerId = "user3", Category = "Music", IsLive = false, ViewerCount = 0 }
    };
    return Results.Ok(sampleStreams);
})
.WithName("GetAllStreams")
.WithOpenApi()
.WithTags("Streams");

app.MapGet("/api/streams/live", async () =>
{
    // TODO: Get only live streams
    // Copilot can help implement filtering logic
    return Results.Ok("Live streams endpoint - to be implemented");
})
.WithName("GetLiveStreams")
.WithOpenApi()
.WithTags("Streams");

app.MapGet("/api/streams/{id:guid}", async (Guid id) =>
{
    // TODO: Get stream by ID
    // Copilot will suggest error handling and validation patterns
    return Results.Ok($"Get stream with ID: {id}");
})
.WithName("GetStreamById")
.WithOpenApi()
.WithTags("Streams");

app.MapPost("/api/streams", async (CreateStreamRequest request) =>
{
    // TODO: Create new stream
    // Copilot will help suggest validation and creation logic
    var newStream = new StreamDingo.Core.Models.Stream
    {
        Title = request.Title,
        Description = request.Description,
        StreamerId = request.StreamerId,
        Category = request.Category
    };
    return Results.Created($"/api/streams/{newStream.Id}", newStream);
})
.WithName("CreateStream")
.WithOpenApi()
.WithTags("Streams");

// User Management Endpoints
app.MapGet("/api/users/{id:guid}", async (Guid id) =>
{
    // TODO: Get user by ID
    // Copilot can suggest user retrieval patterns
    return Results.Ok($"Get user with ID: {id}");
})
.WithName("GetUserById")
.WithOpenApi()
.WithTags("Users");

app.Run();

// DTOs - Data Transfer Objects for API requests/responses
// These records demonstrate modern C# patterns that Copilot works well with

/// <summary>
/// Request model for creating a new stream
/// </summary>
/// <param name="Title">Stream title</param>
/// <param name="Description">Optional stream description</param>
/// <param name="StreamerId">ID of the user creating the stream</param>
/// <param name="Category">Stream category</param>
public record CreateStreamRequest(
    string Title,
    string? Description,
    string StreamerId,
    string? Category
);

// TODO: Add more request/response models
// Copilot will suggest additional models based on the API endpoints
