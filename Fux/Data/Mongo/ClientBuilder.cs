using System;
using System.Collections.Generic;
using System.Linq;
using Fux.Config;
using Fux.Config.DockerHelper.Attribute;
using Fux.Config.EnvironmentHelper.Attribute;
using Fux.Config.RedisHelper.Attribute;
using Fux.Data.Mongo.Abstract;
using MongoDB.Driver;
using Environment = Fux.Config.Environment;

namespace Fux.Data.Mongo
{
    // Define our type aliases
    using IpAndPort = System.Tuple<string, int?, Access, HostType>;

    /// <summary>
    /// This enumeration defines our access values for the servers
    /// </summary>
    public enum Access
    {
        /// <summary>
        /// /// This value defines read-only access (ReadReplica)
        /// </summary>
        Read,

        /// <summary>
        /// This property defines read and write access (Master)
        /// </summary>
        ReadWrite
    }

    /// <summary>
    /// This enumeration defines our host types
    /// </summary>
    public enum HostType
    {
        /// <summary>
        /// This value defines a TCP host
        /// </summary>
        Tcp,

        /// <summary>
        /// This value defines a UNIX Domain Socket host
        /// </summary>
        Unix
    }

    /// <summary>
    /// This class maintains our MongoDB client connection builder
    /// with a fluid interface
    /// </summary>
    public class ClientBuilder
    {
        /// <summary>
        /// This property contains the authentication database
        /// </summary>
        protected string AuthDatabase = "admin";

        /// <summary>
        /// This property contains the authentication password
        /// </summary>
        protected string AuthPassword;

        /// <summary>
        /// This property contains the authentication username
        /// </summary>
        protected string AuthUsername;

        /// <summary>
        /// This property contains our hosts and their ports
        /// </summary>
        protected List<IpAndPort> Hosts = new List<IpAndPort>();

        /// <summary>
        /// This property contains the maximum number of connections for the pool
        /// </summary>
        protected int MaxConnections = 250;

        /// <summary>
        /// This property contains the replica set name
        /// </summary>
        protected string ReplicaSetName;

        /// <summary>
        /// This method provides a factory for instantiating a MongoDB connection from Docker Secrets or Environment Variables
        /// </summary>
        /// <typeparam name="TConfiguration"></typeparam>
        /// <returns></returns>
        public static ClientBuilder FromConfiguration<TConfiguration>() where TConfiguration : Configuration, new()
        {
            // Check the configuration type for Docker Secrets
            if (typeof(TConfiguration) == typeof(IDockerConfiguration))
                return new ClientBuilder(
                    Core.Convert.MapWithValueGetter<TConfiguration, DockerSecretNameAttribute>(
                        (attribute, type, value) => Docker.Get(attribute.Name)));
            // Check the configuration type for the Environment
            if (typeof(TConfiguration) == typeof(IEnvironmentConfiguration))
                return new ClientBuilder(
                    Core.Convert.MapWithValueGetter<TConfiguration, EnvironmentVariableAttribute>(
                        ((attribute, type, value) => Environment.Get(attribute.Name))));
            // If we're here, just return a new instance
            return new ClientBuilder();
        }

        /// <summary>
        /// This method provides a factory for instantiating a MongoDB connection from Redis Keys
        /// </summary>
        /// <typeparam name="TConfiguration"></typeparam>
        /// <typeparam name="TRedisContext"></typeparam>
        /// <returns></returns>
        public static ClientBuilder FromConfiguration<TConfiguration, TRedisContext>()
            where TConfiguration : Configuration, new()
            where TRedisContext : Fux.Config.RedisHelper.Abstract.Connection, new() =>
            new ClientBuilder(Core.Convert.MapWithValueGetter<TConfiguration, RedisKeyAttribute>(
                (attribute, type, value) => Redis.Connection<TRedisContext>().Get(attribute.Name)));

        /// <summary>
        /// This method instantiates our class
        /// </summary>
        public ClientBuilder() { }

        /// <summary>
        /// This method instantiates a MongoDB connection using Docker Secrets
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public ClientBuilder(Configuration configuration) =>
            WithAuthenticationDatabase(configuration.AuthenticationDatabase ?? "admin")
                .WithAuthenticationPassword(configuration.AuthenticationPassword)
                .WithAuthenticationUsername(configuration.AuthenticationUsername)
                .WithAuthenticationDatabase(configuration.Database)
                .WithHosts(configuration.Hosts.Any() ? configuration.Hosts : new List<string> { "localhost" })
                .WithMaximumConnections(configuration.MaximumConnections ?? 1000);

        /// <summary>
        /// This method converts the connection builder to a MongoDB client
        /// </summary>
        /// <returns></returns>
        public MongoClient ToMongoClient()
        {
            // Define our MongoDB credential
            MongoCredential credential =
                MongoCredential.CreateCredential(AuthDatabase, AuthUsername, AuthPassword);
            // Define our MongoDB client settings
            MongoClientSettings clientSettings = new MongoClientSettings();
            // Set the credentials into the settings
            clientSettings.Credential = credential;
            // Define our cluster logging
            //clientSettings.ClusterConfigurator = cb => {
            // Subscribe to the commandStarted callback event
            //    cb.Subscribe<CommandStartedEvent>(e => {
            // Log the event
            //        Console.WriteLine($"{e.CommandName}:\t{e.Command.ToJson()}");
            //    });
            //};
            // Define our server addresses
            List<MongoServerAddress> serverAddresses = new List<MongoServerAddress>();
            // Iterate over the hosts in the instance
            foreach (IpAndPort ipAndPort in Hosts)
            {
                // Check the host type and add the server addresses
                if (ipAndPort.Item4 == HostType.Tcp)
                    serverAddresses.Add(new MongoServerAddress(ipAndPort.Item1, ipAndPort.Item2.Value));
                else
                    serverAddresses.Add(new MongoServerAddress(ipAndPort.Item1));
            }
            // Set the maximum number of connections into the client settings
            clientSettings.MaxConnectionPoolSize = MaxConnections;
            // Set the server addresses into the client settings
            clientSettings.Servers = serverAddresses;
            // Set the read preferences into the client settings
            clientSettings.ReadPreference = ReadPreference.SecondaryPreferred;
            // Check for a replica set name and set it into the client settings
            if (!string.IsNullOrEmpty(ReplicaSetName) && !string.IsNullOrWhiteSpace(ReplicaSetName))
                clientSettings.ReplicaSetName = ReplicaSetName;
            // We're done, return our new MongoDB client
            return new MongoClient(clientSettings);
        }

        /// <summary>
        /// This method adds a host to the instance
        /// </summary>
        /// <param name="address"></param>
        /// <param name="port"></param>
        /// <param name="access"></param>
        /// <param name="hostType"></param>
        /// <returns></returns>
        public ClientBuilder UsingHost(string address, int port = 27017, Access access = Access.ReadWrite,
            HostType hostType = HostType.Tcp)
        {
            // Check the address for a provided port
            if (address.Contains(":") && hostType.Equals(HostType.Tcp))
            {
                // Split the address and port
                string[] pair = address.Split(':');
                // Convert the port string to an integer
                port = Convert.ToInt32(pair[1]);
                // We're done, add the host to the instance
                return UsingTcpHost(pair[0], port, access);
            }
            // Check for a UNIX Domain Socket and add the host to the instance
            if (hostType.Equals(HostType.Unix))
                return UsingUnixHost(address, access);
            // We're done, add the TCP host to the instance
            return UsingTcpHost(address, port, access);
        }

        /// <summary>
        /// This method adds a TCP server host to the instance
        /// </summary>
        /// <param name="address"></param>
        /// <param name="port"></param>
        /// <param name="access"></param>
        /// <returns></returns>
        public ClientBuilder UsingTcpHost(string address, int port = 27017, Access access = Access.ReadWrite)
        {
            // Add the TCP host to the instance
            Hosts.Add(new IpAndPort(address, port, access, HostType.Tcp));
            // We're done, return
            return this;
        }

        /// <summary>
        /// This method sets the replica set name into the instance
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ClientBuilder UsingReplicaSet(string name)
        {
            // Set the replica set name into the instance
            ReplicaSetName = name;
            // We're done, return the instance
            return this;
        }

        /// <summary>
        /// This method adds a UNIX Domain Socket server host to the instance
        /// </summary>
        /// <param name="socketPath"></param>
        /// <param name="access"></param>
        /// <returns></returns>
        public ClientBuilder UsingUnixHost(string socketPath, Access access = Access.ReadWrite)
        {
            // Add the UNIX Domain Socket host to the instance
            Hosts.Add(new IpAndPort(socketPath, null, access, HostType.Unix));
            // We're done, return the instance
            return this;
        }

        /// <summary>
        /// This method sets the authentication parameters into the instance
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="database"></param>
        /// <returns></returns>
        public ClientBuilder WithAuthentication(string username, string password, string database = "admin") =>
            WithAuthenticationDatabase(database).WithAuthenticationUsernameAndPassword(username, password);

        /// <summary>
        /// This method sets the authentication database into the instance
        /// </summary>
        /// <param name="database"></param>
        /// <returns></returns>
        public ClientBuilder WithAuthenticationDatabase(string database)
        {
            // Set the authentication database into the instance
            AuthDatabase = database;
            // We're done, return the instance
            return this;
        }

        /// <summary>
        /// This method sets the authentication password into the instance
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public ClientBuilder WithAuthenticationPassword(string password)
        {
            // Set the password into the instance
            AuthPassword = password;
            // We're done, return the instance
            return this;
        }

        /// <summary>
        /// This method sets the authentication username into the instance
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public ClientBuilder WithAuthenticationUsername(string username)
        {
            // Set the username into the instance
            AuthUsername = username;
            // We're done, return the instance
            return this;
        }

        /// <summary>
        /// This method sets the authentication username and password into the instance
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public ClientBuilder WithAuthenticationUsernameAndPassword(string username, string password) =>
            WithAuthenticationUsername(username).WithAuthenticationPassword(password);

        /// <summary>
        /// This method adds a list of server hosts to the instance
        /// </summary>
        /// <param name="hosts"></param>
        /// <returns></returns>
        public ClientBuilder WithHosts(IEnumerable<string> hosts)
        {
            // Iterate over the hosts and add them to the instance
            hosts.ToList().ForEach(host => UsingHost(host));
            // We're done, return the instance
            return this;
        }

        /// <summary>
        /// This method sets the maximum number of connections for the pool into the instance
        /// </summary>
        /// <param name="connections"></param>
        /// <returns></returns>
        public ClientBuilder WithMaximumConnections(int connections)
        {
            // Set the maximum number of connections into the instance
            MaxConnections = connections;
            // We're done, return the instance
            return this;
        }
    }
}
