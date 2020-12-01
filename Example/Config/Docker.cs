using System.Threading.Tasks;
using Fux.Example.Config.Model;

namespace Fux.Example.Config
{
    /// <summary>
    /// This class is responsible for executing a Fux.Config.Docker example
    /// </summary>
    public class Docker : Runner
    {
        /// <summary>
        /// This property is for testing outside of a Docker environment only
        /// in the library, this defaults to /run/secrets
        /// </summary>
        private static string DockerSecretDirectory = "/home/tbrown/.secrets";

        /// <summary>
        /// This method retrieves a Docker secret value as a boolean
        /// </summary>
        private void Boolean()
        {
            // Localize the Docker secret object
            bool boolean = Fux.Config.Docker.Get<bool>("fux-example-config-docker-boolean");
            // Log the message
            LogMessage(Helper.Message.New("[DockerSecretBoolean] => ${boolean}")
                .WithDataObject("boolean", boolean));
        }

        /// <summary>
        /// This method asynchronously retrieves a Docker secret value as a boolean
        /// </summary>
        /// <returns></returns>
        private async Task BooleanAsync()
        {
            // Localize the Docker secret object
            bool boolean = await Fux.Config.Docker.GetAsync<bool>("fux-example-config-docker-boolean");
            // Log the message
            LogMessage(Helper.Message.New("[DockerSecretBoolean] => ${boolean}")
                .WithDataObject("boolean", boolean));
        }

        /// <summary>
        /// This method retrieves a Docker secret value as a double
        /// </summary>
        private void Double()
        {
            // Localize the Docker secret object
            double dbl = Fux.Config.Docker.Get<double>("fux-example-config-docker-double");
            // Log the message
            LogMessage(Helper.Message.New("[DockerSecretDouble] => ${double}")
                .WithDataObject("double", dbl));
        }

        /// <summary>
        /// This method asynchronously retrieves a Docker secret value as a double
        /// </summary>
        /// <returns></returns>
        private async Task DoubleAsync()
        {
            // Localize the Docker secret object
            double dbl = await Fux.Config.Docker.GetAsync<double>("fux-example-config-docker-double");
            // Log the message
            LogMessage(Helper.Message.New("[DockerSecretDouble] => ${double}")
                .WithDataObject("double", dbl));
        }

        /// <summary>
        /// This method retrieves a Docker secret value as an integer
        /// </summary>
        private void Integer()
        {
            // Localize the Docker secret object
            int integer = Fux.Config.Docker.Get<int>("fux-example-config-docker-integer");
            // Log the message
            LogMessage(Helper.Message.New("[DockerSecretInteger] => ${integer}")
                .WithDataObject("integer", integer));
        }

        /// <summary>
        /// This method asynchronously retrieves a Docker secret value as an integer
        /// </summary>
        private async Task IntegerAsync()
        {
            // Localize the Docker secret object
            int integer = await Fux.Config.Docker.GetAsync<int>("fux-example-config-docker-integer");
            // Log the message
            LogMessage(Helper.Message.New("[DockerSecretInteger] => ${integer}")
                .WithDataObject("integer", integer));
        }

        /// <summary>
        /// This method retrieves a Docker secret and deserializes it into a POCO
        /// </summary>
        /// <returns></returns>
        private void KeyObject()
        {
            // Localize the Docker secret object
            KeyObject keyObject = Fux.Config.Docker.Get<KeyObject>();
            // Log the message
            LogMessage(Helper.Message.New("[DockerSecretKeyObject] => ${keyObject}")
                .WithDataObject("keyObject", keyObject));
        }

        /// <summary>
        /// This method asynchronously retrieves a Docker secret and deserializes it into a POCO
        /// </summary>
        /// <returns></returns>
        private async Task KeyObjectAsync()
        {
            // Localize the Docker secret object
            KeyObject keyObject = await Fux.Config.Docker.GetAsync<KeyObject>();
            // Log the message
            LogMessage(Helper.Message.New("[DockerSecretKeyObject] => ${keyObject}")
                .WithDataObject("keyObject", keyObject));
        }

        /// <summary>
        /// This method retrieves keys from a Docker secret defined as attributes in a POCO
        /// and then populates and returns the POCO
        /// </summary>
        /// <returns></returns>
        private void Object()
        {
            // Localize the Docker secret object
            Object obj = Fux.Config.Docker.GetObject<Object>();
            // Log the message
            LogMessage(Helper.Message.New("[DockerSecretObject] => ${object}").WithDataObject("object", obj));
        }

        /// <summary>
        /// This method asynchronously retrieves keys from a Docker secret defined as
        /// attributes in a POCO and then populates and returns the POCO
        /// </summary>
        /// <returns></returns>
        private async Task ObjectAsync()
        {
            // Localize the Docker secret object
            Object obj = await Fux.Config.Docker.GetObjectAsync<Object>();
            // Log the message
            LogMessage(Helper.Message.New("[DockerSecretObject] => ${object}").WithDataObject("object", obj));
        }

        /// <summary>
        /// This task retrieves a string from a Docker secret
        /// </summary>
        /// <returns></returns>
        private void String()
        {
            // Localize the Docker secret
            string str = Fux.Config.Docker.Get("fux-example-config-docker-string");
            // Log the message
            LogMessage(
                Helper.Message
                    .New("[DockerSecretString] => ${string}")
                    .WithDataObject("string", str));
        }

        /// <summary>
        /// This task asynchronously retrieves a string from a Docker secret
        /// </summary>
        /// <returns></returns>
        private async Task StringAsync()
        {
            // Localize the Docker secret
            string str = await Fux.Config.Docker.GetAsync("fux-example-config-docker-key");
            // Log the message
            LogMessage(Helper.Message.New("[DockerSecretString] => ${string}").WithDataObject("string", str));
        }

        /// <summary>
        /// This method runs the Docker secret example
        /// </summary>
        /// <param name="ticker"></param>
        /// <returns></returns>
        protected override void Task(Core.Ticker ticker)
        {
            // Reset the Docker secret path
            Fux.Config.Docker.SetSecretsDirectory(DockerSecretDirectory);
            // Run the Docker secret boolean task
            Boolean();
            // Run the Docker secret double task
            Double();
            // Run the Docker secret integer task
            Integer();
            // Run the Docker secret key object task
            KeyObject();
            // Run the Docker secret object task
            Object();
            // Run the Docker secret string task
            String();
        }

        /// <summary>
        /// This method runs the Docker secret example asynchronously
        /// </summary>
        /// <param name="ticker"></param>
        /// <returns></returns>
        protected override async Task TaskAsync(Core.Ticker ticker)
        {
            // Reset the Docker secret path
            Fux.Config.Docker.SetSecretsDirectory(DockerSecretDirectory);
            // Run the Docker secret boolean task
            await BooleanAsync();
            // Run the Docker secret double task
            await DoubleAsync();
            // Run the Docker secret integer task
            await IntegerAsync();
            // Run the Docker secret key object task
            await KeyObjectAsync();
            // Run the Docker secret object task
            await ObjectAsync();
            // Run the Docker secret string task
            await StringAsync();
        }
    }
}
