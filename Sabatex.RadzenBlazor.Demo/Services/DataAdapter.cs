using Radzen;
using Radzen.Blazor.Markdown;
using Sabatex.Core;
using Sabatex.Core.RadzenBlazor;
using Sabatex.RadzenBlazor.Demo.Models;
using System.Collections;

namespace Sabatex.RadzenBlazor.Demo.Services;
public class DataAdapter : ISabatexRadzenBlazorDataAdapter
{
    static Dictionary<Type,Dictionary<Guid,IEntityBase<Guid>>> dtaBase = new Dictionary<Type, Dictionary<Guid, IEntityBase<Guid>>>
    {
        { typeof(Person),  new Dictionary<Guid, IEntityBase<Guid>>() }
    };
    async Task ISabatexRadzenBlazorDataAdapter.DeleteAsync<TItem, TKey>(TKey id)
    {
        await Task.Yield(); // Simulate async operation
        var table = dtaBase[typeof(TItem)];
        var Id = (Guid)(object)id;
        if (table.ContainsKey(Id))
        {
            table.Remove(Id);
        }
        else
        {
            throw new Exception("Field no exist");
        }
    }

 
 
    Task<QueryResult<TItem>> ISabatexRadzenBlazorDataAdapter.GetAsync<TItem, TKey>(QueryParams queryParams)
    {
        throw new NotImplementedException();
    }

    Task<TItem?> ISabatexRadzenBlazorDataAdapter.GetByIdAsync<TItem, Guid>(Guid id, string? expand) where TItem : class
    {
        throw new NotImplementedException();
    }

    Task<TItem?> ISabatexRadzenBlazorDataAdapter.GetByIdAsync<TItem,TKey>(string id, string? expand) where TItem : class
    {
        throw new NotImplementedException();
    }

    async Task<SabatexValidationModel<TItem>> ISabatexRadzenBlazorDataAdapter.PostAsync<TItem, TKey>(TItem? item) where TItem : class
    {
        await Task.Yield();
        var table = dtaBase[typeof(TItem)];
        table.Add((Guid)(object)item.Id, item as IEntityBase<Guid>);
        return new SabatexValidationModel<TItem>(item, null);
    }

    //async Task<SabatexValidationModel<TItem>> ISabatexRadzenBlazorDataAdapter.PostAsync<TItem, TKey>(TItem? item) where TItem : class,IEntityBase<TKey>
    //{
    //    await Task.Yield(); // Simulate async operation
    //    var table = dtaBase[typeof(TItem)];
    //    table.Add(item.Id, item);
    //    return new SabatexValidationModel<TItem>(item, null);
    //}


    Task<SabatexValidationModel<TItem>> ISabatexRadzenBlazorDataAdapter.UpdateAsync<TItem, TKey>(TItem item)
    {
        throw new NotImplementedException();
    }
}