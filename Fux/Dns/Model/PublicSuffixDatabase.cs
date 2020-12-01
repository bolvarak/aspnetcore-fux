using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Fux.Dns.Model
{
    /// <summary>
    /// This class maintains the model for storing PublicSuffix data
    /// /// </summary>
    public class PublicSuffixDatabase
    {
        /// <summary>
        /// This property contains the custom top-level domains for the app
        /// </summary>
        [JsonProperty("customTopLevelDomains")]
        public List<string> CustomTopLevelDomains { get; set; } = new List<string>();

        /// <summary>
        /// This property contains the URL of the database used to populate the instance
        /// </summary>
        [JsonProperty("databaseUrl")]
        public string DatabaseUrl = PublicSuffix.DatabaseUrl;

        /// <summary>
        /// This property contains the timestamp of the last database refresh
        /// </summary>
        /// <value></value>
        [JsonProperty("lastRefresh")]
        public DateTime? LastRefresh { get; set; }

        /// <summary>
        /// This property contains the timestamp of the next refresh
        /// </summary>
        /// <value></value>
        [JsonProperty("nextRefresh")]
        public DateTime? NextRefresh { get; set; }

        /// <summary>
        /// This property contains the top-level domains from PublicSuffix
        /// </summary>
        [JsonProperty("topLevelDomains")]
        public List<string> TopLevelDomains { get; set; } = new List<string>();

        /// <summary>
        /// This method provides a fluid factory for reading the structre from the filesystem
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static PublicSuffixDatabase Open(string filename) =>
            new PublicSuffixDatabase(filename);

        /// <summary>
        /// This method provides a fluid factory for asynchronously reading the structure from the filesystem
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static Task<PublicSuffixDatabase> OpenAsync(string filename) =>
            new PublicSuffixDatabase().ReadFromFileAsync(filename);

        /// <summary>
        /// This method provides a fluid factory for storing the structure to the filesystem
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static PublicSuffixDatabase Store(string filename, PublicSuffix instance) =>
            new PublicSuffixDatabase(filename, instance);

        /// <summary>
        /// This method provides a fluid factory for asynchronously storing the structure to the filesystem
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static Task<PublicSuffixDatabase> StoreAsync(string filename, PublicSuffix instance) =>
            new PublicSuffixDatabase()
                .WithCustomTopLevelDomains(instance.CustomTopLevelDomains())
                .WithLastRefreshTimeStamp(instance.LastRefresh())
                .WithNextRefreshTimeStamp(instance.NextRefresh())
                .WithTopLevelDomains(instance.TopLevelDomains())
                .WriteToFileAsync(filename);

        /// <summary>
        /// This method instantiates an empty class
        /// </summary>
        public PublicSuffixDatabase() { }

        /// <summary>
        /// This method instantiates and populates the class from an existing file
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public PublicSuffixDatabase(string filename) =>
            ReadFromFile(filename);

        /// <summary>
        /// This method instantites and populates the class from an existing external instaance
        /// then write the file to the filesystem for persistence
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="instance"></param>
        public PublicSuffixDatabase(string filename, PublicSuffix instance)
        {
            // Set the custom top-level domains into the instance
            CustomTopLevelDomains = instance.CustomTopLevelDomains();
            // Set the last refreshed timestamp into the instance
            LastRefresh = instance.LastRefresh();
            // Set the next refresh timestamp into the instance
            NextRefresh = instance.NextRefresh();
            // Set the top-level domains into the instance
            TopLevelDomains = instance.TopLevelDomains();
            // Write the file to the filesystem
            WriteToFile(filename);
        }

        /// <summary>
        /// This method reads the construct from the filesystem
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public PublicSuffixDatabase ReadFromFile(string filename)
        {
            // Instantiate our stream reader
            using StreamReader streamReader = new StreamReader(filename);
            // Read the file into memory
            string json = streamReader.ReadToEnd();
            // Check the content
            if (!string.IsNullOrEmpty(json) && !string.IsNullOrWhiteSpace(json))
            {
                // Deserialize the file
                PublicSuffixDatabase fileInstance = JsonConvert.DeserializeObject<PublicSuffixDatabase>(json);
                // Set the custom top-level domains into the instance
                CustomTopLevelDomains = fileInstance.CustomTopLevelDomains;
                // Set the last refreshed timestamp into the instance
                LastRefresh = fileInstance.LastRefresh;
                // Set the next refresh timestamp into the instance
                NextRefresh = fileInstance.NextRefresh;
                // Set the top-level domains into the instance
                TopLevelDomains = fileInstance.TopLevelDomains;
            }
            // We're done, return the instance
            return this;
        }

        /// <summary>
        /// This method asynchronously reads the construct from the filesystem
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public async Task<PublicSuffixDatabase> ReadFromFileAsync(string filename)
        {
            // Instantiate our stream reader
            using StreamReader streamReader = new StreamReader(filename);
            // Read the file into memory
            string json = await streamReader.ReadToEndAsync();
            // Check the content
            if (!string.IsNullOrEmpty(json) && !string.IsNullOrWhiteSpace(json))
            {
                // Deserialize the file
                PublicSuffixDatabase fileInstance = JsonConvert.DeserializeObject<PublicSuffixDatabase>(json);
                // Set the custom top-level domains into the instance
                CustomTopLevelDomains = fileInstance.CustomTopLevelDomains;
                // Set the last refreshed timestamp into the instance
                LastRefresh = fileInstance.LastRefresh;
                // Set the next refresh timestamp into the instance
                NextRefresh = fileInstance.NextRefresh;
                // Set the top-level domains into the instance
                TopLevelDomains = fileInstance.TopLevelDomains;
            }
            // We're done with the file reader, close it
            streamReader.Close();
            // We're done, return the instance
            return this;
        }

        /// <summary>
        /// This method fluidly resets the custom top-level domains into the instance
        /// </summary>
        /// <param name="customTopLevelDomains"></param>
        /// <returns></returns>
        public PublicSuffixDatabase WithCustomTopLevelDomains(IEnumerable<string> customTopLevelDomains)
        {
            // Reset the custom top-level domains into the instance
            CustomTopLevelDomains = customTopLevelDomains.ToList();
            // We're done, return the instance
            return this;
        }

        /// <summary>
        /// This method fluidly resets the last refreshed timestamp into the instance
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public PublicSuffixDatabase WithLastRefreshTimeStamp(DateTime? timestamp)
        {
            // Reset the last refreshed timestamp into the instance
            LastRefresh = timestamp;
            // We're done, return the instance
            return this;
        }

        /// <summary>
        /// This method fluidly resets the next refresh timestamp into the instance
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public PublicSuffixDatabase WithNextRefreshTimeStamp(DateTime? timestamp)
        {
            // Reset the next refresh timestamp into the instance
            NextRefresh = timestamp;
            // We're done, return the instance
            return this;
        }

        /// <summary>
        /// This method fluidly resets the top-level domains into the instance
        /// </summary>
        /// <param name="topLevelDomains"></param>
        /// <returns></returns>
        public PublicSuffixDatabase WithTopLevelDomains(IEnumerable<string> topLevelDomains)
        {
            // Reset the top-level domains into the instance
            TopLevelDomains = topLevelDomains.ToList();
            // We're done, return the instance
            return this;
        }

        /// <summary>
        /// This method writes the construct to the file system
        /// </summary>
        /// <returns></returns>
        public PublicSuffixDatabase WriteToFile(string filename)
        {
            // Instantiate our stream writer
            using StreamWriter streamWriter = new StreamWriter(filename);
            // Write the content to the file
            streamWriter.Write(JsonConvert.SerializeObject(this, Formatting.None));
            // We're done with the file, close it
            streamWriter.Close();
            // We're done, return the instance
            return this;
        }

        /// <summary>
        /// This method asynchronously writes the construct to the file system
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public async Task<PublicSuffixDatabase> WriteToFileAsync(string filename)
        {
            // Instantiate our stream writer
            await using StreamWriter streamWriter = new StreamWriter(filename);
            // Write the content to the file
            await streamWriter.WriteAsync(JsonConvert.SerializeObject(this, Formatting.None));
            // We're done with the file, close it
            streamWriter.Close();
            // We're done, return the instance
            return this;
        }
    }
}
