namespace StreamDingo.Examples.WebApp.Models;

/// <summary>
/// Business aggregate root containing business state and their relationships.
/// This is the snapshot that gets stored and replayed for business-specific event sourcing.
/// </summary>
public class BusinessAggregate
{
    public Business? Business { get; set; }
    public Dictionary<Guid, Relationship> Relationships { get; set; } = new();
    
    /// <summary>
    /// Gets all active relationships for this business.
    /// </summary>
    public IEnumerable<Relationship> ActiveRelationships => 
        Relationships.Values.Where(r => !r.IsDeleted && r.IsActive);
    
    /// <summary>
    /// Determines if this aggregate contains a valid business.
    /// </summary>
    public bool HasBusiness => Business != null && !Business.IsDeleted;
}