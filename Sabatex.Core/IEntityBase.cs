namespace Sabatex.Core;

/// <summary>
/// Base entity methods for (api,ef)
/// </summary>
public interface IEntityBase<TKey>
{
    /// <summary>
    /// Get primary key as string (int,long,Guid,string)
    /// </summary>
    TKey Id { get; set; }
}