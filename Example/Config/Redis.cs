using System.Threading.Tasks;
using Fux.Config.RedisHelper;
using Fux.Example.Config.Model;

namespace Fux.Example.Config
{
    /// <summary>
    /// This class is responsible for executing a Fux.Config.Redis example
    /// </summary>
    public class Redis : Runner
    {
        /// <summary>
        /// This property contains our Redis configuration
        /// </summary>
        private readonly EnvironmentConnection<Model.Redis.Configuration> _redis =
            Fux.Config.Redis.ConnectFromEnvironment<Model.Redis.Configuration>();

        /// <summary>
        /// This method retrieves a Redis value as a boolean
        /// </summary>
        private void Boolean()
        {
            // Localize the Redis object
            bool boolean = _redis.Get<bool>("fux-example-config-redis-boolean");
            // Log the message
            LogMessage(Helper.Message.New("[RedisBoolean] => ${boolean}")
                .WithDataObject("boolean", boolean));
        }

        /// <summary>
        /// This method asynchronously retrieves a Redis value as a boolean
        /// </summary>
        private async Task BooleanAsync()
        {
            // Localize the Redis object
            bool boolean = await _redis.GetAsync<bool>("fux-example-config-redis-boolean");
            // Log the message
            LogMessage(Helper.Message.New("[RedisBoolean] => ${boolean}")
                .WithDataObject("boolean", boolean));
        }

        /// <summary>
        /// This method retrieves a Redis value as a double
        /// </summary>
        private void Double()
        {
            // Localize the Redis object
            double dbl = _redis.Get<double>("fux-example-config-redis-double");
            // Log the message
            LogMessage(Helper.Message.New("[RedisDouble] => ${double}")
                .WithDataObject("double", dbl));
        }

        /// <summary>
        /// This method asynchronously retrieves a Redis value as a double
        /// </summary>
        private async Task DoubleAsync()
        {
            // Localize the Redis object
            double dbl = await _redis.GetAsync<double>("fux-example-config-redis-double");
            // Log the message
            LogMessage(Helper.Message.New("[RedisDouble] => ${double}")
                .WithDataObject("double", dbl));
        }

        /// <summary>
        /// This method retrieves a Redis value as an integer
        /// </summary>
        private void Integer()
        {
            // Localize the Redis object
            int integer = _redis.Get<int>("fux-example-config-redis-integer");
            // Log the message
            LogMessage(Helper.Message.New("[RedisInteger] => ${integer}")
                .WithDataObject("integer", integer));
        }

        /// <summary>
        /// This method asynchronously retrieves a Redis value as an integer
        /// </summary>
        private async Task IntegerAsync()
        {
            // Localize the Redis object
            int integer = await _redis.GetAsync<int>("fux-example-config-redis-integer");
            // Log the message
            LogMessage(Helper.Message.New("[RedisInteger] => ${integer}")
                .WithDataObject("integer", integer));
        }

        /// <summary>
        /// This method retrieves a Redis key and deserializes it into a POCO
        /// </summary>
        /// <returns></returns>
        private void KeyObject()
        {
            // Localize the Redis key object
            KeyObject keyObject = _redis.Get<KeyObject>();
            // Log the message
            LogMessage(Helper.Message.New("[RedisKeyObject] => ${keyObject}").WithDataObject("keyObject", keyObject));
        }

        /// <summary>
        /// This method asynchronously retrieves a Redis key and deserializes it into a POCO
        /// </summary>
        /// <returns></returns>
        private async Task KeyObjectAsync()
        {
            // Localize the Redis key object
            KeyObject keyObject = await _redis.GetAsync<KeyObject>();
            // Log the message
            LogMessage(Helper.Message.New("[RedisKeyObject] => ${keyObject}").WithDataObject("keyObject", keyObject));
        }

        /// <summary>
        /// This method retrieves keys from Redis defined as attributes in a POCO
        /// and then populates and returns the POCO
        /// </summary>
        /// <returns></returns>
        private void Object()
        {
            // Localize the Redis object
            Object obj = _redis.WithDatabaseAtIndex(15).GetObject<Object>();
            // Log the message
            LogMessage(Helper.Message.New("[RedisObject] => ${object}").WithDataObject("object", obj));
        }

        /// <summary>
        /// This method asynchronously retrieves keys from Redis defined as
        /// attributes in a POCO and then populates and returns the POCO
        /// </summary>
        /// <returns></returns>
        private async Task ObjectAsync()
        {
            // Localize the Redis object
            Object obj = await _redis.WithDatabaseAtIndex(15).GetObjectAsync<Object>();
            // Log the message
            LogMessage(Helper.Message.New("[RedisObject] => ${object}").WithDataObject("object", obj));
        }

        /// <summary>
        /// This task retrieves a string from a Redis key.
        /// </summary>
        /// <returns></returns>
        private void String()
        {
            // Localize the Redis string
            string str = _redis.Get("fux-example-config-redis-key");
            // Log the message
            LogMessage(
                Helper.Message
                    .New("[RedisString] => ${string}")
                    .WithDataObject("string", str));
        }

        /// <summary>
        /// This task asynchronously retrieves a string from a Redis key.
        /// </summary>
        /// <returns></returns>
        private async Task StringAsync()
        {
            // Localize the Redis string
            string str = await _redis.GetAsync("fux-example-config-redis-key");
            // Log the message
            LogMessage(Helper.Message.New("[RedisString] => ${string}").WithDataObject("string", str));
        }

        /// <summary>
        /// This method runs the Redis example
        /// </summary>
        /// <param name="ticker"></param>
        /// <returns></returns>
        protected override void Task(Core.Ticker ticker)
        {
            // Run the Redis boolean task
            Boolean();
            // Run the Redis double task
            Double();
            // Run the Redis integer task
            Integer();
            // Run the Redis key object task
            KeyObject();
            // Run the Redis object task
            Object();
            // Run the Redis string task
            String();
        }

        /// <summary>
        /// This method runs the Redis example asynchronously
        /// </summary>
        /// <param name="ticker"></param>
        /// <returns></returns>
        protected override async Task TaskAsync(Core.Ticker ticker)
        {
            // Run the Redis boolean task
            await BooleanAsync();
            // Run the Redis double task
            await DoubleAsync();
            // Run the Redis integer task
            await IntegerAsync();
            // Run the Redis key object task
            await KeyObjectAsync();
            // Run the Redis object task
            await ObjectAsync();
            // Run the Redis string task
            await StringAsync();
        }
    }
}
