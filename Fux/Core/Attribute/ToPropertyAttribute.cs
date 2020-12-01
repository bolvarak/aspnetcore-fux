using System;

namespace Fux.Core.Attribute
{
    /// <summary>
    /// This attribute maintains the correlation between properties when mapping to other objects
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class ToPropertyAttribute : System.Attribute
    {
        /// <summary>
        /// This property tells the system whether or not to check the value for null
        /// </summary>
        public bool AllowEmpty { get; set; } = true;

        /// <summary>
        /// This property contains the name of the property on the target object to map to
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// This method instantiates the attribute with a <paramref name="propertyName"/> to map to
        /// with the option to throw an exception if the value is empty with <paramref name="allowEmptyValue"/>
        /// set to <code>false</code>
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="allowEmptyValue"></param>
        public ToPropertyAttribute(string propertyName, bool allowEmptyValue = true)
        {
            // Set the empty flag into the instance
            AllowEmpty = allowEmptyValue;
            // Set the name into the instance
            Name = propertyName;
        }
    }
}
