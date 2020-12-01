using System;

namespace Fux.Core.Attribute
{
    /// <summary>
    /// This attribute maintains the correlation between properties when mapping from other objects
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class FromPropertyAttribute : System.Attribute
    {
        /// <summary>
        /// This property tells the reflector whether or not the value
        /// can be empty
        /// </summary>
        public bool AllowEmpty { get; set; } = true;

        /// <summary>
        /// This property contains the name of the property on the target object to map from
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// This method instantiates the attribute with a <paramref name="propertyName"/> to map from
        /// with the option to require a value with <paramref name="allowEmptyValue"/> set to <code>false</code>
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="allowEmptyValue"></param>
        public FromPropertyAttribute(string propertyName, bool allowEmptyValue = true)
        {
            // Set the empty flag into the instance
            AllowEmpty = allowEmptyValue;
            // Set the property name into the instance
            Name = propertyName;
        }
    }
}
