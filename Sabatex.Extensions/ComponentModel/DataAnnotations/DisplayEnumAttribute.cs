using System;
using System.Globalization;
using System.Collections.Generic;
using System.Text;

namespace sabatex.Extensions.ComponentModel.DataAnnotations
{
#if NET6_0_OR_GREATER
    /// <summary>
    ///     DisplayEnumAttribute is a general-purpose attribute to specify user-visible globalizable strings for enum.
    ///     The string properties of this class can be used either as literals or as resource identifiers into a specified
    /// </summary>
    [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field ,AllowMultiple=false) ]
    public sealed class DisplayEnumAttribute:Attribute
    {
        #region Member Fields 
        private readonly LocalizableString _name = new LocalizableString("Name");
        private Type _resourceType;
        #endregion

        #region All Constructors

        #endregion

        #region Properties
        
        /// <summary>
        ///     Gets or sets the Name attribute property, which may be a resource key string.
        ///     <para>
        ///         Consumers must use the <see cref="GetName" /> method to retrieve the UI display string.
        ///     </para>
        /// </summary>
        /// <remarks>
        ///     The property contains either the literal, non-localized string or the resource key
        ///     to be used in conjunction with <see cref="ResourceType" /> to configure a localized
        ///     name for display.
        ///     <para>
        ///         The <see cref="GetName" /> method will return either the literal, non-localized
        ///         string or the localized string when <see cref="ResourceType" /> has been specified.
        ///     </para>
        /// </remarks>
        /// <value>
        ///     The name is generally used as the field label for a UI element bound to the member
        ///     bearing this attribute.  A <c>null</c> or empty string is legal, and consumers must allow for that.
        /// </value>
        public string Name { get => _name.Value; set=>_name.Value=value; }
        /// <summary>
        ///     Gets or sets the <see cref="System.Type" /> that contains the resources for <see cref="Name" />.
        ///     Using <see cref="ResourceType" /> along with these Key properties, allows the <see cref="GetName" />
        ///     methods to return localized values.
        /// </summary>
        public Type ResourceType
        {
            get => _resourceType;
            set
            {
                if (_resourceType != value)
                {
                    _resourceType = value;

                    _name.ResourceType = value;
                }
            }
        }
        #endregion
        
        #region Methods
        /// <summary>
        ///     Gets the UI display string for Name.
        ///     <para>
        ///         This can be either a literal, non-localized string provided to <see cref="Name" /> or the
        ///         localized string found when <see cref="ResourceType" /> has been specified and <see cref="Name" />
        ///         represents a resource key within that resource type.
        ///     </para>
        /// </summary>
        /// <returns>
        ///     When <see cref="ResourceType" /> has not been specified, the value of
        ///     <see cref="Name" /> will be returned.
        ///     <para>
        ///         When <see cref="ResourceType" /> has been specified and <see cref="Name" />
        ///         represents a resource key within that resource type, then the localized value will be returned.
        ///     </para>
        ///     <para>
        ///         Can return <c>null</c> and will not fall back onto other values, as it's more likely for the
        ///         consumer to want to fall back onto the property name.
        ///     </para>
        /// </returns>
        /// <exception cref="System.InvalidOperationException">
        ///     After setting both the <see cref="ResourceType" /> property and the <see cref="Name" /> property,
        ///     but a public static property with a name matching the <see cref="Name" /> value couldn't be found
        ///     on the <see cref="ResourceType" />.
        /// </exception>
        public string GetName() => _name.GetLocalizableValue();
        #endregion
    }
#endif
}
