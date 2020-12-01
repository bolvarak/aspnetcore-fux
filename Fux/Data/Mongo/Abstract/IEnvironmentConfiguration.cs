using System.Collections.Generic;
using Fux.Config.EnvironmentHelper.Attribute;

namespace Fux.Data.Mongo.Abstract
{
    /// <summary>
    /// This interface maintains the structure for a MongoDB configuration object from the Environment
    /// </summary>
    public interface IEnvironmentConfiguration : IConfiguration
    {
        /// <summary>
        /// This property contains the name of the authentication database
        /// </summary>
        /// <value>admin</value>
        [EnvironmentVariable("FUX_DB_MONGO_AUTH_DBNAME")]
        public new string AuthenticationDatabase { get; set; }

        /// <summary>
        /// This property contains the authentication password
        /// </summary>
        /// <value>null</value>
        [EnvironmentVariable("FUX_DB_MONGO_AUTH_PASS")]
        public new string AuthenticationPassword { get; set; }

        /// <summary>
        /// This property contains the authentication username
        /// </summary>
        /// <value>null</value>
        [EnvironmentVariable("FUX_DB_MONGO_AUTH_USER")]
        public new string AuthenticationUsername { get; set; }

        /// <summary>
        /// This property contains the MongoDB database that the context is responsible for
        /// </summary>
        /// <value>null</value>
        [EnvironmentVariable("FUX_DB_MONGO_DBNAME")]
        public new string Database { get; set; }

        /// <summary>
        /// This property contains a CSV of hostname in the format of <code>[IP[:PORT],...]</code> or <code>[HOST[:PORT],...]</code>
        /// </summary>
        /// <value></value>
        [EnvironmentVariable("FUX_DB_MONGO_HOSTS")]
        public new List<string> Hosts { get; set; }

        /// <summary>
        /// This property tells the engine whether or not to dump MongoDB commands to the console
        /// </summary>
        /// <value>false</value>
        [EnvironmentVariable("FUX_DB_MONGO_LOG_COMMANDS")]
        public new bool LogCommands { get; set; }

        /// <summary>
        /// This property contains the max number of connections to allow the context to open
        /// </summary>
        /// <value>1000</value>
        [EnvironmentVariable("FUX_DB_MONGO_MAX_CONN")]
        public new int? MaximumConnections { get; set; }
    }
}
