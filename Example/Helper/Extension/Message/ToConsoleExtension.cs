namespace Fux.Example.Helper.Extension.Message
{
    /// <summary>
    /// This class maintains the extension for logging a Runner Message to console
    /// </summary>
    public static class ToConsoleExtension
    {
        /// <summary>
        /// This method writes a Message structure to STDOUT
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Helper.Message ToConsole(this Helper.Message source)
        {
            // Localize the string representation of the message
            string message = source.ToString();
            // Write the message to console
            System.Console.WriteLine(message);
            // We're done, return the instance
            return source;
        }
    }
}
