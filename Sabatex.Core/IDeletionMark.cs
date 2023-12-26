namespace Sabatex.Core;
/// <summary>
/// Entity object support mark object as deleted
/// </summary>
public interface IDeletionMark
{
       /// <summary>
       /// Mark record for delete (user invisble )
       /// </summary>
       public bool DeletionMark { get; set; }
}