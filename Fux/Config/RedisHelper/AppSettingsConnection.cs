using Microsoft.Extensions.Configuration;
using Fux.Config.RedisHelper.Abstract;

namespace Fux.Config.RedisHelper
{
    /// <summary>
    /// This class maintains the connection settings from an <code>IConfiguration</code> object
    /// </summary>
    public class AppSettingsConnection : Connection
    {
        /// <summary>
        /// This property contains the configuration section name for the Redis configuration details
        /// </summary>
        private static string _configurationSectionName = "Fux:Redis";

        /// <summary>
        /// This method resets the configuration section name into memory
        /// </summary>
        /// <param name="configurationSectionName"></param>
        /// <returns></returns>
        public static string SetConfigurationSectionName(string configurationSectionName) =>
            _configurationSectionName = configurationSectionName;

        /// <summary>
        /// This method instantiates the connection settings from the appSettings.json file section
        /// </summary>
        /// <param name="configuration"></param>
        public AppSettingsConnection(IConfiguration configuration) : base()
        {
            // Localize the configuration section
            ConnectionSettings appSettings =
                configuration.GetSection(_configurationSectionName).Get<ConnectionSettings>();
            // Setup the instance
            WithAllowAdminFlag(appSettings.AllowAdmin)
                .WithDatabaseAtIndex(appSettings.Database)
                .WithHost(appSettings.Host)
                .WithPort(appSettings.Port)
                .WithPassword(appSettings.Password)
                .WithSerializerSettings()
                .WithSocketFlag(appSettings.IsUnixSocket)
                .WithSslFlag(appSettings.UseSsl);
        }

        /// <summary>
        /// This method instantiates the connection from the parsed appSettings.json file section
        /// </summary>
        /// <param name="appSettings"></param>
        public AppSettingsConnection(ConnectionSettings appSettings) =>
            // Setup the instance
            WithAllowAdminFlag(appSettings.AllowAdmin)
                .WithDatabaseAtIndex(appSettings.Database)
                .WithHost(appSettings.Host)
                .WithPort(appSettings.Port)
                .WithPassword(appSettings.Password)
                .WithSerializerSettings()
                .WithSocketFlag(appSettings.IsUnixSocket)
                .WithSslFlag(appSettings.UseSsl);
    }

    /// <summary>
    /// This class maintains the connection settings from an <code>IConfiguration</code> object
    /// with a generic settings class provider
    /// </summary>
    /// <typeparam name="TSettings"></typeparam>
    public class AppSettingsConnection<TSettings> : Connection where TSettings : ConnectionSettings
    {
        /// <summary>
        /// This property contains the configuration section name for the Redis configuration details
        /// </summary>
        private static string _configurationSectionName = "Fux:Redis";

        /// <summary>
        /// This method resets the configuration section name into memory
        /// </summary>
        /// <param name="configurationSectionName"></param>
        /// <returns></returns>
        private static string SetConfigurationSectionName(string configurationSectionName) =>
            _configurationSectionName = configurationSectionName;

        /// <summary>
        /// This method instantiates the connection settings from the appSettings.json file section
        /// </summary>
        /// <param name="configuration"></param>
        public AppSettingsConnection(IConfiguration configuration) : base()
        {
            // Localize the configuration section
            TSettings appSettings =
                configuration.GetSection(_configurationSectionName).Get<TSettings>();
            // Setup the instance
            WithAllowAdminFlag(appSettings.AllowAdmin)
                .WithDatabaseAtIndex(appSettings.Database)
                .WithHost(appSettings.Host)
                .WithPort(appSettings.Port)
                .WithPassword(appSettings.Password)
                .WithSerializerSettings()
                .WithSocketFlag(appSettings.IsUnixSocket)
                .WithSslFlag(appSettings.UseSsl);
        }

        /// <summary>
        /// This method instantiates the connection from the parsed appSettings.json file section
        /// </summary>
        /// <param name="appSettings"></param>
        public AppSettingsConnection(TSettings appSettings) =>
            WithAllowAdminFlag(appSettings.AllowAdmin)
                .WithDatabaseAtIndex(appSettings.Database)
                .WithHost(appSettings.Host)
                .WithPort(appSettings.Port)
                .WithPassword(appSettings.Password)
                .WithSerializerSettings()
                .WithSocketFlag(appSettings.IsUnixSocket)
                .WithSslFlag(appSettings.UseSsl);
    }
}

