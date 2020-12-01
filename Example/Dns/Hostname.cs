using System.Threading.Tasks;

namespace Fux.Example.Dns
{
    /// <summary>
    /// This class is responsible for executing a Fux.Dns.Hostname example
    /// </summary>
    public class Hostname : Runner
    {
        /// <summary>
        /// This property contains the custom domain with which to test
        /// </summary>
        /// <value></value>
        private static readonly string TestCustomDomain = "internal.fux";

        /// <summary>
        /// This property contains a custom hostname and TLD with which to test
        /// </summary>
        /// <value></value>
        private static readonly string TestHostnameCustom = $"asgard.{TestCustomDomain}";

        /// <summary>
        /// This property contains an invalid hostname with which to test
        /// </summary>
        private static readonly string TestHostnameInValid = "moo.bleh.fubar";

        /// <summary>
        /// This property contains a valid hostname with which to test
        /// </summary>
        private static readonly string TestHostnameValid = "asgard.fux.app";

        /// <summary>
        /// This method executes our test on a custom TLD
        /// </summary>
        private void Custom()
        {
            // Add our custom domain
            Fux.Dns.Hostname.WithCustomTopLevelDomain(TestCustomDomain);
            // Execute our custom test model
            Fux.Dns.Model.Hostname customTld = Fux.Dns.Hostname.Parse(TestHostnameCustom);
            // Log the message
            LogMessage(Helper.Message.New("[CustomTld] => ${customTld}")
                .WithDataObject("customTld", customTld));
        }

        /// <summary>
        /// This method asynchronously executes our test on a custom TLD
        /// </summary>
        private async Task CustomAsync()
        {
            // Add our custom domain
            Fux.Dns.Hostname.WithCustomTopLevelDomain(TestCustomDomain);
            // Execute our custom test model
            Fux.Dns.Model.Hostname customTld = await Fux.Dns.Hostname.ParseAsync(TestHostnameCustom);
            // Log the message
            LogMessage(Helper.Message.New("[CustomTld] => ${customTld}")
                .WithDataObject("customTld", customTld));
        }

        /// <summary>
        /// This method executes our test on an invalid TLD
        /// </summary>
        private void Invalid()
        {
            // Execute our invalid test model
            Fux.Dns.Model.Hostname invalidTld = Fux.Dns.Hostname.Parse(TestHostnameInValid);
            // Log the message
            LogMessage(Helper.Message.New("[InvalidTld] => ${invalidTld}").WithDataObject("invalidTld", invalidTld));
        }

        /// <summary>
        /// This method asynchronously executes our test on an invalid TLD
        /// </summary>
        private async Task InvalidAsync()
        {
            // Execute our invalid test model
            Fux.Dns.Model.Hostname invalidTld = await Fux.Dns.Hostname.ParseAsync(TestHostnameInValid);
            // Log the message
            LogMessage(Helper.Message.New("[InvalidTld] => ${invalidTld}").WithDataObject("invalidTld", invalidTld));
        }

        /// <summary>
        /// This method executes our test on a valid TLD
        /// </summary>
        private void Valid()
        {
            // Execute our valid test model
            Fux.Dns.Model.Hostname validTld = Fux.Dns.Hostname.Parse(TestHostnameValid);
            // Log the message
            LogMessage(Helper.Message.New("[ValidTld] => ${validTld}").WithDataObject("validTld", validTld));
        }

        /// <summary>
        /// This method asynchronously executes our test on a valid TLD
        /// </summary>
        private async Task ValidAsync()
        {
            // Execute our valid test model
            Fux.Dns.Model.Hostname validTld = await Fux.Dns.Hostname.ParseAsync(TestHostnameValid);
            // Log the message
            LogMessage(Helper.Message.New("[ValidTld] => ${validTld}").WithDataObject("validTld", validTld));
        }

        /// <summary>
        /// This method runs the PublicSuffix Hostname example(s)
        /// </summary>
        /// <param name="ticker"></param>
        /// <returns></returns>
        protected override void Task(Core.Ticker ticker)
        {
            // Execute our valid test model
            Valid();
            // Execute our invalid test model
            Invalid();
            // Execute our custom test model
            Custom();
        }

        /// <summary>
        /// This method asynchronously runs the PublicSuffix Hostname example(s)
        /// </summary>
        /// <param name="ticker"></param>
        /// <returns></returns>
        protected override async Task TaskAsync(Core.Ticker ticker)
        {
            // Run the valid test model
            await ValidAsync();
            // Run the invalid test model
            await InvalidAsync();
            // Run the custom test model
            await CustomAsync();
        }
    }
}
