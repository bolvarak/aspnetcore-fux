using System;

namespace Fux.Core.Attribute
{
    /// <summary>
    /// This attribute maintains the the correlation between convertible objects
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class ToAttribute : System.Attribute
    {
        /// <summary>
        /// This property contains the type to convert to
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// This method instantiates the attribute with a to-type
        /// </summary>
        /// <param name="type"></param>
        public ToAttribute(Type type) => Type = type;

        /// <summary>
        /// This method instantiates the attribute with a to-type from its name
        /// </summary>
        /// <param name="typeName"></param>
        public ToAttribute(string typeName) => Type = Type.GetType(typeName);
    }
}
