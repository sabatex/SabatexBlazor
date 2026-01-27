using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabatex.RadzenBlazor
{
    /// <summary>
    /// Represents the push subscription details required to send Web Push notifications to a Progressive Web App (PWA)
    /// client.
    /// </summary>
    /// <remarks>This record encapsulates all information necessary to target and securely deliver push
    /// notifications to a specific PWA client. The values are typically obtained from the browser's Push API
    /// subscription object.</remarks>
    /// <param name="endpoint">The absolute URL of the push service endpoint associated with the PWA client. This URL is used to deliver push
    /// messages.</param>
    /// <param name="p256dh">The client's public key, encoded in Base64, used for encrypting push message payloads according to the Web Push
    /// protocol.</param>
    /// <param name="auth">The authentication secret, encoded in Base64, used to authenticate push messages sent to the client.</param>
    public record PWAPushHandler(string endpoint,string p256dh,string auth);
    /// <summary>
    /// Defines methods for managing Progressive Web App (PWA) push notification subscriptions, including subscribing,
    /// unsubscribing, updating, and clearing push subscriptions.
    /// </summary>
    /// <remarks>Implementations of this interface enable applications to interact with browser push
    /// notification services, allowing users to receive and manage push notifications. Methods are asynchronous and may
    /// require user permission or browser support for push notifications.</remarks>
    public interface IPWAPush
    {
        /// <summary>
        /// Asynchronously retrieves the current push notification subscription for the user, if one exists.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="PWAPushHandler"/>
        /// instance representing the user's push subscription, or <see langword="null"/> if no subscription is
        /// available.</returns>
        Task<PWAPushHandler?> GetSubscriptionAsync();
        /// <summary>
        /// Initiates a subscription to push notifications for the current user asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="PWAPushHandler"/>
        /// instance if the subscription is successful; otherwise, <see langword="null"/>.</returns>
        Task<PWAPushHandler?> SubscribeAsync();
        /// <summary>
        /// Asynchronously unsubscribes from the current subscription, stopping the receipt of further updates.
        /// </summary>
        /// <returns>A task that represents the asynchronous unsubscribe operation.</returns>
        Task UnSubscribeAsync();
        /// <summary>
        /// Asynchronously updates the current subscription with the latest configuration or data.
        /// </summary>
        /// <returns>A task that represents the asynchronous update operation.</returns>
        Task UpdateSubscriptionAsync();
        /// <summary>
        /// Asynchronously removes all items from the current subscription.
        /// </summary>
        /// <returns>A task that represents the asynchronous clear operation.</returns>
        Task ClearSubscriptionAsync();

    }
}
