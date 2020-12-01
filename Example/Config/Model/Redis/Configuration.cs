namespace Fux.Example.Config.Model.Redis
{
    /// <summary>
    /// This class is an alias class than maintains the settings structure for Redis
    /// </summary>
    public class Configuration : Fux.Config.RedisHelper.EnvironmentConnectionSettings
    {
        /*
         * This class is just an alias, below is an example of what's expected in the environment
         *
         *  | ----------------------- | ----------- | ----------------------- | ----------- |
         *  |  Environment Variable   |  Type       |  Example Value          | Default     |
         *  | ----------------------- | ----------- | ----------------------- | ----------- |
         *  |  FUX_REDIS_ALLOW_ADMIN  |  <boolean>  |  true|false             |  false      |
         *  |  FUX_REDIS_HOST         |  <string>   |  'IPv4|IPv6|Host|UNIX'  |  localhost  |
         *  |  FUX_REDIS_IS_SOCKET    |  <boolean>  |  true|false             |  false      |
         *  |  FUX_REDIS_PASSWORD     |  <string>   |  '<password>'           |  null       |
         *  |  FUX_REDIS_PORT         |  <int>      |  6379                   |  6379       |
         *  |  FUX_REDIS_USERNAME     |  <string>   |  '<acl-username>'       |  null       |
         *  |  FUX_REDIS_USE_SSL      |  <boolean>  |  true|false             |  false      |
         *  | ----------------------- | ----------- | ----------------------- | ----------- |
         *
         */
    }
}
