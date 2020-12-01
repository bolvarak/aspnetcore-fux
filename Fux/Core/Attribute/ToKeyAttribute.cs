namespace Fux.Core.Attribute
{
    /// <summary>
    /// This attribute maintains the correlation between properties when mapping to other objects
    /// </summary>
    public class ToKeyAttribute : ToPropertyAttribute
    {
        /// <summary>
        /// This method instantiates our attribute with a key name
        /// </summary>
        /// <param name="keyName"></param>
        /// <param name="allowEmptyValue"></param>
        public ToKeyAttribute(string keyName, bool allowEmptyValue = true) : base(keyName, allowEmptyValue) { }
    }
}
