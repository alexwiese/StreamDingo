namespace StreamDingo.Examples.WebApp.Models;

/// <summary>
/// Aggregate root containing all the domain state for the application.
/// This is the snapshot that gets stored and replayed in event sourcing.
/// </summary>
public class DomainSnapshot
{
    public Dictionary<Guid, User> Users { get; set; } = new();
    public Dictionary<Guid, Business> Businesses { get; set; } = new();
    public Dictionary<Guid, Relationship> Relationships { get; set; } = new();
    
    /// <summary>
    /// Gets all active (non-deleted) users.
    /// </summary>
    public IEnumerable<User> ActiveUsers => Users.Values.Where(u => !u.IsDeleted);
    
    /// <summary>
    /// Gets all active (non-deleted) businesses.
    /// </summary>
    public IEnumerable<Business> ActiveBusinesses => Businesses.Values.Where(b => !b.IsDeleted);
    
    /// <summary>
    /// Gets all active (non-deleted) relationships.
    /// </summary>
    public IEnumerable<Relationship> ActiveRelationships => Relationships.Values.Where(r => !r.IsDeleted);
    
    /// <summary>
    /// Gets relationships for a specific user.
    /// </summary>
    public IEnumerable<Relationship> GetUserRelationships(Guid userId) => 
        ActiveRelationships.Where(r => r.UserId == userId);
    
    /// <summary>
    /// Gets relationships for a specific business.
    /// </summary>
    public IEnumerable<Relationship> GetBusinessRelationships(Guid businessId) => 
        ActiveRelationships.Where(r => r.BusinessId == businessId);
}