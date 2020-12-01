namespace Fux.Config.RedisHelper.Attribute
{
    /// <summary>
    /// This class maintains the RedisKey attribute for classes
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Field | System.AttributeTargets.Property | System.AttributeTargets.Struct)]
    public class RedisKeyAttribute : Fux.Core.Attribute.FromKeyAttribute
    {
        /// <summary>
        /// This method instantiates the attribute class with a key name
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="allowEmptyValue"></param>
        public RedisKeyAttribute(string redisKey, bool allowEmptyValue = true) : base(redisKey, allowEmptyValue) { }
    }
}
