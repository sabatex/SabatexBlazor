using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;


namespace Sabatex.Core
{
    /// <summary>
    /// for primary key base class 
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public abstract class EntityBase<TKey>:IEntityBase<TKey>
    {
        /// <summary>
        /// Primary key
        /// </summary>
        public TKey Id { get; set; } = default!;
    }
}
