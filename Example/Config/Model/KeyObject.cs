using Fux.Config.DockerHelper.Attribute;
using Fux.Config.EnvironmentHelper.Attribute;
using Fux.Config.RedisHelper.Attribute;
using Newtonsoft.Json;

namespace Fux.Example.Config.Model
{
    /// <summary>
    /// This class maintains the structure of a JSON serialized environment variable, docker secret or redis object
    /// </summary>
    [RedisDatabase(15)]
    [RedisKey("fux-example-config-redis-key-object")]
    [EnvironmentVariable("FUX_EXAMPLE_CONFIG_KEY_OBJECT")]
    [DockerSecretName("fux-example-config-docker-key-object")]
    public class KeyObject
    {
        /// <summary>
        /// This property contains a boolean value
        /// </summary>
        /// <value>true</value>
        [JsonProperty("boolean")]
        public bool Boolean { get; set; }

        /// <summary>
        /// This property contains a double value
        /// </summary>
        /// <value>3.142</value>
        [JsonProperty("double")]
        public double Double { get; set; }

        /// <summary>
        /// This property contains an integer value
        /// </summary>
        /// <value>42</value>
        [JsonProperty("integer")]
        public int Integer { get; set; }

        /// <summary>
        /// This property contains a nested object
        /// </summary>
        [JsonProperty("nestedObject")]
        public KeyObjectNested NestedObject { get; set; } = new KeyObjectNested();

        /// <summary>
        /// This property contains a string value
        /// </summary>
        /// <value>I think therefore I am</value>
        [JsonProperty("string")]
        public string String { get; set; }
    }
}
