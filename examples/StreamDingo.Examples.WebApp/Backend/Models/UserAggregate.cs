namespace StreamDingo.Examples.WebApp.Models;

/// <summary>
/// User aggregate root containing user data and their relationships.
/// This replaces the domain-wide snapshot for user-specific operations.
/// </summary>
public class UserAggregate
{
    public User? User { get; set; }
    public Dictionary<Guid, Relationship> Relationships { get; set; } = new();
    
    /// <summary>
    /// Gets all active (non-deleted) relationships for this user.
    /// </summary>
    public IEnumerable<Relationship> ActiveRelationships => 
        Relationships.Values.Where(r => !r.IsDeleted);
    
    /// <summary>
    /// Gets the user if they exist and are not deleted.
    /// </summary>
    public User? ActiveUser => User?.IsDeleted == false ? User : null;
}