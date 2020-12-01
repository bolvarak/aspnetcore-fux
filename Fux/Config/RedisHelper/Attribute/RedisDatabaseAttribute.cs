namespace Fux.Config.RedisHelper.Attribute
{
    /// <summary>
    /// This class maintains the RedisDatabase attribute for classes
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
    public class RedisDatabaseAttribute : System.Attribute
    {
        /// <summary>
        /// This property contains the database index to connect to in Redis
        /// </summary>
        /// <value></value>
        public int Database { get; set; } = -1;

        /// <summary>
        /// This method instantiates the attribute class with a database index
        /// </summary>
        /// <param name="database"></param>
        public RedisDatabaseAttribute(int database) =>
            Database = database;
    }
}
