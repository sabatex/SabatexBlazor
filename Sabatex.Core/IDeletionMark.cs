namespace Sabatex.Core;
/// <summary>
/// Defines an entity that supports logical (soft) deletion via a deletion mark.
/// </summary>
public interface IDeletionMark
{
    /// <summary>
    /// Indicates whether the record is marked as deleted (invisible to the user).
    /// </summary>
    public bool DeletionMark { get; set; }
}