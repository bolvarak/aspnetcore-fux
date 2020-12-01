using Fux.Config.DockerHelper.Attribute;
using Fux.Config.RedisHelper.Abstract;

namespace Fux.Config.RedisHelper
{
    /// <summary>
    /// This class maintains the connections settings structure from Docker
    /// </summary>
    public class DockerConnectionSettings : ConnectionSettings
    {
        /// <summary>
        /// This property contains the AllowAdmin flag for the Redis connection
        /// </summary>
        [DockerSecretName("fux-redis-allow-admin")]
        public override bool AllowAdmin { get; set; } = false;

        /// <summary>
        /// This property contains the database to connect to
        /// </summary>
        [DockerSecretName("fux_redis_database")]
        public override int? Database { get; set; } = null;

        /// <summary>
        /// This property contains the host address on which the Redis service listens
        /// </summary>
        [DockerSecretName("fux-redis-host")]
        public override string Host { get; set; } = null;

        /// <summary>
        /// This property contains the UNIX domain socket flag
        /// </summary>
        [DockerSecretName("fux-redis-is-socket")]
        public override bool IsUnixSocket { get; set; } = false;

        /// <summary>
        /// This property contains the password for authenticating with the Redis service
        /// </summary>
        [DockerSecretName("fux-redis-password")]
        public override string Password { get; set; } = null;

        /// <summary>
        /// This property contains the port number on which the Redis service listens
        /// </summary>
        [DockerSecretName("fux-redis-port")]
        public override int? Port { get; set; } = null;

        /// <summary>
        /// This property contains the username for authenticating with the Redis service
        /// </summary>
        [DockerSecretName("fux-redis-username")]
        public override string Username { get; set; }

        /// <summary>
        /// This property contains the SSL flag for connecting to the Redis service
        /// </summary>
        [DockerSecretName("fux-redis-use-ssl")]
        public override bool UseSsl { get; set; } = false;
    }
}
