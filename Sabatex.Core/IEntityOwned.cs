namespace Sabatex.Core;
/// <summary>
/// 
/// </summary>
/// <typeparam name="TKey"></typeparam>
public interface IEntityOwned<TKey>
{
    /// <summary>
    /// Reference to parent item
    /// </summary>
    public TKey Owner { get; set; }
}
/// <summary>
/// 
/// </summary>
public class EntityOwnerAttribute<TEntity> : Attribute 
{
/// <summary>
/// owner type 
/// </summary>
    public Type? OwnerType { get; set; } 
}