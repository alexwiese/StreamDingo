namespace StreamDingo.Examples.WebApp.Models;

/// <summary>
/// User aggregate root containing user state and their relationships.
/// This is the snapshot that gets stored and replayed for user-specific event sourcing.
/// </summary>
public class UserAggregate
{
    public User? User { get; set; }
    public Dictionary<Guid, Relationship> Relationships { get; set; } = new();
    
    /// <summary>
    /// Gets all active relationships for this user.
    /// </summary>
    public IEnumerable<Relationship> ActiveRelationships => 
        Relationships.Values.Where(r => !r.IsDeleted && r.IsActive);
    
    /// <summary>
    /// Determines if this aggregate contains a valid user.
    /// </summary>
    public bool HasUser => User != null && !User.IsDeleted;
}