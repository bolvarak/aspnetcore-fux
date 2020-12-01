using System;

namespace Fux.Core.Attribute
{
    /// <summary>
    /// This attribute maintains the the correlation between convertible objects
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class FromAttribute : System.Attribute
    {
        /// <summary>
        /// This property contains the type to convert from
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// This method instantiates the attribute with a from-type
        /// </summary>
        /// <param name="type"></param>
        public FromAttribute(Type type) => Type = type;

        /// <summary>
        /// This method instantiates the attribute with a from-type from its name
        /// </summary>
        /// <param name="typeName"></param>
        public FromAttribute(string typeName) => Type = Type.GetType(typeName);
    }
}
