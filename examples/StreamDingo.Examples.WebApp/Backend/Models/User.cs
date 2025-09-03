namespace StreamDingo.Examples.WebApp.Models;

/// <summary>
/// Represents a user in the system.
/// </summary>
public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public bool IsDeleted { get; set; } = false;
    
    public string FullName => $"{FirstName} {LastName}".Trim();
}