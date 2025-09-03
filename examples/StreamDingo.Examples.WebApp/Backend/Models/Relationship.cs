namespace StreamDingo.Examples.WebApp.Models;

/// <summary>
/// Types of relationships between users and businesses.
/// </summary>
public enum RelationshipType
{
    Employee,
    Partner,
    Contractor,
    Consultant,
    Owner,
    Investor
}

/// <summary>
/// Represents a relationship between a user and a business.
/// </summary>
public class Relationship
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public Guid BusinessId { get; set; }
    public RelationshipType Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;
}