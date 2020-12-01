using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fux.Dns
{
    /// <summary>
    /// This class translates a hostname into its parts and validates it against the PublicSuffix database
    /// </summary>
    public class Hostname
    {
        /// <summary>
        /// This property contains the Public Suffix database
        /// </summary>
        private static readonly Model.PublicSuffix _publicSuffixDatabase = new Model.PublicSuffix();

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
        /// This method instantiates the class
        /// </summary>
        public Hostname() { }

        /// <summary>
        /// This method instantiates the class with a source URI to parse
        /// </summary>
        /// <param name="source"></param>
        public Hostname(Uri source) => Parse(source);

        /// <summary>
        /// This method instantiates the class with a source string to parse
        /// </summary>
        /// <param name="source"></param>
        public Hostname(string source) => Parse(source);

        /// <summary>
        /// This method ensures the database is in place when needed
        /// </summary>
        private static void database()
        {
            // Refresh the database
            _publicSuffixDatabase.Refresh();
        }

        /// <summary>
        /// This method asynchronously ensures the database is in place when needed
        /// </summary>
        /// <returns></returns>
        private static async Task databaseAsync()
        {
            // Refresh the database
            await _publicSuffixDatabase.RefreshAsync();
        }

        /// <summary>
        /// This method determines the host of the source
        /// </summary>
        /// <param name="source"></param>
        /// <param name="domain"></param>
        /// <param name="host"></param>
        private static void determineHost(string source, string domain, out string host)
        {
            // Determine the host
            host = source.Replace(domain, "").TrimEnd('.').ToLower();
        }

        /// <summary>
        /// This method determines the Top Level Domain (TLD) from the source
        /// </summary>
        /// <param name="source"></param>
        /// <param name="favorCustomTopLevelDomains"></param>
        /// <param name="isCustom"></param>
        /// <param name="isValid"></param>
        /// <param name="topLevelDomain"></param>
        /// <param name="domain"></param>
        private static void determineTopLevelDomain(string source, bool favorCustomTopLevelDomains, out bool isValid, out bool isCustom, out string topLevelDomain, out string domain)
        {
            // Run our search agains the custom top-level domains
            searchCustomTopLevelDomains(source, out bool customIsValid, out string customTopLevelDomain,
                out string customDomain);
            // Run our search against the PublicSuffix database
            searchPublicTopLevelDomains(source, out bool publicIsValid, out string publicTopLevelDomain,
                out string publicDomain);
            // Check our flags to see if we need to favor the custom TLDs
            if (customIsValid && publicIsValid && favorCustomTopLevelDomains)
            {
                // Reset the custom flag
                isCustom = true;
                // Reset the valid flag
                isValid = customIsValid;
                // Reset the top-level domain
                topLevelDomain = customTopLevelDomain;
                // Reset the domain
                domain = customDomain;
            }
            else if (customIsValid && publicIsValid)
            {
                // Reset the custom flag
                isCustom = false;
                // Reset the valid flag
                isValid = publicIsValid;
                // Reset the top-level domain
                topLevelDomain = publicTopLevelDomain;
                // Reset the domain
                domain = publicDomain;
            }
            else if (publicIsValid)
            {
                // Reset the custom flag
                isCustom = false;
                // Reset the valid flag
                isValid = publicIsValid;
                // Reset the top-level domain
                topLevelDomain = publicTopLevelDomain;
                // Reset the domain
                domain = publicDomain;
            }
            else
            {
                // Reset the custom flag
                isCustom = true;
                // Reset the valid flag
                isValid = customIsValid;
                // Reset the top-level domain
                topLevelDomain = customTopLevelDomain;
                // Reset the domain
                domain = customDomain;
            }
        }

        /// <summary>
        /// This method is responsible for parsing a hostname
        /// </summary>
        /// <param name="source"></param>
        /// <param name="favorCustomTopLevelDomains"></param>
        /// <returns></returns>
        private static Model.Hostname parseInternal(string source, bool favorCustomTopLevelDomains = false)
        {
            // Instantiate our response
            Model.Hostname response = new Model.Hostname();
            // Check for a port
            if (source.Contains(':'))
            {
                // Set the port into the response
                response.Port = int.Parse(source.Split(':', StringSplitOptions.RemoveEmptyEntries).Last());
                // Set the source into the response
                response.Source = source.Split(':', StringSplitOptions.RemoveEmptyEntries).First().ToLower();
            }
            else response.Source = source;

            // Determine the top level domain
            determineTopLevelDomain(response.Source, favorCustomTopLevelDomains, out bool isValid, out bool isCustom,
                out string tld, out string domain);
            // Set the validation flag into the response
            response.IsValid = isValid;
            // Check the valid response
            if (response.IsValid)
            {
                // Set the custom flag into the response
                response.IsCustom = isCustom;
                // Set the top level domain into the response
                response.TopLevelDomain = tld;
                // Set the domain into the response
                response.Domain = domain;
                // Determine the host
                determineHost(response.Source, response.Domain, out string host);
                // Set the host into the response
                response.Host = host;
            }

            // We're done, send the response
            return response;
        }

        /// <summary>
        /// This method processes the search results and generates the top-level domain and domain
        /// </summary>
        /// <param name="needle"></param>
        /// <param name="haystack"></param>
        /// <param name="isValid"></param>
        /// <param name="topLevelDomain"></param>
        /// <param name="domain"></param>
        private static void processSearchResults(string needle, IEnumerable<string> haystack, out bool isValid,
            out string topLevelDomain, out string domain)
        {
            // Split the domain parts
            List<string> parts = needle.ToLower().Trim().Split('.')
                .Where(s => !string.IsNullOrEmpty(s) && !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())
                .ToList();
            // Define our working TLD
            string workingTld = parts.Last().Trim().ToLower();
            // Remove the last part of the list
            parts.RemoveAt(parts.Count - 1);
            // Define our found flag
            bool found = false;
            // Iterate until we've found it
            while (parts.Any())
            {
                // Localize the TLD
                string tld = haystack.FirstOrDefault(s => s.Equals(workingTld));
                // Check to see if we've found our TLD
                if (string.IsNullOrEmpty(tld) || string.IsNullOrWhiteSpace(tld))
                {
                    // Check for parts
                    if (parts.Any())
                    {
                        // Reset the working TLD
                        workingTld = $"{parts.Last().Trim().ToLower()}.{workingTld}";
                        // Remove the last part from the list
                        parts.RemoveAt(parts.Count - 1);
                    }
                }
                else
                {
                    // Reset the found flag
                    found = true;
                    // We're done, break the loop
                    break;
                }
            }
            // Check the found flag
            if (!found)
            {
                // Set the valid flag
                isValid = false;
                // Set the domain
                domain = null;
                // Set the top level domain
                topLevelDomain = null;
                return;
            }
            // Check for a working TLD
            if (!string.IsNullOrEmpty(workingTld.Trim()) && !string.IsNullOrWhiteSpace(workingTld.Trim()))
            {
                // Set the valid flag in the instance
                isValid = true;
                // Set the top level domain
                topLevelDomain = workingTld.Trim().ToLower();
                // Check for parts and set the domain
                if (parts.Any()) domain = $"{parts.Last().Trim().ToLower()}.{workingTld.Trim().ToLower()}";
                else domain = workingTld.Trim().ToLower();
            }
            else
            {
                // Set the valid flag in the instance
                isValid = false;
                // Set the top level domain
                topLevelDomain = null;
                // Set the domain
                domain = null;
            }
        }

        /// <summary>
        /// This method searches the custom top-level domains
        /// </summary>
        /// <param name="source"></param>
        /// <param name="isValid"></param>
        /// <param name="topLevelDomain"></param>
        /// <param name="domain"></param>
        private static void searchCustomTopLevelDomains(string source, out bool isValid, out string topLevelDomain,
            out string domain) =>
            processSearchResults(source, _publicSuffixDatabase.CustomTopLevelDomains(), out isValid, out topLevelDomain,
                out domain);

        /// <summary>
        /// This method searches the top-level domains from PublicSuffix
        /// </summary>
        /// <param name="source"></param>
        /// <param name="isValid"></param>
        /// <param name="topLevelDomain"></param>
        /// <param name="domain"></param>
        private static void searchPublicTopLevelDomains(string source, out bool isValid, out string topLevelDomain,
            out string domain) =>
            processSearchResults(source, _publicSuffixDatabase.TopLevelDomains(), out isValid, out topLevelDomain,
                out domain);

        /// <summary>
        /// This is a wrapper method that executes a parse and returns the valid flag
        /// </summary>
        /// <param name="hostname"></param>
        /// <param name="favorCustomTopLevelDomains"></param>
        /// <returns></returns>
        public static bool IsValid(string hostname, bool favorCustomTopLevelDomains = false) =>
            Parse(hostname, favorCustomTopLevelDomains)?.IsValid ?? false;

        /// <summary>
        /// This method is a wrapper method that executes an asynchronous parse and returns the valid flag
        /// </summary>
        /// <param name="hostname"></param>
        /// <param name="favorCustomTopLevelDomains"></param>
        /// <returns></returns>
        public static async Task<bool> IsValidAsync(string hostname, bool favorCustomTopLevelDomains = false) =>
            (await ParseAsync(hostname, favorCustomTopLevelDomains))?.IsValid ?? false;

        /// <summary>
        /// This is a wrapper method that executes a parse and returns the valid flag
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="favorCustomTopLevelDomains"></param>
        /// <returns></returns>
        public static bool IsValid(Uri uri, bool favorCustomTopLevelDomains = false) =>
            IsValid(uri.Host.ToLower().Trim(), favorCustomTopLevelDomains);

        /// <summary>
        /// This method is a wrapper method that executes an asynchronous parse and returns the valid flag
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="favorCustomTopLevelDomains"></param>
        /// <returns></returns>
        public static Task<bool> IsValidAsync(Uri uri, bool favorCustomTopLevelDomains = false) =>
            IsValidAsync(uri.Host.ToLower().Trim(), favorCustomTopLevelDomains);

        /// <summary>
        /// This method parses a URI source
        /// </summary>
        /// <param name="source"></param>
        /// <param name="favorCustomTopLevelDomains"></param>
        /// <returns></returns>
        public static Model.Hostname Parse(Uri source, bool favorCustomTopLevelDomains = false) =>
            Parse(source.Host.ToLower().Trim(), favorCustomTopLevelDomains);

        /// <summary>
        /// This method parses a string source
        /// </summary>
        /// <param name="source"></param>
        /// <param name="favorCustomTopLevelDomains"></param>
        /// <returns></returns>
        public static Model.Hostname Parse(string source, bool favorCustomTopLevelDomains = false)
        {
            // Ensure the database is in place
            database();
            // We're done, parse the hostname and return
            return parseInternal(source, favorCustomTopLevelDomains);
        }

        /// <summary>
        /// This method asynchronously parses a string source
        /// </summary>
        /// <param name="source"></param>
        /// <param name="favorCustomTopLevelDomains"></param>
        /// <returns></returns>
        public static async Task<Model.Hostname> ParseAsync(string source, bool favorCustomTopLevelDomains = false)
        {
            // Ensure the database is in place
            await databaseAsync();
            // We're done, parse the hostname and return
            return parseInternal(source, favorCustomTopLevelDomains);
        }

        /// <summary>
        /// This method adds a custom top-level domain to the database
        /// </summary>
        /// <param name="tld"></param>
        public static void WithCustomTld(string tld) =>
            WithCustomTopLevelDomain(tld);

        /// <summary>
        /// This method adds a custom top-level domain to the database
        /// </summary>
        /// <param name="tld"></param>
        public static void WithCustomTopLevelDomain(string tld) =>
            _publicSuffixDatabase.WithCustomTopLevelDomain(tld);

    }
}
