using Fux.Core;
using Fux.Config.EnvironmentHelper.Attribute;

namespace Fux.Config.RedisHelper
{
    /// <summary>
    /// This class bootstraps a Redis connection from Environment variables
    /// </summary>
    public class EnvironmentConnection : Abstract.Connection
    {
        /// <summary>
        /// This method instantiates the connection from the environment
        /// </summary>
        public EnvironmentConnection()
        {
            // Localize the docker settings construct
            EnvironmentConnectionSettings connectionSettings =
                Convert.MapWithValueGetter<EnvironmentConnectionSettings, EnvironmentVariableAttribute>(
                    (attribute, type, currentValue) => Environment.Get(type, attribute.Name, attribute.AllowEmpty));
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
    /// This class provides a generic for connections using Environment Variables
    /// </summary>
    /// <typeparam name="TSettings"></typeparam>
    public class EnvironmentConnection<TSettings> : EnvironmentConnection where TSettings : EnvironmentConnectionSettings, new()
    {
        /// <summary>
        /// This method instantiates the environment connection from a custom settings class
        /// </summary>
        /// <returns></returns>
        public EnvironmentConnection()
        {
            // Localize the docker settings construct
            TSettings connectionSettings =
                Convert.MapWithValueGetter<TSettings, EnvironmentVariableAttribute>(
                    (attribute, type, currentValue) => Environment.Get(type, attribute.Name, attribute.AllowEmpty));
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
