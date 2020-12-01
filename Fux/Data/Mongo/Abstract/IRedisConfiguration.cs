using System.Collections.Generic;
using Fux.Config.RedisHelper.Attribute;

namespace Fux.Data.Mongo.Abstract
{
    /// <summary>
    /// This interface maintains the structure for a MongoDB configuration object from Redis
    /// </summary>
    public interface IRedisConfiguration : IConfiguration
    {
        /// <summary>
        /// This property contains the name of the authentication database
        /// </summary>
        /// <value>admin</value>
        [RedisKey("fux-db-mongo-auth-dbname")]
        public new string AuthenticationDatabase { get; set; }

        /// <summary>
        /// This property contains the authentication password
        /// </summary>
        /// <value>null</value>
        [RedisKey("fux-db-mongo-auth-pass")]
        public new string AuthenticationPassword { get; set; }

        /// <summary>
        /// This property contains the authentication username
        /// </summary>
        /// <value>null</value>
        [RedisKey("fux-db-mongo-auth-user")]
        public new string AuthenticationUsername { get; set; }

        /// <summary>
        /// This property contains the MongoDB database that the context is responsible for
        /// </summary>
        /// <value>null</value>
        [RedisKey("fux-db-mongo-dbname")]
        public new string Database { get; set; }

        /// <summary>
        /// This property contains a CSV of hostname in the format of <code>[IP[:PORT]]</code> or <code>[HOST[:PORT]]</code>
        /// </summary>
        /// <value></value>
        [RedisKey("fux-db-mongo-hosts")]
        public new List<string> Hosts { get; set; }

        /// <summary>
        /// This property tells the engine whether or not to dump MongoDB commands to the console
        /// </summary>
        /// <value>false</value>
        [RedisKey("fux-db-mongo-log-commands")]
        public new bool LogCommands { get; set; }

        /// <summary>
        /// This property contains the max number of connections to allow the context to open
        /// </summary>
        /// <value>1000</value>
        [RedisKey("fux-db-mongo-max-conn")]
        public new int? MaximumConnections { get; set; }
    }
}
