using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sabatex.Core;
using Sabatex.Core.RadzenBlazor;
using System.Reflection;

namespace Sabatex.RadzenBlazor.Server;

public abstract class DataAdapterBase<TItem, TKey> : ISabatexRadzenBlazorDataAdapter 
    where TItem : class, IEntityBase<TKey>
{
    protected readonly DbContext Db;
    protected readonly ILogger Logger;

    protected DataAdapterBase(DbContext db, ILogger logger)
    {
        Db = db;
        Logger = logger;
    }

    Task<QueryResult<TItem>> ISabatexRadzenBlazorDataAdapter.GetAsync<TItem, TKey>(QueryParams queryParams)
    {
        throw new NotImplementedException();
    }

    Task<TItem> ISabatexRadzenBlazorDataAdapter.GetByIdAsync<TItem, TKey>(TKey id, string? expand)
    {
        throw new NotImplementedException();
    }

    Task<TItem1> ISabatexRadzenBlazorDataAdapter.GetByIdAsync<TItem1, TKey1>(string id, string? expand)
    {
        throw new NotImplementedException();
    }

    Task<SabatexValidationModel<TItem1>> ISabatexRadzenBlazorDataAdapter.PostAsync<TItem1, TKey1>(TItem1 item)
    {
        throw new NotImplementedException();
    }

    Task<SabatexValidationModel<TItem1>> ISabatexRadzenBlazorDataAdapter.UpdateAsync<TItem1, TKey1>(TItem1 item)
    {
        throw new NotImplementedException();
    }

    Task ISabatexRadzenBlazorDataAdapter.DeleteAsync<TItem1, TKey1>(TKey1 id)
    {
        throw new NotImplementedException();
    }
}


public  class SabatexDataAdapterServerSide : ISabatexRadzenBlazorDataAdapter
{
    protected readonly DbContext Db;
    protected readonly ILogger Logger;

    private readonly IServiceProvider _sp;
    private static readonly Dictionary<(Type, Type), Type> _map;
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

    public Task<QueryResult<TItem>> GetAsync<TItem, TKey>(QueryParams queryParams)
        where TItem : class, IEntityBase<TKey>
        => Resolve<TItem, TKey>().GetAsync<TItem, TKey>(queryParams);

    public Task<TItem?> GetByIdAsync<TItem, TKey>(TKey id, string? expand = null)
        where TItem : class, IEntityBase<TKey>
        => Resolve<TItem, TKey>().GetByIdAsync<TItem, TKey>(id, expand);

    public Task<TItem?> GetByIdAsync<TItem, TKey>(string id, string? expand = null)
        where TItem : class, IEntityBase<TKey>
        => Resolve<TItem, TKey>().GetByIdAsync<TItem, TKey>(id, expand);

    public Task<SabatexValidationModel<TItem>> PostAsync<TItem, TKey>(TItem? item)
        where TItem : class, IEntityBase<TKey>
        => Resolve<TItem, TKey>().PostAsync(item);

    public Task<SabatexValidationModel<TItem>> UpdateAsync<TItem, TKey>(TItem item)
        where TItem : class, IEntityBase<TKey>
        => Resolve<TItem, TKey>().UpdateAsync(item);

    public Task DeleteAsync<TItem, TKey>(TKey id)
        where TItem : class, IEntityBase<TKey>
        => Resolve<TItem, TKey>().DeleteAsync<TItem, TKey>(id);
}

