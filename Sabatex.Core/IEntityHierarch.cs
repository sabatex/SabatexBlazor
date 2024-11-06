namespace Sabatex.Core;
/// <summary>
/// 
/// </summary>
/// <typeparam name="TKey"></typeparam>
public interface IEntityHierarch<TKey>
{
    /// <summary>
    /// Reference to parent item
    /// </summary>
    public TKey? Parent { get; set; }
}