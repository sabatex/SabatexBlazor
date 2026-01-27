using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sabatex.Core;
using Sabatex.Core.RadzenBlazor;
using System.Reflection;

namespace Sabatex.RadzenBlazor.Server;
/// <summary>
/// Provides a base class for data adapters that support CRUD operations for entities using a specified key type.
/// Intended for use with implementations of the ISabatexRadzenBlazorDataAdapter interface.
/// </summary>
/// <remarks>Inherit from this class to implement data access logic for specific entity types in applications that
/// use Radzen Blazor components. This base class supplies access to the underlying DbContext and logger for use in
/// derived classes. Thread safety depends on the implementation of the derived class and the lifetime of the injected
/// services.</remarks>
/// <typeparam name="TItem">The type of the entity managed by the data adapter. Must implement IEntityBase<TKey>.</typeparam>
/// <typeparam name="TKey">The type of the key used to identify entities.</typeparam>
public abstract class DataAdapterBase<TItem, TKey> : ISabatexRadzenBlazorDataAdapter 
    where TItem : class, IEntityBase<TKey>
{
    /// <summary>
    /// Provides access to the underlying database context used for data operations within the derived class.
    /// </summary>
    /// <remarks>This field is intended for use by derived classes to interact with the database. It should
    /// not be modified outside of the class hierarchy.</remarks>
    protected readonly DbContext Db;
    /// <summary>
    /// Provides access to the logger used for recording diagnostic and operational messages within the class.
    /// </summary>
    /// <remarks>Intended for use by derived classes to log information, warnings, errors, or other events.
    /// The specific behavior and configuration of the logger depend on the implementation of the injected ILogger
    /// instance.</remarks>
    protected readonly ILogger Logger;
    /// <summary>
    /// Initializes a new instance of the DataAdapterBase class with the specified database context and logger.
    /// </summary>
    /// <param name="db">The database context to be used by the data adapter. Cannot be null.</param>
    /// <param name="logger">The logger instance used for logging operations. Cannot be null.</param>
    protected DataAdapterBase(DbContext db, ILogger logger)
    {
        Db = db;
        Logger = logger;
    }
    /// <summary>
    /// Asynchronously retrieves a collection of items that match the specified query parameters.
    /// </summary>
    /// <typeparam name="TItem">The type of the items to be retrieved.</typeparam>
    /// <typeparam name="TKey">The type of the key used to identify items.</typeparam>
    /// <param name="queryParams">The parameters that define filtering, sorting, paging, and other query options for retrieving items. Cannot be
    /// null.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="QueryResult{TItem}"/>
    /// with the items that match the query parameters.</returns>
    /// <exception cref="NotImplementedException">The method is not implemented.</exception>
    Task<QueryResult<TItem>> ISabatexRadzenBlazorDataAdapter.GetAsync<TItem, TKey>(QueryParams queryParams)
    {
        throw new NotImplementedException();
    }
    /// <summary>
    /// Asynchronously retrieves an item by its unique identifier, optionally including related entities.
    /// </summary>
    /// <typeparam name="TItem">The type of the item to retrieve.</typeparam>
    /// <typeparam name="TKey">The type of the unique identifier for the item.</typeparam>
    /// <param name="id">The unique identifier of the item to retrieve. Cannot be null.</param>
    /// <param name="expand">A comma-separated list of related entities to include in the result, or null to exclude related data.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the item matching the specified
    /// identifier, or null if no such item exists.</returns>
    /// <exception cref="NotImplementedException">The method is not implemented.</exception>
    Task<TItem> ISabatexRadzenBlazorDataAdapter.GetByIdAsync<TItem, TKey>(TKey id, string? expand)
    {
        throw new NotImplementedException();
    }
    /// <summary>
    /// Asynchronously retrieves an item by its unique identifier, optionally including related entities.
    /// </summary>
    /// <typeparam name="TItem1">The type of the item to retrieve.</typeparam>
    /// <typeparam name="TKey1">The type of the item's unique identifier.</typeparam>
    /// <param name="id">The unique identifier of the item to retrieve. Cannot be null.</param>
    /// <param name="expand">A comma-separated list of related entities to include in the result, or null to exclude related data.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the item matching the specified
    /// identifier.</returns>
    /// <exception cref="NotImplementedException">The method is not implemented.</exception>
    Task<TItem1> ISabatexRadzenBlazorDataAdapter.GetByIdAsync<TItem1, TKey1>(string id, string? expand)
    {
        throw new NotImplementedException();
    }
    /// <summary>
    /// Asynchronously creates a new item in the data source.
    /// </summary>
    /// <typeparam name="TItem1">The type of the item to create.</typeparam>
    /// <typeparam name="TKey1">The type of the key for the item.</typeparam>
    /// <param name="item">The item to add to the data source. Cannot be null.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a validation model with the created
    /// item and any validation results.</returns>
    /// <exception cref="NotImplementedException">The method is not implemented.</exception>
    Task<SabatexValidationModel<TItem1>> ISabatexRadzenBlazorDataAdapter.PostAsync<TItem1, TKey1>(TItem1 item)
    {
        throw new NotImplementedException();
    }
    /// <summary>
    /// Asynchronously updates the specified item in the data source.
    /// </summary>
    /// <typeparam name="TItem1">The type of the item to update.</typeparam>
    /// <typeparam name="TKey1">The type of the key used to identify the item.</typeparam>
    /// <param name="item">The item to update in the data source. Cannot be null.</param>
    /// <returns>A task that represents the asynchronous update operation. The task result contains a validation model indicating
    /// the outcome of the update.</returns>
    /// <exception cref="NotImplementedException">The method is not implemented.</exception>
    Task<SabatexValidationModel<TItem1>> ISabatexRadzenBlazorDataAdapter.UpdateAsync<TItem1, TKey1>(TItem1 item)
    {
        throw new NotImplementedException();
    }
    /// <summary>
    /// Asynchronously deletes the item with the specified identifier.
    /// </summary>
    /// <typeparam name="TItem1">The type of the item to delete.</typeparam>
    /// <typeparam name="TKey1">The type of the identifier for the item.</typeparam>
    /// <param name="id">The identifier of the item to delete.</param>
    /// <returns>A task that represents the asynchronous delete operation.</returns>
    /// <exception cref="NotImplementedException">The method is not implemented.</exception>
    Task ISabatexRadzenBlazorDataAdapter.DeleteAsync<TItem1, TKey1>(TKey1 id)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Provides a server-side data adapter for managing entities using a DbContext and supporting dynamic discovery of data
/// adapter types. Implements the ISabatexRadzenBlazorDataAdapter interface to enable CRUD operations and query support
/// for various entity types.
/// </summary>
/// <remarks>This class enables dynamic resolution and invocation of data adapters for different entity and key
/// type combinations at runtime. It is designed to work with Entity Framework Core DbContext and supports extensibility
/// by discovering adapter types that inherit from DataAdapterBase<,> within the assembly. Thread safety depends on the
/// underlying DbContext and logger implementations. Typically, a single instance of SabatexDataAdapterServerSide is
/// created per request or per application, depending on the lifetime of the DbContext provided.</remarks>
public  class SabatexDataAdapterServerSide : ISabatexRadzenBlazorDataAdapter
{
    protected readonly DbContext Db;
    protected readonly ILogger Logger;

    private readonly IServiceProvider _sp;
    private static readonly Dictionary<(Type, Type), Type> _map;
    /// <summary>
    /// Discovers all non-abstract types in the specified assembly that derive from DataAdapterBase<,> and returns a
    /// mapping of their item and key types to the adapter type.
    /// </summary>
    /// <remarks>Only non-abstract types whose direct base type is a closed generic of DataAdapterBase<,> are
    /// included. This method can be used to dynamically discover available data adapters for specific item and key type
    /// combinations.</remarks>
    /// <param name="assembly">The assembly to search for adapter types that inherit from DataAdapterBase<,>.</param>
    /// <returns>A dictionary mapping each (item type, key type) pair to the corresponding adapter type found in the assembly.
    /// The dictionary is empty if no matching adapters are found.</returns>
    public static Dictionary<(Type Item, Type Key), Type> DiscoverAdapters(Assembly assembly)
    {
        var result = new Dictionary<(Type, Type), Type>();

        var adapterTypes = assembly.GetTypes()
            .Where(t => !t.IsAbstract && t.BaseType != null && t.BaseType.IsGenericType
                        && t.BaseType.GetGenericTypeDefinition() == typeof(DataAdapterBase<,>));

        foreach (var t in adapterTypes)
        {
            var args = t.BaseType.GetGenericArguments();
            var itemType = args[0];
            var keyType = args[1];
            result[(itemType, keyType)] = t;
        }

        return result;
    }

    static SabatexDataAdapterServerSide()
    {
        // один раз при старті збірки
        _map = DiscoverAdapters(typeof(SabatexDataAdapterServerSide).Assembly);
    }
    /// <summary>
    /// Initializes a new instance of the SabatexDataAdapterServerSide class with the specified database context and
    /// logger.
    /// </summary>
    /// <param name="db">The database context to be used for data operations. Cannot be null.</param>
    /// <param name="logger">The logger instance used to record diagnostic and operational information. Cannot be null.</param>
    public SabatexDataAdapterServerSide(DbContext db, ILogger logger)
    {
        Db = db;
        Logger = logger;
    }

    private ISabatexRadzenBlazorDataAdapter Resolve<TItem, TKey>()
        where TItem : class, IEntityBase<TKey>
    {
        if (_map.TryGetValue((typeof(TItem), typeof(TKey)), out var adapterType))
        {
            return (ISabatexRadzenBlazorDataAdapter)ActivatorUtilities.CreateInstance(_sp, adapterType);
        }

        throw new InvalidOperationException(
            $"Не знайдено адаптер для {typeof(TItem).Name} з ключем {typeof(TKey).Name}");
    }
    /// <summary>
    /// Asynchronously retrieves a collection of items that match the specified query parameters.
    /// </summary>
    /// <typeparam name="TItem">The type of the items to retrieve. Must implement IEntityBase<TKey>.</typeparam>
    /// <typeparam name="TKey">The type of the key for the items.</typeparam>
    /// <param name="queryParams">The parameters that define the query criteria and options for retrieving items.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a QueryResult<TItem> with the items
    /// that match the query.</returns>
    public Task<QueryResult<TItem>> GetAsync<TItem, TKey>(QueryParams queryParams)
        where TItem : class, IEntityBase<TKey>
        => Resolve<TItem, TKey>().GetAsync<TItem, TKey>(queryParams);
    /// <summary>
    /// Asynchronously retrieves an entity by its unique identifier, with optional expansion of related entities.
    /// </summary>
    /// <typeparam name="TItem">The type of the entity to retrieve. Must implement IEntityBase<TKey>.</typeparam>
    /// <typeparam name="TKey">The type of the unique identifier for the entity.</typeparam>
    /// <param name="id">The unique identifier of the entity to retrieve.</param>
    /// <param name="expand">A comma-separated list of related entities to include in the result, or null to retrieve only the main entity.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the entity if found; otherwise,
    /// null.</returns>
    public Task<TItem?> GetByIdAsync<TItem, TKey>(TKey id, string? expand = null)
        where TItem : class, IEntityBase<TKey>
        => Resolve<TItem, TKey>().GetByIdAsync<TItem, TKey>(id, expand);
    /// <summary>
    /// Asynchronously retrieves an entity by its unique identifier.
    /// </summary>
    /// <typeparam name="TItem">The type of the entity to retrieve. Must implement IEntityBase<TKey>.</typeparam>
    /// <typeparam name="TKey">The type of the entity's unique identifier.</typeparam>
    /// <param name="id">The unique identifier of the entity to retrieve. Cannot be null.</param>
    /// <param name="expand">An optional comma-separated list of related entities to include in the result. If null, no related entities are
    /// expanded.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the entity if found; otherwise,
    /// null.</returns>
    public Task<TItem?> GetByIdAsync<TItem, TKey>(string id, string? expand = null)
        where TItem : class, IEntityBase<TKey>
        => Resolve<TItem, TKey>().GetByIdAsync<TItem, TKey>(id, expand);
    /// <summary>
    /// Creates a new entity by sending the specified item to the underlying data store asynchronously.
    /// </summary>
    /// <typeparam name="TItem">The type of the entity to create. Must implement IEntityBase<TKey>.</typeparam>
    /// <typeparam name="TKey">The type of the key for the entity.</typeparam>
    /// <param name="item">The entity to create. Can be null if the implementation allows creating an empty or default entity.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a SabatexValidationModel<TItem> with
    /// the created entity and any validation results.</returns>
    public Task<SabatexValidationModel<TItem>> PostAsync<TItem, TKey>(TItem? item)
        where TItem : class, IEntityBase<TKey>
        => Resolve<TItem, TKey>().PostAsync(item);
    /// <summary>
    /// Asynchronously updates the specified entity in the data store.
    /// </summary>
    /// <typeparam name="TItem">The type of the entity to update. Must implement IEntityBase<TKey>.</typeparam>
    /// <typeparam name="TKey">The type of the entity's key.</typeparam>
    /// <param name="item">The entity instance to update. Cannot be null.</param>
    /// <returns>A task that represents the asynchronous update operation. The task result contains a
    /// SabatexValidationModel<TItem> with the updated entity and any validation results.</returns>
    public Task<SabatexValidationModel<TItem>> UpdateAsync<TItem, TKey>(TItem item)
        where TItem : class, IEntityBase<TKey>
        => Resolve<TItem, TKey>().UpdateAsync(item);
    /// <summary>
    /// Asynchronously deletes the entity with the specified identifier from the data store.
    /// </summary>
    /// <typeparam name="TItem">The type of the entity to delete. Must implement IEntityBase<TKey>.</typeparam>
    /// <typeparam name="TKey">The type of the entity's identifier.</typeparam>
    /// <param name="id">The identifier of the entity to delete.</param>
    /// <returns>A task that represents the asynchronous delete operation.</returns>
    public Task DeleteAsync<TItem, TKey>(TKey id)
        where TItem : class, IEntityBase<TKey>
        => Resolve<TItem, TKey>().DeleteAsync<TItem, TKey>(id);
}

