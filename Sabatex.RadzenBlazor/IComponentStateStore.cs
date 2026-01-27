using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabatex.RadzenBlazor;
/// <summary>
/// Defines a contract for asynchronously saving and loading component state using a key-based store.
/// </summary>
/// <remarks>Implementations of this interface enable components to persist and retrieve state data across
/// application sessions or lifecycles. The storage mechanism and serialization format are determined by the concrete
/// implementation. Thread safety and persistence guarantees may vary; refer to the specific implementation for
/// details.</remarks>
public interface IComponentStateStore
{
    /// <summary>
    /// Asynchronously saves the specified state object under the given key.
    /// </summary>
    /// <remarks>If a state already exists for the specified key, it will be overwritten. This method does not
    /// guarantee immediate persistence; the operation may be subject to eventual consistency depending on the
    /// underlying storage implementation.</remarks>
    /// <typeparam name="TState">The type of the state object to be saved.</typeparam>
    /// <param name="key">The unique identifier used to store and retrieve the state. Cannot be null or empty.</param>
    /// <param name="state">The state object to be saved. Cannot be null.</param>
    /// <returns>A task that represents the asynchronous save operation.</returns>
    Task SaveStateAsync<TState>(string key, TState state);
    /// <summary>
    /// Asynchronously loads the persisted state associated with the specified key.
    /// </summary>
    /// <typeparam name="TState">The type of the state object to load. Must be a reference type or a nullable value type.</typeparam>
    /// <param name="key">The unique key identifying the state to load. Cannot be null or empty.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the loaded state object if found;
    /// otherwise, null.</returns>
    Task<TState?> LoadStateAsync<TState>(string key);
}
