using Fux.Config.DockerHelper.Attribute;
using Fux.Core;

namespace Fux.Config.RedisHelper
{
    /// <summary>
    /// This class bootstraps a Redis connection from Docker secrets
    /// </summary>
    public class DockerConnection : Abstract.Connection
    {
        /// <summary>
        /// This method instantiates the connection from the environment
        /// </summary>
        public DockerConnection()
        {
            // Localize the docker settings construct
            DockerConnectionSettings connectionSettings =
                Convert.MapWithValueGetter<DockerConnectionSettings, DockerSecretNameAttribute>((attribute, type, currentValue) =>
                    Docker.Get(attribute.Name));
            // Setup the instance
            WithAllowAdminFlag(connectionSettings.AllowAdmin)
                .WithDatabaseAtIndex(connectionSettings.Database)
                .WithHost(connectionSettings.Host)
                .WithPassword(connectionSettings.Password)
                .WithPort(connectionSettings.Port)
                .WithSerializerSettings()
                .WithSocketFlag(connectionSettings.IsUnixSocket)
                .WithSslFlag(connectionSettings.UseSsl)
                .WithUsername(connectionSettings.Username);
        }
    }

    /// <summary>
    /// This class bootstraps a Redis connection from Docker secrets with a generic connection settings provider
    /// </summary>
    /// <typeparam name="TSettings"></typeparam>
    public class DockerConnection<TSettings> : DockerConnection where TSettings : DockerConnectionSettings, new()
    {
        /// <summary>
        /// This method instantiates the connection from the environment
        /// </summary>
        public DockerConnection()
        {
            // Localize the docker settings construct
            TSettings connectionSettings =
                Convert.MapWithValueGetter<TSettings, DockerSecretNameAttribute>((attribute, type, currentValue) =>
                    Docker.Get(attribute.Name));
            // Setup the instance
            WithAllowAdminFlag(connectionSettings.AllowAdmin)
                .WithDatabaseAtIndex(connectionSettings.Database)
                .WithHost(connectionSettings.Host)
                .WithPassword(connectionSettings.Password)
                .WithPort(connectionSettings.Port)
                .WithSerializerSettings()
                .WithSocketFlag(connectionSettings.IsUnixSocket)
                .WithSslFlag(connectionSettings.UseSsl)
                .WithUsername(connectionSettings.Username);
        }
    }
}
