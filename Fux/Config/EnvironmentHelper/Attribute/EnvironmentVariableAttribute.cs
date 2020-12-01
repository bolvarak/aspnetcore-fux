using Fux.Core.Attribute;

namespace Fux.Config.EnvironmentHelper.Attribute
{
    /// <summary>
    /// This attributes maintains correspondences between Environment Variables and POCO properties
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Field | System.AttributeTargets.Property | System.AttributeTargets.Struct)]
    public class EnvironmentVariableAttribute : FromKeyAttribute
    {
        /// <summary>
        /// This method instantiates our attribute with a variable name
        /// </summary>
        /// <param name="variableName"></param>
        /// <param name="allowEmpty"></param>
        public EnvironmentVariableAttribute(string variableName, bool allowEmpty = true) : base(variableName, allowEmpty) { }
    }
}
