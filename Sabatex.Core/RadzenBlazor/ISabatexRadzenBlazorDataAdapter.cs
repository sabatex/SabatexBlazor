using Sabatex.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sabatex.Core.RadzenBlazor;

namespace Sabatex.Core.RadzenBlazor;

public interface ISabatexRadzenBlazorDataAdapter<TKey>
{
    Task<ODataServiceResult<TItem>> GetAsync<TItem>(string? filter, string? orderby, string? expand, int? top, int? skip, bool? count, string? format=null, string? select=null, string? apply = null) where TItem : class,IEntityBase<TKey>;
    Task<ODataServiceResult<TItem>> GetAsync<TItem>(QueryParams queryParams) where TItem : class, IEntityBase<TKey>;
    Task<TItem?> GetByIdAsync<TItem>(TKey id, string? expand=null) where TItem: class, IEntityBase<TKey>;
    Task<TItem?> GetByIdAsync<TItem>(string id, string? expand = null) where TItem : class, IEntityBase<TKey>;

    Task<SabatexValidationModel<TItem>> PostAsync<TItem>(TItem? item) where TItem : class, IEntityBase<TKey>;
    Task<SabatexValidationModel<TItem>> UpdateAsync<TItem>(TItem item) where TItem : class, IEntityBase<TKey>;

    Task DeleteAsync<TItem>(TKey id) where TItem : class, IEntityBase<TKey>;
    
}

public record struct  FieldDescriptor(string Name,Type FieldType,string operation=" or ",int priority = 0,string? value=null);

public record SabatexValidationModel<TItem>(TItem? Result, Dictionary<string, List<string>>? Errors=null);

public class DeserializeException : Exception
{
    public DeserializeException() : base("Deserialize object error.") { }
}