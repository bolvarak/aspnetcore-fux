using System;
using Fux.Config.EnvironmentHelper.Attribute;

namespace Fux.Config
{
    /// <summary>
    /// This class maintains the structure for the Environment configuration service provider
    /// </summary>
    public class Environment
    {
        /// <summary>
        /// This method loads a POCO from an Environment Variable stored as a JSON string using a
        /// <code>DockerSecretName</code> attribute at the class level
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public static TValue Get<TValue>()
        {
            // Load the value's type
            Type type = typeof(TValue);
            // Localize the custom attributes
            EnvironmentVariableAttribute[] attributes =
                (type.GetCustomAttributes(typeof(EnvironmentVariableAttribute), true) as EnvironmentVariableAttribute[]);
            // Localize the key attribute
            EnvironmentVariableAttribute keyAttribute =
                attributes[0] ?? null;
            // Make sure we have a key attribute
            if (keyAttribute == null) throw new Exception($"{typeof(TValue).Name} Does Not Contain a EnvironmentVariable Attribute");
            // Load the value from the environment
            TValue value = Get<TValue>(keyAttribute.Name, keyAttribute.AllowEmpty);
            // We're done, return the value
            return value;
        }

        /// <summary>
        /// This method loads an environment variable from the current context
        /// via <paramref name="variableName"/> with the option to throw an
        /// exception if <paramref name="allowEmpty"/> is <code>false</code>
        /// </summary>
        /// <param name="variableName"></param>
        /// <param name="allowEmpty"></param>
        /// <exception cref="System.Exception"></exception>
        /// <returns></returns>
        public static string Get(string variableName, bool allowEmpty = true)
        {
            // Localize the environment variable's value
            string environmentVariable =
                System.Environment.GetEnvironmentVariable(variableName);
            // Check the value and empty flag
            if (!allowEmpty && environmentVariable == null)
                throw new Exception($"Environment Variable Cannot Be Empty [{variableName}]");
            // We're done, return the string value
            return environmentVariable?.ToString();
        }

        /// <summary>
        /// This method loads an environment variable, statically typed to <paramref name="type"/>
        /// from the current context via <paramref name="variableName"/> with the option to throw
        /// an exception if <paramref name="allowEmpty"/> is <code>false</code>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="variableName"></param>
        /// <param name="allowEmpty"></param>
        /// <returns></returns>
        public static dynamic Get(Type type, string variableName, bool allowEmpty = true) =>
            Core.Convert.FromString(type, Get(variableName, allowEmpty));

        /// <summary>
        /// This method loads an environment variable, statically typed to <typeparamref name="TValue"/>
        /// from the current context via <paramref name="variableName"/> with the option to throw
        /// an exception if <paramref name="allowEmpty"/> is <code>false</code>
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="variableName"></param>
        /// <param name="allowEmpty"></param>
        /// <returns></returns>
        public static TValue Get<TValue>(string variableName, bool allowEmpty = true) =>
            Core.Convert.FromString<TValue>(Get(variableName, allowEmpty));

        /// <summary>
        /// This method loads and populates an object from Environment Variables using the <code>EnvironmentKeyAttribute</code>
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public static TValue GetObject<TValue>() where TValue: class, new() =>
            Core.Convert.MapWithValueGetter<TValue, EnvironmentVariableAttribute>((attribute, type, currentValue) =>
                Get(type, attribute.Name, attribute.AllowEmpty));

        /// <summary>
        /// This method sets <paramref name="value"/> into the environment as <paramref name="variableName"/>
        /// </summary>
        /// <param name="variableName"></param>
        /// <param name="value"></param>
        public static void Set(string variableName, string value) =>
            System.Environment.SetEnvironmentVariable(variableName, value);

        /// <summary>
        /// This method sets <paramref name="value"/> of <paramref name="type"/> into the environment as <paramref name="variableName"/>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="variableName"></param>
        /// <param name="value"></param>
        public static void Set(Type type, string variableName, dynamic value) =>
            Set(variableName, Core.Convert.ToString(type, value));

        /// <summary>
        /// This method sets <paramref name="value"/> of <typeparamref name="TValue"/> into the environment as <paramref name="variableName"/>
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="variableName"></param>
        /// <param name="value"></param>
        public static void Set<TValue>(string variableName, TValue value) =>
            Set(variableName, Core.Convert.ToString<TValue>(value));

        /// <summary>
        /// This method loads and populates an object from Environment Variables using the <code>EnvironmentVariableAttribute</code>
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <returns></returns>
        public static void SetObject<TType>() where TType : class, new() =>
            // Generate the instance
            Core.Convert.MapWithValueGetter<TType, EnvironmentVariableAttribute>((attribute, type, value) => {
                // Set the value into the environment
                Set(type, attribute.Name, value);
                // We're done, just return the current value
                return value;
            }, false);
    }
}
