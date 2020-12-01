using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Fux.Config.DockerHelper.Attribute;

namespace Fux.Config
{
    /// <summary>
    /// This class maintains the configuration for Docker
    /// </summary>
    public class Docker
    {
        /// <summary>
        /// This property contains the dictionary of secrets from Docker
        /// </summary>
        private static readonly Dictionary<string, string> _secrets = new Dictionary<string, string>();

        /// <summary>
        /// This property contains the path to the Docker secrets
        /// </summary>
        private static string _secretsDirectory =
            (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ?
                $"{Path.DirectorySeparatorChar}ProgramData{Path.DirectorySeparatorChar}Docker{Path.DirectorySeparatorChar}secrets"
                    : $"{Path.DirectorySeparatorChar}run{Path.DirectorySeparatorChar}secrets");

        /// <summary>
        /// This method normalizes a secret's name
        /// </summary>
        /// <param name="secretName"></param>
        /// <returns></returns>
        private static string NormalizeSecretName(string secretName) =>
            secretName.ToLower().Replace(_secretsDirectory.ToLower(), "").TrimStart(Path.DirectorySeparatorChar);

        /// <summary>
        /// This method reads the secrets from the file system
        /// </summary>
        private static void ReadSecrets()
        {
            // Check the secrets in memory and populate them
            if (_secrets.Count == 0)
                foreach (string s in Directory.GetFiles(_secretsDirectory))
                    _secrets[NormalizeSecretName(s)] = File.ReadAllText(s).Trim();
        }

        /// <summary>
        /// This method asynchronously reads the secrets from the file system
        /// </summary>
        /// <returns></returns>
        private static async Task ReadSecretsAsync()
        {
            // Check the secrets in memory and populate them
            if (_secrets.Count == 0)
                foreach (string s in Directory.GetFiles(_secretsDirectory))
                    _secrets[NormalizeSecretName(s)] = (await File.ReadAllTextAsync(s)).Trim();
        }

        /// <summary>
        /// This method sets a secret into the instance and
        /// tries to write the secret to the filesystem
        /// </summary>
        /// <param name="secretName"></param>
        /// <param name="secretValue"></param>
        private static void WriteSecret(string secretName, string secretValue)
        {
            // Try to write the secret to the filesystem
            try
            {
                // Write the secret to the filesystem
                File.WriteAllText($"{_secretsDirectory}{Path.DirectorySeparatorChar}{secretName.ToLower()}", secretValue);
            }
            catch (Exception) { /* Fail Gracefully */ }
            // Reset the secret into the instance
            _secrets[NormalizeSecretName(secretName)] = secretValue;
        }

        /// <summary>
        /// This method sets a secret into the instance and asynchronously
        /// tries to write the secret to the filesystem
        /// </summary>
        /// <param name="secretName"></param>
        /// <param name="secretValue"></param>
        /// <returns></returns>
        private static async Task WriteSecretAsync(string secretName, string secretValue)
        {
            // Try to write the secret to the filesystem
            try
            {
                // Write the secret to the filesystem
                await File.WriteAllTextAsync($"{_secretsDirectory}{Path.DirectorySeparatorChar}{secretName.ToLower()}", secretValue);
            }
            catch (Exception) { /* Fail Gracefully */ }
            // Reset the secret into the instance
            _secrets[NormalizeSecretName(secretName)] = secretValue;
        }

        /// <summary>
        /// This method resets the secrets directory path into memory
        /// </summary>
        /// <param name="directoryPath"></param>
        public static void SetSecretsDirectory(string directoryPath) =>
            _secretsDirectory = Path.GetFullPath(directoryPath);

        /// <summary>
        /// This method loads a POCO from a Docker Secret stored as a JSON string using a
        /// <code>DockerSecretName</code> attribute at the class level
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public static TValue Get<TValue>()
        {
            // Load the value's type
            Type type = typeof(TValue);
            // Localize the key attribute
            DockerSecretNameAttribute keyAttribute =
                (type.GetCustomAttributes(typeof(DockerSecretNameAttribute), true).FirstOrDefault() as DockerSecretNameAttribute);
            // Make sure we have a key attribute
            if (keyAttribute == null) throw new Exception($"{typeof(TValue).Name} Does Not Contain a DockerSecretName Attribute");
            // Load the value from the Docker Secrets
            TValue value = Get<TValue>(keyAttribute.Name, keyAttribute.AllowEmpty);
            // We're done, return the value
            return value;
        }

        /// <summary>
        /// This method returns the string value of a Docker secret named <paramref name="secretName"/>
        /// </summary>
        /// <param name="secretName"></param>
        /// <param name="allowEmpty"></param>
        /// <returns></returns>
        public static string Get(string secretName, bool allowEmpty = true)
        {
            // Ensure the secrets are in memory
            ReadSecrets();
            // Localise the value
            string value = _secrets[NormalizeSecretName(secretName)] ?? null;
            // Check the value and empty flag
            if (!allowEmpty && (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value)))
                throw new Exception($"Docker Secret Cannot Be Empty [{secretName}]");
            // Check the value and return null
            if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
                return null;
            // Return the secret
            return value;
        }

        /// <summary>
        /// This method loads a Docker Secret, statically typed to <paramref name="type"/>
        /// from the current context via <paramref name="secretName"/> with the option to throw
        /// an exception if <paramref name="allowEmpty"/> is <code>false</code>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="secretName"></param>
        /// <param name="allowEmpty"></param>
        /// <returns></returns>
        public static dynamic Get(Type type, string secretName, bool allowEmpty = true) =>
            Core.Convert.FromString(type, Get(secretName, allowEmpty));

        /// <summary>
        /// This method returns the typed value of a Docker secret name <paramref name="secretName"/>
        /// </summary>
        /// <param name="secretName"></param>
        /// <param name="allowEmpty"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public static TValue Get<TValue>(string secretName, bool allowEmpty = true) =>
            Core.Convert.FromString<TValue>(Get(secretName, allowEmpty));

        /// <summary>
        /// This method asynchronously loads a POCO from a Docker Secret stored as a JSON string using a
        /// <code>DockerSecretName</code> attribute at the class level
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public static async Task<TValue> GetAsync<TValue>()
        {
            // Load the value's type
            Type type = typeof(TValue);
            // Localize the key attribute
            DockerSecretNameAttribute keyAttribute =
                (type.GetCustomAttributes(typeof(DockerSecretNameAttribute), true).FirstOrDefault() as DockerSecretNameAttribute);
            // Make sure we have a key attribute
            if (keyAttribute == null) throw new Exception($"{typeof(TValue).Name} Does Not Contain a DockerSecretName Attribute");
            // Load the value from the Docker Secret
            TValue value = await GetAsync<TValue>(keyAttribute.Name, keyAttribute.AllowEmpty);
            // We're done, return the value
            return value;
        }

        /// <summary>
        /// This method asynchronously returns the string value of a Docker secret name <paramref name="secretName"/>
        /// </summary>
        /// <param name="secretName"></param>
        /// <param name="allowEmpty"></param>
        /// <returns></returns>
        public static async Task<string> GetAsync(string secretName, bool allowEmpty = true)
        {
            // Ensure the secrets are in memory
            await ReadSecretsAsync();
            // Localise the value
            string value = _secrets[NormalizeSecretName(secretName)] ?? null;
            // Check the value and empty flag
            if (!allowEmpty && (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value)))
                throw new Exception($"Docker Secret Cannot Be Empty [{secretName}]");
            // Check the value and return null
            if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
                return null;
            // Return the secret
            return value;
        }

        /// <summary>
        /// This method asynchronously returns the typed value of a Docker secret named <paramref name="secretName"/>
        /// </summary>
        /// <param name="secretName"></param>
        /// <param name="allowEmpty"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public static async Task<TValue> GetAsync<TValue>(string secretName, bool allowEmpty = true) =>
            Core.Convert.FromString<TValue>(await GetAsync(secretName, allowEmpty));

        /// <summary>
        /// This method asynchronously loads a Docker Secret, statically typed to <paramref name="type"/>
        /// from the current context via <paramref name="secretName"/> with the option to throw
        /// an exception if <paramref name="allowEmpty"/> is <code>false</code>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="secretName"></param>
        /// <param name="allowEmpty"></param>
        /// <returns></returns>
        public static async Task<dynamic> GetAsync(Type type, string secretName, bool allowEmpty = true) =>
            Core.Convert.FromString(type, await GetAsync(secretName, allowEmpty));

        /// <summary>
        /// This method loads and populates an object from Docker using the <code>DockerSecretNameAttribute</code>
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public static TValue GetObject<TValue>() where TValue : class, new() =>
            Core.Convert.MapWithValueGetter<TValue, DockerSecretNameAttribute>((attribute, type, currentValue) =>
                Get(type, attribute.Name, attribute.AllowEmpty));

        /// <summary>
        /// This method asynchronously loads and populates an object from Docker using the <code>DockerSecretNameAttribute</code>
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public static Task<TValue> GetObjectAsync<TValue>() where TValue : class, new() =>
            Core.Convert.MapWithValueGetterAsync<TValue, DockerSecretNameAttribute>(async (attribute, type, currentValue) =>
                await GetAsync(type, attribute.Name, attribute.AllowEmpty));

        /// <summary>
        /// This method saves a POCO to a Docker Secret stored as a JSON string using a
        /// <code>DockerSecretName</code> attribute at the class level
        /// </summary>
        /// <param name="value"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public static void Set<TValue>(TValue value)
        {
            // Localize our type
            Type type = typeof(TValue);
            // Localize the key attribute
            DockerSecretNameAttribute keyAttribute =
                (type.GetCustomAttributes(typeof(DockerSecretNameAttribute), true).FirstOrDefault() as DockerSecretNameAttribute);
            // Make sure we have a key attribute
            if (keyAttribute == null) throw new System.Exception($"{typeof(TValue).Name} Does Not Contain a RedisKey Attribute");
            // Set the value into Docker
            Set<TValue>(keyAttribute.Name, value);
        }

        /// <summary>
        /// This method sets <paramref name="value"/> into the instance and filesystem as <paramref name="secretName"/>
        /// </summary>
        /// <param name="secretName"></param>
        /// <param name="value"></param>
        public static void Set(string secretName, string value) =>
            WriteSecret(secretName, value);

        /// <summary>
        /// This method sets <paramref name="value"/> of <paramref name="type"/> into the instance and filesystem as <paramref name="secretName"/>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="secretName"></param>
        /// <param name="value"></param>
        public static void Set(Type type, string secretName, dynamic value) =>
            Set(secretName, Core.Convert.ToString(type, value));

        /// <summary>
        /// This method sets <paramref name="value"/> of <typeparamref name="TValue"/> into the instance and filesystem as <paramref name="secretName"/>
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="secretName"></param>
        /// <param name="value"></param>
        public static void Set<TValue>(string secretName, TValue value) =>
            Set(secretName, Core.Convert.ToString<TValue>(value));

        /// <summary>
        /// This method populates an object in a Docker Secret using the <code>DockerSecretNameAttribute</code>
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <returns></returns>
        public static void SetObject<TType>() where TType : class, new() =>
            // Generate the instance
            Core.Convert.MapWithValueGetter<TType, DockerSecretNameAttribute>((attribute, type, value) =>
            {
                // Set the value into the instance
                Set(type, attribute.Name, value);
                // We're done, just return the current value
                return value;
            }, false);

        /// <summary>
        /// This method asynchronously saves a POCO to a Docker Secret stored as a JSON string using a
        /// <code>DockerSecretName</code> attribute at the class level
        /// </summary>
        /// <param name="value"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public static Task SetAsync<TValue>(TValue value)
        {
            // Localize our type
            Type type = typeof(TValue);
            // Localize the key attribute
            DockerSecretNameAttribute keyAttribute =
                (type.GetCustomAttributes(typeof(DockerSecretNameAttribute), true).FirstOrDefault() as DockerSecretNameAttribute);
            // Make sure we have a key attribute
            if (keyAttribute == null) throw new System.Exception($"{typeof(TValue).Name} Does Not Contain a DockerSecretName Attribute");
            // Set the value into Docker
            return SetAsync<TValue>(keyAttribute.Name, value);
        }

        /// <summary>
        /// This method asynchronously sets <paramref name="value"/> into the instance and filesystem as <paramref name="secretName"/>
        /// </summary>
        /// <param name="secretName"></param>
        /// <param name="value"></param>
        public static Task SetAsync(string secretName, string value) =>
            WriteSecretAsync(secretName, value);

        /// <summary>
        /// This method asynchronously sets <paramref name="value"/> of <paramref name="type"/> into the instance and filesystem as <paramref name="secretName"/>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="secretName"></param>
        /// <param name="value"></param>
        public static Task SetAsync(Type type, string secretName, dynamic value) =>
            SetAsync(secretName, Core.Convert.ToString(type, value));

        /// <summary>
        /// This method asynchronously sets <paramref name="value"/> of <typeparamref name="TValue"/> into the instance and filesystem as <paramref name="secretName"/>
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="secretName"></param>
        /// <param name="value"></param>
        public static Task SetAsync<TValue>(string secretName, TValue value) =>
            SetAsync(secretName, Core.Convert.ToString<TValue>(value));

        /// <summary>
        /// This method asynchronously populates an object in a Docker Secret using the <code>DockerSecretNameAttribute</code>
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <returns></returns>
        public static Task SetObjectAsync<TType>() where TType : class, new() =>
            // Generate the instance
            Core.Convert.MapWithValueGetterAsync<TType, DockerSecretNameAttribute>(async (attribute, type, value) =>
            {
                // Set the value into the instance
                await SetAsync(type, attribute.Name, value);
                // We're done, just return the current value
                return value;
            }, false);
    }
}
