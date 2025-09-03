namespace StreamDingo.Examples.WebApp.Models;

/// <summary>
/// Represents a business in the system.
/// </summary>
public class Business
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Industry { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Website { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public bool IsDeleted { get; set; } = false;
}