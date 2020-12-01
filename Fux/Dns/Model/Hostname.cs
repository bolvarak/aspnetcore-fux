using Newtonsoft.Json;

namespace Fux.Dns.Model
{
    /// <summary>
    /// This class maintains the structure of a parsed hostname using PublicSuffix
    /// </summary>
    public class Hostname
    {
        /// <summary>
        /// This property contains the domain parsed from the source
        /// </summary>
        [JsonProperty("domain")]
        public string Domain { get; set; }

        /// <summary>
        /// This property contains the host parsed from the source
        /// </summary>
        [JsonProperty("host")]
        public string Host { get; set; }

        /// <summary>
        /// This property denotes that a custom TLD was matched
        /// </summary>
        /// <value></value>
        [JsonProperty("isCustom")]
        public bool IsCustom { get; set; } = false;

        /// <summary>
        /// This property contains the valid flag for the hostname
        /// </summary>
        [JsonProperty("isValid")]
        public bool IsValid { get; set; } = false;

        /// <summary>
        /// This property contains the port parsed from the source if there was one
        /// </summary>
        [JsonProperty("port")]
        public int? Port { get; set; }

        /// <summary>
        /// This property contains the protocol parsed from the source if there was one
        /// </summary>
        [JsonProperty("protocol")]
        public string Protocol { get; set; }

        /// <summary>
        /// This property contains the source to that was parsed
        /// </summary>
        [JsonProperty("source")]
        public string Source { get; set; }

        /// <summary>
        /// This property contains the TLD parsed from the source
        /// </summary>
        [JsonProperty("topLevelDomain")]
        public string TopLevelDomain { get; set; }

        /// <summary>
        /// This method converts the instance to a domain name
        /// </summary>
        /// <returns></returns>
        public string ToDomain() =>
            Domain;

        /// <summary>
        /// This method converts the instance to a fully qualified domain name
        /// </summary>
        /// <returns></returns>
        public string ToFullyQualifiedDomain() =>
            string.IsNullOrEmpty(Host) || string.IsNullOrWhiteSpace(Host) ? Domain : $"{Host}.{Domain}";

        /// <summary>
        /// This method converts the instance to a host name
        /// </summary>
        /// <returns></returns>
        public string ToHost() =>
            Host;

        /// <summary>
        /// This method generates the wildcard for the domain
        /// </summary>
        /// <returns></returns>
        public string ToWildcard()
            => $"*.{Domain}";
    }
}
