using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Fux.Dns.Model
{
    /// <summary>
    /// This class maintains interactions with the PublicSuffix database
    /// </summary>
    public class PublicSuffix
    {
        /// <summary>
        /// This constant defines the URL to the Public Suffix database
        /// </summary>
        public const string DatabaseUrl = "https://publicsuffix.org/list/public_suffix_list.dat";

        /// <summary>
        /// This delegate defines a refresh callback
        /// </summary>
        /// <param name="processedLine"></param>
        /// <param name="sourceLine"></param>
        public delegate void DelegateRefreshCallback(string processedLine, string sourceLine);

        /// <summary>
        /// This delegate defines an asynchronous refresh callback
        /// </summary>
        /// <param name="processedLine"></param>
        /// <param name="sourceLine"></param>
        public delegate Task DelegateRefreshCallbackAsync(string processedLine, string sourceLine);

        /// <summary>
        /// This property contains the public suffix local storage path
        /// </summary>
        public static string PublicSuffixPath = null;

        /// <summary>
        /// This property contains the custom top-level domains for the application
        /// </summary>
        private readonly List<string> _customTopLevelDomains = new List<string>();

        /// <summary>
        /// This property contains the timestamp of the last refresh of the PublicSuffix database
        /// </summary>
        private DateTime? _lastRefresh;

        /// <summary>
        /// This property contains the timestamp of the next refresh of the PublicSuffix database
        /// </summary>
        public DateTime? _nextRefresh;

        /// <summary>
        /// This property contains the PublicSuffix database
        /// </summary>
        /// <returns></returns>
        private readonly List<string> _topLevelDomains = new List<string>();

        /// <summary>
        /// This class instantiates an empty model
        /// </summary>
        public PublicSuffix() => Refresh();

        /// <summary>
        /// This method instantiates the model with a pre-populated PublicSuffix database
        /// </summary>
        /// <param name="publicSuffixDatabase"></param>
        public PublicSuffix(IEnumerable<string> publicSuffixDatabase)
        {
            // Set the public suffix database into the instance
            if (publicSuffixDatabase.Any()) _topLevelDomains = publicSuffixDatabase.ToList();
        }

        /// <summary>
        /// This method instantiates the model with a pre-populated PublicSuffix database
        /// as well as custom TLDs for the app
        /// </summary>
        /// <param name="publicSuffixDatabase"></param>
        /// <param name="customTopLevelDomains"></param>
        public PublicSuffix(IEnumerable<string> publicSuffixDatabase, IEnumerable<string> customTopLevelDomains)
        {
            // Set the public suffix database into the instance
            if (publicSuffixDatabase.Any()) _topLevelDomains = publicSuffixDatabase.ToList();
            // Set the custom top-level domains into the instance
            if (customTopLevelDomains.Any()) _customTopLevelDomains = customTopLevelDomains.ToList();
        }

        /// <summary>
        /// This method localizes the PublicSuffix database with an optional callback
        /// </summary>
        /// <param name="publicSuffixData"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        protected PublicSuffix Localize(string publicSuffixData, DelegateRefreshCallback callback = null)
        {
            // Clear the existing database
            _topLevelDomains.Clear();
            // Iterate over the lines
            publicSuffixData.Split("\n").ToList().ForEach(line =>
            {
                // Make sure we have a valid line
                if (!string.IsNullOrEmpty(line) && !string.IsNullOrWhiteSpace(line) && !line.StartsWith("//") &&
                    !line.StartsWith('#'))
                {
                    // Localize the processed line
                    string processedLine = line
                        .Replace("!.", "")
                        .Replace("*.", "")
                        .Replace("*", "")
                        .Replace("!", "")
                        .Trim()
                        .ToLower();
                    // Add the TLD to the instance
                    _topLevelDomains.Add(processedLine);
                    // Check for a callback
                    callback?.Invoke(processedLine, line);
                }
            });
            // We're done, return the instance
            return this;
        }

        /// <summary>
        /// This method localizes the PublicSuffix database with an optional asynchronous callback
        /// </summary>
        /// <param name="publicSuffixData"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        protected PublicSuffix LocalizeWithAsyncCallback(string publicSuffixData, DelegateRefreshCallbackAsync callback = null)
        {
            // Clear the existing database
            _topLevelDomains.Clear();
            // Iterate over the lines
            publicSuffixData.Split("\n").ToList().ForEach(async line =>
            {
                // Make sure we have a valid line
                if (!string.IsNullOrEmpty(line) && !string.IsNullOrWhiteSpace(line) && !line.StartsWith("//") &&
                    !line.StartsWith('#'))
                {
                    // Localize the processed line
                    string processedLine = line
                        .Replace("!.", "")
                        .Replace("*.", "")
                        .Replace("*", "")
                        .Replace("!", "")
                        .Trim()
                        .ToLower();
                    // Add the TLD to the instance
                    _topLevelDomains.Add(processedLine);
                    // Check for a callback
                    await callback?.Invoke(processedLine, line);
                }
            });
            // We're done, return the instance
            return this;
        }

        /// <summary>
        /// This method concatenates <code>_customTopLevelDomains</code> and <code>_topLevelDomains</code>
        /// for a full list of TLDs
        /// </summary>
        /// <returns></returns>
        public List<string> AllTlds() =>
            AllTopLevelDomains();

        /// <summary>
        /// This method concatenates <code>_customTopLevelDomains</code> and <code>_topLevelDomains</code>
        /// for a full list of TLDs
        /// </summary>
        /// <returns></returns>
        public List<string> AllTopLevelDomains() =>
            new List<string>(_topLevelDomains).Concat(_customTopLevelDomains).ToList();

        /// <summary>
        /// This method returns the custom top-level domains from the instance
        /// </summary>
        /// <returns></returns>
        public List<string> CustomTlds() =>
            CustomTopLevelDomains();

        /// <summary>
        /// This method returns the custom top-level domains from the instance
        /// </summary>
        /// <returns></returns>
        public List<string> CustomTopLevelDomains() =>
            _customTopLevelDomains;

        /// <summary>
        /// This method returns the last refreshed timestamp from the instance
        /// </summary>
        /// <returns></returns>
        public DateTime? LastRefresh() =>
            _lastRefresh;

        /// <summary>
        /// This method returns the next refresh timestamp from the instance
        /// </summary>
        /// <returns></returns>
        public DateTime? NextRefresh() =>
            _nextRefresh;

        /// <summary>
        /// This method refreshes the PublicSuffix database
        /// </summary>
        /// <returns></returns>
        public PublicSuffix Refresh()
        {
            // Check the next refresh timestamp to see if we even need to refresh
            if (_nextRefresh.HasValue && (_nextRefresh.Value > DateTime.UtcNow)) return this;
            // Ensure we have a database path
            if (string.IsNullOrEmpty(PublicSuffixPath) || string.IsNullOrWhiteSpace(PublicSuffixPath))
                PublicSuffixPath = Path.GetTempFileName();
            // Read the database from the file
            PublicSuffixDatabase database = PublicSuffixDatabase.Open(PublicSuffixPath);
            // Check the value of the next refresh
            if (database.LastRefresh.HasValue && (database.LastRefresh.Value > DateTime.UtcNow))
            {
                // Set the custom TLDs into the instance
                _customTopLevelDomains.AddRange(database.CustomTopLevelDomains);
                // Set the last refreshed timestamp into the instance
                _lastRefresh = database.LastRefresh;
                // Set the next refresh timestamp into the instance
                _nextRefresh = database.NextRefresh;
                // Set the top-level domains into the instance
                _topLevelDomains.AddRange(database.TopLevelDomains);
                // We're done, return the instance
                return this;
            }
            // Set the last refresh timestamp into the instance
            _lastRefresh = DateTime.UtcNow;
            // Set the next refresh timestamp into the instance
            _nextRefresh = _lastRefresh.Value.AddHours(24);
            // Localize our WebClient
            using WebClient client = new WebClient();
            // Download the Public Suffix database
            string publicSuffixDatabase = client.DownloadString(new Uri(DatabaseUrl));
            // Localize the data and return the instance
            Localize(publicSuffixDatabase, null);
            // Write the database to file
            PublicSuffixDatabase.Store(PublicSuffixPath, this);
            // We're done, return the instance
            return this;
        }

        /// <summary>
        /// This method asynchronously refreshes the PublicSuffix database
        /// </summary>
        /// <returns></returns>
        public async Task<PublicSuffix> RefreshAsync()
        {
            // Check the next refresh timestamp to see if we even need to refresh
            if (_nextRefresh.HasValue && (_nextRefresh.Value > DateTime.UtcNow)) return this;
            // Ensure we have a database path
            if (string.IsNullOrEmpty(PublicSuffixPath) || string.IsNullOrWhiteSpace(PublicSuffixPath))
                PublicSuffixPath = Path.GetTempFileName();
            // Read the database from the file
            PublicSuffixDatabase database = await PublicSuffixDatabase.OpenAsync(PublicSuffixPath);
            // Check the value of the next refresh
            if (database.LastRefresh.HasValue && (database.LastRefresh.Value > DateTime.UtcNow))
            {
                // Set the custom TLDs into the instance
                _customTopLevelDomains.AddRange(database.CustomTopLevelDomains);
                // Set the last refreshed timestamp into the instance
                _lastRefresh = database.LastRefresh;
                // Set the next refresh timestamp into the instance
                _nextRefresh = database.NextRefresh;
                // Set the top-level domains into the instance
                _topLevelDomains.AddRange(database.TopLevelDomains);
                // We're done, return the instance
                return this;
            }
            // Set the last refresh timestamp into the instance
            _lastRefresh = DateTime.UtcNow;
            // Set the next refresh timestamp into the instance
            _nextRefresh = _lastRefresh.Value.AddHours(24);
            // Localize our WebClient
            using WebClient client = new WebClient();
            // Download the Public Suffix database
            string publicSuffixDatabase = await client.DownloadStringTaskAsync(new Uri(DatabaseUrl));
            // We're done, localize the data and return the instance
            Localize(publicSuffixDatabase, null);
            // Writeh the datbase to file
            await PublicSuffixDatabase.StoreAsync(PublicSuffixPath, this);
            // We're done, return the instance
            return this;
        }

        /// <summary>
        /// This method returns the top-level domains from the instance
        /// </summary>
        /// <returns></returns>
        public List<string> Tlds() =>
            TopLevelDomains();

        /// <summary>
        /// This method returns the top-level domains from the instance
        /// </summary>
        /// <returns></returns>
        public List<string> TopLevelDomains() =>
            _topLevelDomains;

        /// <summary>
        /// This method adds a custom top-level domain to the instance
        /// </summary>
        /// <param name="tld"></param>
        /// <returns></returns>
        public PublicSuffix WithCustomTld(string tld) =>
            WithCustomTopLevelDomain(tld);

        /// <summary>
        /// This method adds a custom top-level domain to the instance
        /// </summary>
        /// <param name="tld"></param>
        /// <returns></returns>
        public PublicSuffix WithCustomTopLevelDomain(string tld)
        {
            // Add the custom top-level domain to the instance
            _customTopLevelDomains.Add(tld);
            // We're done, return the instance
            return this;
        }
    }
}
