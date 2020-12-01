namespace Fux.Example.Helper.Extension.Exception
{
    /// <summary>
    ///
    /// </summary>
    public static class ToMessageExtension
    {
        /// <summary>
        /// This extension converts an exception to a Runner Message structure
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Helper.Message ToMessage(this System.Exception source) =>
            Helper.Message.New(source);
    }
}
