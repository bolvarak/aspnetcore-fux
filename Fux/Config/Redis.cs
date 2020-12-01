using System;
using Fux.Config.RedisHelper;
using Fux.Core;
using Fux.Config.RedisHelper.Abstract;

namespace Fux.Config
{
    /// <summary>
    /// This class maintains our fluid interface for Redis connections
    /// </summary>
    public static class Redis
    {
        /// <summary>
        /// This method sets a new connection structure into memory
        /// </summary>
        /// <param name="connection"></param>
        public static Connection Connect(Connection connection) =>
            Singleton<Connection>.Instance(connection);

        /// <summary>
        /// This method sets a new typed connection structure into memory
        /// </summary>
        /// <param name="connection"></param>
        /// <typeparam name="TConnection"></typeparam>
        /// <returns></returns>
        public static TConnection Connect<TConnection>(TConnection connection) where TConnection : Connection, new() =>
            Singleton<TConnection>.Instance(connection);

        /// <summary>
        /// This method sets up a Redis connection from Docker Secrets using only the configuration POCO
        /// </summary>
        /// <typeparam name="TDockerSettings"></typeparam>
        /// <returns></returns>
        public static DockerConnection<TDockerSettings> ConnectFromDocker<TDockerSettings>()
            where TDockerSettings : DockerConnectionSettings, new() =>
                new DockerConnection<TDockerSettings>();

        /// <summary>
        /// This method sets up a Redis connection from Environment Variables using only the configuration POCO
        /// </summary>
        /// <typeparam name="TEnvironmentSettings"></typeparam>
        /// <returns></returns>
        public static EnvironmentConnection<TEnvironmentSettings> ConnectFromEnvironment<TEnvironmentSettings>()
            where TEnvironmentSettings : EnvironmentConnectionSettings, new() =>
                new EnvironmentConnection<TEnvironmentSettings>();

        /// <summary>
        /// This method sets up a Redis connection from Docker Secrets
        /// </summary>
        /// <returns></returns>
        public static DockerConnection ConnectWithDockerSecrets() =>
            Connect(Reflection.Instance<DockerConnection>());

        /// <summary>
        /// This method sets up a Redis connection from Docker Secrets
        /// </summary>
        /// <typeparam name="TDockerConnection"></typeparam>
        /// <returns></returns>
        public static TDockerConnection ConnectWithDockerSecrets<TDockerConnection>() where TDockerConnection : DockerConnection, new() =>
            Connect<TDockerConnection>(Reflection.Instance<TDockerConnection>());

        /// <summary>
        /// This method sets up a Redis connection from Environment Variables
        /// </summary>
        /// <returns></returns>
        public static EnvironmentConnection ConnectWithEnvironmentVariables() =>
            Connect(new EnvironmentConnection());

        /// <summary>
        /// This method sets up a Redis connection from Environment Variables
        /// </summary>
        /// <typeparam name="TEnvironmentConnection"></typeparam>
        /// <returns></returns>
        public static EnvironmentConnection ConnectWithEnvironmentVariables<TEnvironmentConnection>() where TEnvironmentConnection : EnvironmentConnection, new() =>
            Connect<TEnvironmentConnection>(Reflection.Instance<TEnvironmentConnection>());

        /// <summary>
        /// This method returns a typed connection structure from memory
        /// </summary>
        /// <typeparam name="TConnection"></typeparam>
        /// /// <returns></returns>
        public static TConnection Connection<TConnection>() where TConnection: Connection, new() =>
            Singleton<TConnection>.Instance();
    }
}
