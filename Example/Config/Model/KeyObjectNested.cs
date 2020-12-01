using System;
using Newtonsoft.Json;

namespace Fux.Example.Config.Model
{
    /// <summary>
    /// This class maintains the example structure of a nested object from an environment, docker secret or redis key
    /// </summary>
    public class KeyObjectNested
    {
        /// <summary>
        /// This property contains a DateTime value
        /// </summary>
        /// <value>2020-01-01T00:00:00.000Z</value>
        [JsonProperty("dateTime")]
        public DateTime DateTime { get; set; }

        /// <summary>
        /// This property contains a GUID value
        /// </summary>
        /// <value>00000000-0000-0000-0000-000000000001</value>
        [JsonProperty("guid")]
        public Guid Guid { get; set; }
    }
}
