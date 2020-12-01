namespace Fux.Core.Extension.String
{
    /// <summary>
    /// This class provides the string.Mask(char) functionality
    /// </summary>
    public static class MaskExtension
    {
        /// <summary>
        /// This method masks a string <paramref name="source"/> with <paramref name="maskCharacter"/>
        /// </summary>
        /// <param name="source"></param>
        /// <param name="maskCharacter"></param>
        /// <returns></returns>
        public static string Mask(this string source, char maskCharacter = '*') =>
            new string(maskCharacter, source.Length);
    }
}
