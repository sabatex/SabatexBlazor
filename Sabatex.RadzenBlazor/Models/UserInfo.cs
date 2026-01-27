using Sabatex.Core;

namespace Sabatex.RadzenBlazor.Models;
/// <summary>
/// Add properties to this class and update the server and client AuthenticationStateProviders
/// to expose more information about the authenticated user to the client.
/// </summary>
public class UserInfo : IEntityBase<Guid>
{
    /// <summary>
    /// Represents the name of the administrator role used for access control or authorization checks.
    /// </summary>
    public const string AdministratorRole = "Administrator";
    /// <summary>
    /// user id in database
    /// </summary>
    public required Guid Id { get; set; }
    /// <summary>
    /// user email 
    /// </summary>
    public required string Email { get; set; }
    /// <summary>
    /// user name or email
    /// </summary>
    public required string Name { get; set; }
    /// <summary>
    /// user roles collection
    /// </summary>
    public required IEnumerable<string> Roles { get; set; }
    /// <summary>
    /// present roles as string separated comas
    /// </summary>
    public string RolesAsString => string.Join(", ", Roles);


}

