using System.Collections.Generic;

namespace Fux.Data.Mongo.Abstract
{
    /// <summary>
    /// This class provides an abstract configuration base
    /// </summary>
    public class Configuration : IConfiguration
    {
        /// <summary>
        /// This property contains the name of the authentication database
        /// </summary>
        /// <value>admin</value>
        public string AuthenticationDatabase { get; set; }

        /// <summary>
        /// This property contains the authentication password
        /// </summary>
        /// <value>null</value>
        public string AuthenticationPassword { get; set; }

        /// <summary>
        /// This property contains the authentication username
        /// </summary>
        /// <value>null</value>
        public string AuthenticationUsername { get; set; }

        /// <summary>
        /// This property contains the MongoDB database that the context is responsible for
        /// </summary>
        /// <value>null</value>
        public string Database { get; set; }

        /// <summary>
        /// This property contains a CSV of hostname in the format of <code>[IP[:PORT]]</code> or <code>[HOST[:PORT]]</code>
        /// </summary>
        /// <value></value>
        public List<string> Hosts { get; set; }

        /// <summary>
        /// This property tells the engine whether or not to dump MongoDB commands to the console
        /// </summary>
        /// <value>false</value>
        public bool LogCommands { get; set; }

        /// <summary>
        /// This property contains the max number of connections to allow the context to open
        /// </summary>
        /// <value>1000</value>
        public int? MaximumConnections { get; set; }
    }
}
