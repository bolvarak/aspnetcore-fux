using System;
using System.Collections.Generic;
using System.Linq;

namespace Fux.Core
{
    /// <summary>
    /// This class provides a generically typed fluid interface to the Singleton class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class Singleton<T> where T : class, new()
    {
        /// <summary>
        /// This method instantiates an object as a new singleton [or returns an existing one]
        /// </summary>
        /// <returns></returns>
        public static T Instance() => Singleton.Instance<T>();

        /// <summary>
        /// This method instantiates an object as a new singleton
        /// This replaces any existing singleton(s) of the same type
        /// </summary>
        /// <param name="reset"></param>
        /// <returns></returns>
        public static T Instance(bool reset) => Singleton.Instance<T>(reset);

        /// <summary>
        /// This method instantiates an object as a new singleton from a provided instance
        /// This replaces any existing singleton(s) of the same type
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static T Instance(T instance) => Singleton.Instance<T>(instance);

        /// <summary>
        /// This method instantiates an object as a new singleton that requires constructor arguments
        /// This replaces any existing singleton(s) of the same type
        /// </summary>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public static T Instance(IEnumerable<dynamic> arguments) => Singleton.Instance<T>(arguments);

        /// <summary>
        /// This method instantiates an object as a new singleton the requires constructor arguments
        /// This replaces any existing singleton(s) of the same type
        /// </summary>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public static T Instance(params dynamic[] arguments) => Singleton.Instance<T>(arguments);

        /// <summary>
        /// This method instantiates an object as a new singleton [or returns an existing one] and returns its reflection
        /// </summary>
        /// <returns></returns>
        public static Reflection<T> Reflect() => Singleton.Reflect<T>();

        /// <summary>
        /// This method instantiates an object as a new singleton and returns its reflection
        /// This replaces any existing singleton(s) of the same type
        /// </summary>
        /// <param name="reset"></param>
        /// <returns></returns>
        public static Reflection<T> Reflect(bool reset) => Singleton.Reflect<T>(reset);

        /// <summary>
        /// This method instantiates an object as a new singleton from a provided instance and returns its reflection
        /// This replaces any existing singleton(s) of the same type
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static Reflection<T> Reflect(T instance) => Singleton.Reflect<T>(instance);

        /// <summary>
        /// This method instantiates an object as a new singleton that requires constructor arguments and returns its reflection
        /// This replaces any existing singleton(s) of the same type
        /// </summary>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public static Reflection<T> Reflect(IEnumerable<dynamic> arguments) => Singleton.Reflect<T>(arguments);

        /// <summary>
        /// This method instantiates an object as a new singleton that requires constructor arguments and returns its reflection
        /// This replaces any existing singleton(s) of the same type
        /// </summary>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public static Reflection<T> Reflect(params dynamic[] arguments) => Singleton.Reflect<T>(arguments);
    }

    /// <summary>
    /// This class maintains an interface for fluidly generating Singletons
    /// </summary>
    public static class Singleton
    {
        /// <summary>
        /// This property contains the list of registered singleton instances
        /// </summary>
        private static readonly Dictionary<Type, Reflection<dynamic>> _instances =
            new Dictionary<Type, Reflection<dynamic>>();

        /// <summary>
        /// This method returns a typed singleton from the instance
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private static T instance<T>() where T : class, new()
        {
            // Localize our generic type
            Type genericType = typeof(Reflection<>).GenericTypeArguments[0].MakeGenericType(typeof(T));
            // Localize the instance
            Reflection<T> instance =
                ((_instances.FirstOrDefault(i => i.Key.Equals(genericType)).Value as Reflection<T>) ?? null);
            // Check the instance and return its value
            if (instance != null) return instance.Instance();
            // We're done, nothing to return
            return default;
        }

        /// <summary>
        /// This method returns the reflection construct for a singleton
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static Reflection<dynamic> reflection(Type type) =>
            _instances.FirstOrDefault(i => i.Key.Equals(type.GUID)).Value;

        /// <summary>
        /// This method returns the reflection construct for a singleton
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private static Reflection<T> reflection<T>() where T : class, new() =>
            (_instances.FirstOrDefault(i => i.Key.Equals(typeof(T).GUID)).Value as Reflection<T>);

        /// <summary>
        /// This method fluidly adds a singleton to the instance and returns its instantiation
        /// </summary>
        /// <param name="reset"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Instance<T>(bool reset = false) where T : class, new()
        {
            // Check for an existing singleton and instantiate the singleton
            if (instance<T>() == null || reset)
                _instances[typeof(T)] = (new Reflection<T>() as Reflection<dynamic>);
            // We're done, return the singleton
            return instance<T>();
        }

        /// <summary>
        /// This method fluidly resets or adds a singleton to the instance with constructor arguments
        /// </summary>
        /// <param name="arguments"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Instance<T>(IEnumerable<dynamic> arguments) where T : class, new()
        {
            // Reset the singleton instance
            _instances[typeof(T)] = (new Reflection<T>(arguments) as Reflection<dynamic>);
            // We're done, return the singleton
            return instance<T>();
        }

        /// <summary>
        /// This method fluidly resets or adds a singleton to the instance with constructor arguments
        /// </summary>
        /// <param name="arguments"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Instance<T>(params dynamic[] arguments) =>
            Instance<T>(arguments);

        /// <summary>
        /// This method fluidly resets or adds a singleton to the instance with constructor arguments
        /// from its system type and returns its reflection
        /// </summary>
        /// <param name="type"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public static Reflection<dynamic> Reflect(Type type, params dynamic[] arguments) =>
            Reflect(type, arguments);

        /// <summary>
        /// This method fluidly adds a singleton to the instance and returns its reflection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Reflection<T> Reflect<T>() where T : class, new()
        {
            // Instantiate the object
            Instance<T>();
            // We're done, return the reflection
            return reflection<T>();
        }

        /// <summary>
        /// This method fluidly resets or adds a singleton to the instance and returns its reflection
        /// </summary>
        /// <param name="reset"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Reflection<T> Reflect<T>(bool reset) where T : class, new()
        {
            // Instantiate the object
            Instance<T>(reset);
            // We're done, return the reflection
            return reflection<T>();
        }

        /// <summary>
        /// This method fluidly resets or adds a singleton to the instance with constructor arguments and returns its reflection
        /// </summary>
        /// <param name="arguments"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Reflection<T> Reflect<T>(IEnumerable<dynamic> arguments) where T : class, new()
        {
            // Instantiate the object
            Instance<T>(arguments);
            // We're done, return the reflection
            return reflection<T>();
        }

        /// <summary>
        /// This method fluidly resets or adds a singleton to the instance with constructor arguments and returns its reflection
        /// </summary>
        /// <param name="arguments"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Reflection<T> Reflect<T>(params dynamic[] arguments) where T : class, new() =>
            Reflect<T>(arguments);
    }
}
