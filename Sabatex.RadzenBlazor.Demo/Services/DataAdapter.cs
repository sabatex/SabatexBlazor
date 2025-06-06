using Radzen;
using Radzen.Blazor.Markdown;
using Sabatex.Core;
using Sabatex.RadzenBlazor.Demo.Models;
using System.Collections;

namespace Sabatex.RadzenBlazor.Demo.Services;
public class DataAdapter : ISabatexRadzenBlazorDataAdapter<Guid>
{
    static Dictionary<Type,Dictionary<Guid,IEntityBase<Guid>>> dtaBase = new Dictionary<Type, Dictionary<Guid, IEntityBase<Guid>>>
    {
        { typeof(Person),  new Dictionary<Guid, IEntityBase<Guid>>() }
    };
    async Task ISabatexRadzenBlazorDataAdapter<Guid>.DeleteAsync<TItem>(Guid id)
    {
        await Task.Yield(); // Simulate async operation
        var table = dtaBase[typeof(TItem)];
        if (table.ContainsKey(id))
        {
            table.Remove(id);
        }
        else
        {
            throw new Exception("Field no exist");
        }
    }

    Task<ODataServiceResult<TItem>> ISabatexRadzenBlazorDataAdapter<Guid>.GetAsync<TItem>(string? filter, string? orderby, string? expand, int? top, int? skip, bool? count, string? format, string? select, string? apply)
    {
        throw new NotImplementedException();
    }

    Task<ODataServiceResult<TItem>> ISabatexRadzenBlazorDataAdapter<Guid>.GetAsync<TItem>(QueryParams queryParams)
    {
        throw new NotImplementedException();
    }

    Task<TItem?> ISabatexRadzenBlazorDataAdapter<Guid>.GetByIdAsync<TItem>(Guid id, string? expand) where TItem : class
    {
        throw new NotImplementedException();
    }

    Task<TItem?> ISabatexRadzenBlazorDataAdapter<Guid>.GetByIdAsync<TItem>(string id, string? expand) where TItem : class
    {
        throw new NotImplementedException();
    }

    async Task<SabatexValidationModel<TItem>> ISabatexRadzenBlazorDataAdapter<Guid>.PostAsync<TItem>(TItem? item) where TItem : class
    {
        await Task.Yield(); // Simulate async operation
        var table = dtaBase[typeof(TItem)];
        table.Add(item.Id, item);
        return new SabatexValidationModel<TItem>(item, null);
    }

    Task<SabatexValidationModel<TItem>> ISabatexRadzenBlazorDataAdapter<Guid>.UpdateAsync<TItem>(TItem item)
    {
        throw new NotImplementedException();
    }
}