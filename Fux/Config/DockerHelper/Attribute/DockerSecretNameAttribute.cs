using Fux.Core.Attribute;

namespace Fux.Config.DockerHelper.Attribute
{
    /// <summary>
    /// This attributes maintains correspondences between Docker secrets an POCO properties
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Field | System.AttributeTargets.Property | System.AttributeTargets.Struct)]
    public class DockerSecretNameAttribute : FromKeyAttribute
    {
        /// <summary>
        /// This method instantiates the attribute class with a secret name
        /// </summary>
        /// <param name="secretName"></param>
        /// <param name="allowEmptyValue"></param>
        public DockerSecretNameAttribute(string secretName, bool allowEmptyValue = true) : base(secretName, allowEmptyValue) { }
    }
}
