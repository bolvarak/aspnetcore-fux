namespace Fux.Config.Abstract
{
    /// <summary>
    /// This class maintains the structure expectations for Fux in <code>appSettings.json</code>
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// This property contains the Redis configuration expectations from <code>appSettings.json</code>
        /// /// </summary>
        /// <value></value>
        public RedisHelper.Abstract.ConnectionSettings Redis { get; set; }
            = new RedisHelper.Abstract.ConnectionSettings();
    }
}
