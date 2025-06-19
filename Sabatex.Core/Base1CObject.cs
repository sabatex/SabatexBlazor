using Sabatex.Core;

namespace sabatex.BakeryWebApp.Models;

// <summary>
/// Base abstract class that provides a primary key and soft deletion support.
/// Common ancestor for all 1C-compatible business entities.
/// </summary>
public abstract class Base1CObject : IEntityBase<Guid>, IDeletionMark
{
    /// <summary>
    /// Unique identifier of the entity.
    /// Serves as the primary key and external reference.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Alias for <see cref="Id"/> used for compatibility with 1C systems
    /// where the primary reference is referred to as "Ref".
    /// </summary>
    public Guid Ref => Id;

    /// <summary>
    /// Soft deletion marker.
    /// If <c>true</c>, the entity is considered logically deleted but still stored in the database.
    /// </summary>
    public bool DeletionMark { get; set; }
}
