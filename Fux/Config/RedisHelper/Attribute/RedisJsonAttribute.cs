using System;

namespace Fux.Config.RedisHelper.Attribute
{
    /// <summary>
    /// This attribute maintains the typed de/serialization of JSON objects in Redis
    /// </summary>
    public class RedisJsonAttribute : Fux.Core.Attribute.FromAttribute
    {
        /// <summary>
        /// This method instantiates the attribute with a JSON type
        /// </summary>
        /// <param name="jsonType"></param>
        public RedisJsonAttribute(Type jsonType) : base(jsonType) { }
    }
}
