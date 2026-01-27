using Sabatex.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabatex.RadzenBlazor.Models;
/// <summary>
/// Represents a user's subscription to receive notifications through a specified endpoint.
/// </summary>
/// <remarks>This class stores the information required to send push notifications to a user, including endpoint
/// details and cryptographic keys. It is typically used in systems that implement web push or similar notification
/// mechanisms.</remarks>
public class NotificationSubscription: IEntityBase<Guid>
{
    /// <summary>
    /// Gets or sets the unique identifier for the entity.
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// Gets or sets the unique identifier for the user associated with this instance.
    /// </summary>
    public string? UserId { get; set; }
    /// <summary>
    /// Gets or sets the network endpoint URI used to connect to the service.
    /// </summary>
    public string? Endpoint { get; set; }
    /// <summary>
    /// Gets or sets the P-256 Diffie-Hellman public key used for encrypted push notifications.
    /// </summary>
    /// <remarks>This property typically contains a Base64-encoded string representing the user's public key,
    /// as specified by the Web Push protocol. The value may be null if the key is not available.</remarks>
    public string? P256dh { get; set; }
    /// <summary>
    /// Gets or sets the authentication token or credentials used for accessing protected resources.
    /// </summary>
    public string? Auth { get; set; }

}