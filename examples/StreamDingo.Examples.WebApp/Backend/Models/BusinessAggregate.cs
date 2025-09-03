namespace StreamDingo.Examples.WebApp.Models;

/// <summary>
/// Business aggregate root containing business data.
/// This replaces the domain-wide snapshot for business-specific operations.
/// </summary>
public class BusinessAggregate
{
    public Business? Business { get; set; }
    
    /// <summary>
    /// Gets the business if it exists and is not deleted.
    /// </summary>
    public Business? ActiveBusiness => Business?.IsDeleted == false ? Business : null;
}