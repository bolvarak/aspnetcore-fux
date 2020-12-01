using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Fux.Example
{
    /// <summary>
    /// This class maintains the ingress into our program
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// This property contains the JSON serializer settings used throughout the examples
        /// </summary>
        public static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Include,
            ReferenceLoopHandling = ReferenceLoopHandling.Serialize
        };

        /// <summary>
        /// This method maintains the main event loop
        /// </summary>
        /// <param name="arguments"></param>
        static async Task Main(string[] arguments)
        {
            // Execute our Docker secret example(s) asynchronously
            await Runner.ExecuteAsync<Config.Docker>();
            // Execute our Environment variable example(s) asynchronously
            await Runner.ExecuteAsync<Config.Environment>();
            // Execute our Redis example(s) asynchronously
            await Runner.ExecuteAsync<Config.Redis>();
            // Execute our DNS hostname example(s) asynchronously
            await Runner.ExecuteAsync<Dns.Hostname>();
        }
    }
}
