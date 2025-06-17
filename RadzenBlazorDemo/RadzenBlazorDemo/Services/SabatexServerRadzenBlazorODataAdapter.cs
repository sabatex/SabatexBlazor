using Microsoft.EntityFrameworkCore;
using Radzen;
using RadzenBlazorDemo.Data;
using Sabatex.Core;
using Sabatex.Core.RadzenBlazor;
using Sabatex.RadzenBlazor;


namespace RadzenBlazorDemo.Services;

public class SabatexServerRadzenBlazorODataAdapter : ISabatexRadzenBlazorDataAdapter<Guid>
{
    private readonly ApplicationDbContext context;
    public SabatexServerRadzenBlazorODataAdapter(ApplicationDbContext context)
    {
        this.context = context;
    }
    public Task DeleteAsync<TItem>(Guid id) where TItem : class,Sabatex.Core.IEntityBase<Guid>
    {
        throw new NotImplementedException();
    }

    public async Task<QueryResult<TItem>> GetAsync<TItem>(string? filter, string? orderby, string? expand, int? top, int? skip, bool? count, string? format = null, string? select = null, string? ee = null) where TItem : class, IEntityBase<Guid>
    {
        var result = context.Set<TItem>().AsQueryable();
        return new QueryResult<TItem> { Count = 0,Value = await result.ToArrayAsync() };
    }

    public Task<string[]> GetAvaliableRolesAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<TItem?> GetByIdAsync<TItem>(Guid id, string? expand = null) where TItem : class, Sabatex.Core.IEntityBase<Guid>
    {
        await Task.Yield();
        return null;
    }

    public Task<TItem?> GetByIdAsync<TItem>(string id, string? expand = null) where TItem : class, Sabatex.Core.IEntityBase<Guid>
    {
        return null;
    }

    public Task<SabatexValidationModel<TItem>> PostAsync<TItem>(TItem? item) where TItem : class, Sabatex.Core.IEntityBase<Guid>
    {
        throw new NotImplementedException();
    }

    public Task<SabatexValidationModel<TItem>> UpdateAsync<TItem>(TItem item) where TItem : class, Sabatex.Core.IEntityBase<Guid>
    {
        throw new NotImplementedException();
    }

    Task<QueryResult<TItem>> ISabatexRadzenBlazorDataAdapter<Guid>.GetAsync<TItem>(Sabatex.Core.RadzenBlazor.QueryParams queryParams)
    {
        return null;
        throw new NotImplementedException();
    }
}
