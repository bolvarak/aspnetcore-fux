using System.Threading.Tasks;
using Fux.Example.Config.Model;

namespace Fux.Example.Config
{
    /// <summary>
    /// This class is responsible for executing a Fux.Config.Environment example
    /// </summary>
    public class Environment : Runner
    {
        /// <summary>
        /// This method retrieves an Environment variable value as a boolean
        /// </summary>
        private void Boolean()
        {
            // Localize the Environment variable object
            bool boolean = Fux.Config.Environment.Get<bool>("FUX_EXAMPLE_CONFIG_BOOLEAN");
            // Log the message
            LogMessage(Helper.Message.New("[EnvironmentVariableBoolean] => ${boolean}")
                .WithDataObject("boolean", boolean));
        }

        /// <summary>
        /// This method retrieves an Environment variable value as a double
        /// </summary>
        private void Double()
        {
            // Localize the Environment variable object
            double dbl = Fux.Config.Environment.Get<double>("FUX_EXAMPLE_CONFIG_DOUBLE");
            // Log the message
            LogMessage(Helper.Message.New("[EnvironmentVariableDouble] => ${double}")
                .WithDataObject("double", dbl));
        }

        /// <summary>
        /// This method retrieves an Environment variable value as an integer
        /// </summary>
        private void Integer()
        {
            // Localize the Environment variable object
            int integer = Fux.Config.Environment.Get<int>("FUX_EXAMPLE_CONFIG_INTEGER");
            // Log the message
            LogMessage(Helper.Message.New("[EnvironmentVariableInteger] => ${integer}")
                .WithDataObject("integer", integer));
        }

        /// <summary>
        /// This method retrieves an Environment variable and deserializes it into a POCO
        /// </summary>
        /// <returns></returns>
        private void KeyObject()
        {
            // Localize the Environment variable object
            KeyObject keyObject = Fux.Config.Environment.Get<KeyObject>();
            // Log the message
            LogMessage(Helper.Message.New("[EnvironmentVariableKeyObject] => ${keyObject}").WithDataObject("keyObject", keyObject));
        }

        /// <summary>
        /// This method retrieves keys from an Environment variable defined as attributes in a POCO
        /// and then populates and returns the POCO
        /// </summary>
        /// <returns></returns>
        private void Object()
        {
            // Localize the Environment variable object
            Object obj = Fux.Config.Environment.GetObject<Object>();
            // Log the message
            LogMessage(Helper.Message.New("[EnvironmentVariableObject] => ${object}").WithDataObject("object", obj));
        }

        /// <summary>
        /// This task retrieves a string from an Environment variable
        /// </summary>
        /// <returns></returns>
        private void String()
        {
            // Localize the Environment variable
            string str = Fux.Config.Environment.Get("FUX_EXAMPLE_CONFIG_STRING");
            // Log the message
            LogMessage(
                Helper.Message
                    .New("[EnvironmentVariableString] => ${string}")
                    .WithDataObject("string", str));
        }

        /// <summary>
        /// This method runs the Environment variable example
        /// </summary>
        /// <param name="ticker"></param>
        /// <returns></returns>
        protected override void Task(Core.Ticker ticker)
        {
            // Run the Environment variable boolean task
            Boolean();
            // Run the Environment variable double task
            Double();
            // Run the Environment variable integer task
            Integer();
            // Run the Environment variable key object task
            KeyObject();
            // Run the Environment variable object task
            Object();
            // Run the Environment variable string task
            String();
        }

        /// <summary>
        /// This method asynchronously runs the Environment variable example
        /// </summary>
        /// <param name="ticker"></param>
        /// <returns></returns>
        protected override Task TaskAsync(Core.Ticker ticker) =>
            System.Threading.Tasks.Task.Run(() => Task(ticker));
    }
}
