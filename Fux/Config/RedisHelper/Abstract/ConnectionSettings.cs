namespace Fux.Config.RedisHelper.Abstract
{
    /// <summary>
    /// This class maintains the structure of Redis connection settings
    /// </summary>
    public class ConnectionSettings
    {
        /// <summary>
        /// This property contains the AllowAdmin flag for the Redis connection
        /// </summary>
        public virtual bool AllowAdmin { get; set; } = false;

        /// <summary>
        /// This property contains the database to connect to
        /// </summary>
        public virtual int? Database { get; set; } = null;

        /// <summary>
        /// This property contains the host address on which the Redis service listens
        /// </summary>
        public virtual string Host { get; set; } = null;

        /// <summary>
        /// This property contains the UNIX domain socket flag
        /// </summary>
        public virtual bool IsUnixSocket { get; set; } = false;

        /// <summary>
        /// This property contains the password for authenticating with the Redis service
        /// </summary>
        public virtual string Password { get; set; } = null;

        /// <summary>
        /// This property contains the port number on which the Redis service listens
        /// </summary>
        public virtual int? Port { get; set; } = null;

        /// <summary>
        /// This property contains the username for authenticating with the Redis service
        /// </summary>
        public virtual string Username { get; set; }

        /// <summary>
        /// This property contains the SSL flag for connecting to the Redis service
        /// </summary>
        public virtual bool UseSsl { get; set; } = false;
    }
}
