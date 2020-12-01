using Fux.Config.EnvironmentHelper.Attribute;
using Fux.Config.RedisHelper.Abstract;

namespace Fux.Config.RedisHelper
{
    public class EnvironmentConnectionSettings : ConnectionSettings
    {
        /// <summary>
        /// This property contains the AllowAdmin flag for the Redis connection
        /// </summary>
        [EnvironmentVariable("FUX_REDIS_ALLOW_ADMIN")]
        public override bool AllowAdmin { get; set; } = false;

        /// <summary>
        /// This property contains the database to connect to
        /// </summary>
        [EnvironmentVariable("FUX_REDIS_DATABASE")]
        public override int? Database { get; set; } = null;

        /// <summary>
        /// This property contains the host address on which the Redis service listens
        /// </summary>
        [EnvironmentVariable("FUX_REDIS_HOST")]
        public override string Host { get; set; } = null;

        /// <summary>
        /// This property contains the UNIX domain socket flag
        /// </summary>
        [EnvironmentVariable("FUX_REDIS_IS_SOCKET")]
        public override bool IsUnixSocket { get; set; } = false;

        /// <summary>
        /// This property contains the password for authenticating with the Redis service
        /// </summary>
        [EnvironmentVariable("FUX_REDIS_PASSWORD")]
        public override string Password { get; set; } = null;

        /// <summary>
        /// This property contains the port number on which the Redis service listens
        /// </summary>
        [EnvironmentVariable("FUX_REDIS_PORT")]
        public override int? Port { get; set; } = null;

        /// <summary>
        /// This property contains the username for authenticating with the Redis service
        /// </summary>
        [EnvironmentVariable("FUX_REDIS_USERNAME")]
        public override string Username { get; set; }

        /// <summary>
        /// This property contains the SSL flag for connecting to the Redis service
        /// </summary>
        [EnvironmentVariable("FUX_REDIS_USE_SSL")]
        public override bool UseSsl { get; set; } = false;
    }
}
