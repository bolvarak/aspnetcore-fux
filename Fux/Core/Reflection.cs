using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Newtonsoft.Json;

namespace Fux.Core
{
    /// <summary>
    /// This class provides an interface for fluidly instantiating objects
    /// </summary>
    public static class Reflection
    {
        /// <summary>
        /// This method flattens an object and normalizes the property names to outer<paramref name="separator"/>inner
        /// NOTE:  This uses case insensitivity, keep that in mind when using this against POCOs
        /// </summary>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static Dictionary<string, PropertyInfo> FlattenAndNormalize<TType>(char separator = '.')
            where TType : class, new() =>
                Instantiate<TType>().FlattenAndNormalize(separator);

        /// <summary>
        /// This method flattens and normalizes the property names to outer<paramref name="separator"/>inner of nested objects
        /// NOTE:  This uses case insensitivity, keep that in mind when using this against POCOs
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <param name="separator"></param>
        /// <param name="path"></param>
        /// <param name="flattenedProperties"></param>
        /// <typeparam name="TNestedType"></typeparam>
        /// <returns></returns>
        public static Dictionary<string, PropertyInfo> FlattenAndNormalize<TType, TNestedType>(char separator, string path,
            ref Dictionary<string, PropertyInfo> flattenedProperties)
                where TType : class, new() where TNestedType: class, new() =>
                    Instantiate<TType>()
                        .FlattenAndNormalize<TNestedType>(separator, path, ref flattenedProperties);

        /// <summary>
        /// This method fluidly instantiates a typed object
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public static TResult Instance<TResult>() where TResult : class, new() =>
            new Reflection<TResult>().Instance();

        /// <summary>
        /// This method fluidly instantiates a typed object with constructor arguments
        /// </summary>
        /// <param name="arguments"></param>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public static TResult Instance<TResult>(IEnumerable<object> arguments)
            where TResult : class, new() =>
                new Reflection<TResult>(arguments).Instance();

        /// <summary>
        /// This method instantiates a reflection
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public static Reflection<TResult> Instantiate<TResult>() where TResult : class, new() =>
            new Reflection<TResult>();

        /// <summary>
        /// This method instantiates a reflection with contructor arguments
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public static Reflection<TResult> Instantiate<TResult>(IEnumerable<dynamic> arguments)
            where TResult : class, new() =>
                new Reflection<TResult>(arguments);

        /// <summary>
        /// This method instantiates a reflection with constructor arguments
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public static Reflection<TResult> Instantiate<TResult>(params object[] arguments)
            where TResult : class, new() =>
                Instantiate<TResult>(arguments);

        /// <summary>
        /// This method calls a method on the instantiated object
        /// </summary>
        /// <param name="methodName"></param>
        /// <returns></returns>
        /// <typeparam name="TType"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        public static TResult Invoke<TType, TResult>(string methodName)
            where TType : class, new() =>
                Instantiate<TType>()
                    .Invoke<TResult>(methodName);

        /// <summary>
        /// This method calls a method with arguments on the instantiated object
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="arguments"></param>
        /// <typeparam name="TType"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public static TResult Invoke<TType, TResult>(string methodName, IEnumerable<object> arguments)
            where TType : class, new() =>
                Instantiate<TType>()
                    .Invoke<TResult>(methodName, arguments);

        /// <summary>
        /// This method calls a method with arguments on the instantiated object
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public static TResult Invoke<TType, TResult>(string methodName, params object[] arguments)
            where TType : class, new() =>
                Instantiate<TType>()
                    .Invoke<TResult>(methodName, arguments);

        /// <summary>
        /// This method calls a generic method on the instantiated object
        /// </summary>
        /// <param name="methodName"></param>
        /// <typeparam name="TType"></typeparam>
        /// <typeparam name="TGenericType"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public static TResult InvokeGeneric<TType, TGenericType, TResult>(string methodName)
            where TType : class, new() =>
                Instantiate<TType>()
                    .InvokeGeneric<TResult, TGenericType>(methodName);

        /// <summary>
        /// This method calls a generic method with arguments on the object
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="arguments"></param>
        /// <typeparam name="TType"></typeparam>
        /// <typeparam name="TGenericType"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public static TResult InvokeGeneric<TType, TGenericType, TResult>(string methodName, IEnumerable<object> arguments)
            where TType : class, new() =>
                Instantiate<TType>()
                    .InvokeGeneric<TResult, TGenericType>(methodName, arguments);

        /// <summary>
        /// This method calls a generic method with arguments on the instantiated object
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="arguments"></param>
        /// <typeparam name="TType"></typeparam>
        /// <typeparam name="TGenericType"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public static TResult InvokeGeneric<TType, TGenericType, TResult>(string methodName, params object[] arguments) =>
            InvokeGeneric<TType, TGenericType, TResult>(methodName, arguments);

        /// <summary>
        /// This method determines whether a type is built-in or not
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsSystemType(Type type) =>
            (type.Assembly.Equals(typeof(object).Assembly) && typeof(IConvertible).IsAssignableFrom(type));

        /// <summary>
        /// This method determines whether a type is built-in or not
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <returns></returns>
        public static bool IsSystemType<TType>() =>
            (typeof(TType).Assembly.Equals(typeof(object).Assembly) && typeof(IConvertible).IsAssignableFrom(typeof(TType)));
    }

    /// <summary>
    /// This class provides object reflection
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Reflection<T> where T: class, new()
    {
        /// <summary>
        /// This property contains the list of arguments to pass to the constructor
        /// </summary>
        private readonly List<dynamic> _arguments = new List<dynamic>();

        /// <summary>
        /// This property contains the instance of our reflection
        /// </summary>
        private T _instance;

        /// <summary>
        /// This property contains the PropertyInfo objects for the properties in <typeparamref name="T"/>
        /// </summary>
        private Dictionary<string, PropertyInfo> _properties = null;

        /// <summary>
        /// This property contains our property selector lambda expressions
        /// </summary>
        private readonly Dictionary<Expression<Func<T, dynamic>>, PropertyInfo> _propertySelectors =
            new Dictionary<Expression<Func<T, dynamic>>, PropertyInfo>();

        /// <summary>
        /// This property contains the MethodInfo objects for the properties in <typeparamref name="T"/>
        /// </summary>
        private Dictionary<string, MethodInfo> _methods = null;

        /// <summary>
        /// This property contains the type in which we will be reflecting
        /// </summary>
        private readonly Type _type = typeof(T);

        /// <summary>
        /// This property contains the reflected type's full name
        /// </summary>
        [JsonProperty("fullName")]
        public string FullName => GetTypeFullName();

        /// <summary>
        /// This method reflects and instantiates the object
        /// </summary>
        public Reflection() { }

        /// <summary>
        /// This method reflects and instantiates the object with constructor arguments
        /// </summary>
        /// <param name="arguments"></param>
        public Reflection(IEnumerable<dynamic> arguments) =>
            WithArguments(arguments);

        /// <summary>
        /// This method reflects and instantiates the object with constructor arguments
        /// </summary>
        /// <param name="arguments"></param>
        public Reflection(params dynamic[] arguments) =>
            WithArguments(arguments);

        /// <summary>
        /// This method builds the property map on reflection
        /// </summary>
        private Dictionary<string, PropertyInfo> buildPropertyMap()
        {
            // Check the property map
            if (_properties == null)
            {
                // Instantiate the property map
                _properties = new Dictionary<string, PropertyInfo>();
                // Iterate over the properties and add them to the instance
                foreach (PropertyInfo property in typeof(T).GetProperties())
                    _properties[property.Name] = property;
            }
            // We're done, return the property map
            return _properties;
        }

        /// <summary>
        /// THis method builds the method map on reflection
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, MethodInfo> buildMethodMap()
        {
            // Check the method map
            if (_methods == null)
            {
                // Intantiate the method map
                _methods = new Dictionary<string, MethodInfo>();
                // Iterate over the methods and add them to the instance
                foreach (MethodInfo method in typeof(T).GetMethods())
                    _methods[method.Name] = method;
            }
            // We're one, return the method map
            return _methods;
        }

        /// <summary>
        /// This method localizes the property information from a lambda selector expressions
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        protected PropertyInfo ExpressionToPropertyInfo(Expression<Func<T, dynamic>> selector)
        {
            // Check to see if the instance has processed this reflection before
            if (!_propertySelectors.Any(e => e.Key.Equals(selector)))
            {
                // Localize the member expression
                MemberExpression memberExpression = (selector.Body as MemberExpression);
                // Make sure we have a member expression
                if (memberExpression != null)
                {
                    // Localize the property information
                    PropertyInfo property = (memberExpression.Member as PropertyInfo);
                    // Make sure we have property information and set the value into the instance
                    if (property != null) _propertySelectors[selector] = property;
                }
            }

            // We're done, return the property info object
            return _propertySelectors
                .Where(e => e.Key.Equals(selector))
                .Select(e => e.Value)
                .FirstOrDefault();
        }

        /// <summary>
        /// This method invokes both normal and generic methods on objects using reflection
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="methodName"></param>
        /// <param name="arguments"></param>
        /// <param name="genericType"></param>
        /// <returns></returns>
        protected TValue InvokeMethod<TValue>(T instance, string methodName, IEnumerable<dynamic> arguments = null,
            Type genericType = null)
        {
            // Localize our method information
            MethodInfo methodInfo = Methods()
                .Where(m => m.Key.Equals(methodName))
                .Select(m => m.Value)
                .FirstOrDefault();
            // Make sure we have a method
            if (methodInfo == null) return default;
            // Check for a generic and reset the information
            if (methodInfo.IsGenericMethod && !genericType.Equals(null))
                methodInfo = methodInfo.MakeGenericMethod(genericType);
            // Check for arguments and return the invocation
            if (arguments != null && arguments.Any()) return (TValue)methodInfo.Invoke(instance, arguments.ToArray());
            // We're done, return the invocation
            return (TValue)methodInfo.Invoke(instance, null);
        }

        /// <summary>
        /// This method sets the value of property <paramref name="propertyName"/>
        /// on <paramref name="instance"/> to <paramref name="value"/> of type <typeparamref name="TValue"/>
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <typeparam name="TValue"></typeparam>
        protected void SetPropertyValue<TValue>(T instance, string propertyName, TValue value) =>
            PropertyInfo(propertyName)?.SetValue(instance, value);

        /// <summary>
        /// This method sets the value of the property found with <paramref name="expression"/>
        /// on <paramref name="instance"/> to <paramref name="value"/> of type <typeparamref name="TValue"/>
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="expression"></param>
        /// <param name="value"></param>
        /// <typeparam name="TValue"></typeparam>
        protected void SetPropertyValue<TValue>(T instance, Expression<Func<T, TValue>> expression, TValue value) =>
            PropertyInfo(expression)?.SetValue(instance, value);

        /// <summary>
        /// This method flattens an object and normalizes the property names to outer<paramref name="separator"/>inner
        /// NOTE:  This uses case insensitivity, keep that in mind when using this against POCOs
        /// </summary>
        /// <param name="separator"></param>
        /// <returns></returns>
        public Dictionary<string, PropertyInfo> FlattenAndNormalize(char separator = '.') =>
            FlattenAndNormalize<T>(separator);

        /// <summary>
        /// This method flattens an object and normalizes the property names to outer<paramref name="separator"/>inner
        /// NOTE:  This uses case insensitivity, keep that in mind when using this against POCOs
        /// </summary>
        /// <param name="separator"></param>
        /// <returns></returns>
        public Dictionary<string, PropertyInfo> FlattenAndNormalize<TSource>(char separator = '.') where TSource : class, new()
        {
            // Define our properties
            Dictionary<string, PropertyInfo> flattenedProperties = new Dictionary<string, PropertyInfo>();
            // Iterate over the properties
            foreach (KeyValuePair<string, PropertyInfo> propertyInfo in Properties())
            {
                // Define the property path
                string path = propertyInfo.Value.Name.ToLower();
                // Check the property for a class and flatten it
                if (propertyInfo.Value.GetType().IsClass)
                    FlattenAndNormalize(propertyInfo.Value.PropertyType, separator, path, ref flattenedProperties);
                // Add the property to the response
                flattenedProperties[path] = propertyInfo.Value;
            }
            // We're done, return the flattened object
            return flattenedProperties;
        }

        /// <summary>
        /// This method flattens a nested object and normalizes the property names
        /// to outer<paramref name="separator"/>inner
        /// NOTE:  This uses case insensitivity, keep that in mind when using this against POCOs
        /// </summary>
        /// <param name="separator"></param>
        /// <param name="path"></param>
        /// <param name="flattenedProperties"></param>
        /// <typeparam name="TNestedType"></typeparam>
        /// <returns></returns>
        public Dictionary<string, PropertyInfo> FlattenAndNormalize<TNestedType>(char separator, string path,
            ref Dictionary<string, PropertyInfo> flattenedProperties)
                where TNestedType: class, new()
        {
            // Iterate over the properties
            foreach (PropertyInfo property in typeof(TNestedType).GetProperties())
            {
                // Reset the property path
                path = $"{path}{separator.ToString()}{property.Name.ToLower()}";
                // Check the property for a class and flatten it
                if (property.GetType().IsClass)
                    FlattenAndNormalize(property.PropertyType, separator, path, ref flattenedProperties);
                // Add the property to the response
                flattenedProperties[path] = property;
            }

            // We're done, return the flattened object
            return flattenedProperties;
        }

        /// <summary>
        /// This method flattens a nested object and normalizes the property names
        /// to outer<paramref name="separator"/>inner
        /// NOTE:  This uses case insensitivity, keep that in mind when using this against POCOs
        /// </summary>
        /// <param name="nestedType"></param>
        /// <param name="separator"></param>
        /// <param name="path"></param>
        /// <param name="flattenedProperties"></param>
        /// <returns></returns>
        public Dictionary<string, PropertyInfo> FlattenAndNormalize(Type nestedType, char separator, string path,
            ref Dictionary<string, PropertyInfo> flattenedProperties)
        {
            // Iterate over the properties
            foreach (PropertyInfo property in nestedType.GetProperties())
            {
                // Reset the property path
                path = $"{path}{separator.ToString()}{property.Name.ToLower()}";
                // Check the property for a class and flatten it
                if (property.GetType().IsClass)
                    FlattenAndNormalize(property.PropertyType, separator, path, ref flattenedProperties);
                // Add the property to the response
                flattenedProperties[path] = property;
            }

            // We're done, return the flattened object
            return flattenedProperties;
        }

        /// <summary>
        /// This method returns the property value from the property's name and provided instance
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public TValue Get<TValue>(T instance, string propertyName) =>
            (TValue)PropertyInfo(propertyName)?.GetValue(instance);

        /// <summary>
        /// This method returns the property value from a lambda expression selector and provided instance
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="selector"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public TValue Get<TValue>(T instance, Expression<Func<T, TValue>> selector) =>
            (TValue)PropertyInfo(selector)?.GetValue(instance);

        /// <summary>
        /// This method returns the property value from the property's name and existing instance
        /// </summary>
        /// <param name="propertyName"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public TValue Get<TValue>(string propertyName) =>
            Get<TValue>(_instance, propertyName);

        /// <summary>
        /// This method returns the property value from a lambda expression selector and existing instance
        /// </summary>
        /// <param name="selector"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public TValue Get<TValue>(Expression<Func<T, TValue>> selector) =>
            (TValue)Get(_instance, selector);

        /// <summary>
        /// This method returns the property information for a flattened and normalized object
        /// or maps it immediately, stores it and returns it
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <returns></returns>
        public Dictionary<string, PropertyInfo> GetFlattenedType<TType>()
            where TType: class, new() =>
                new Reflection<TType>()
                    .FlattenAndNormalize();

        /// <summary>
        /// This method returns the full name of the reflected type
        /// </summary>
        /// <returns></returns>
        public string GetTypeFullName() =>
            _type.FullName;

        /// <summary>
        /// This method instantiates an instance of our reflection
        /// </summary>
        /// <param name="reset"></param>
        /// <returns></returns>
        public T Instance(bool reset = false)
        {
            // Check for an instance and instantiate the object
            if (_instance == null || reset)
            {
                // Check to see if we need  to send arguments to the constructor
                if (_arguments.Any())
                    _instance = (T)Activator.CreateInstance(typeof(T), _arguments);
                else
                    _instance = (T)Activator.CreateInstance(typeof(T));
                // Build the property map
                buildPropertyMap();
                // Build the method map
                buildMethodMap();
            }
            // We're done, return the instance of the object
            return _instance;
        }

            /// <summary>
            /// This method invokes the method <paramref name="methodName"/>
            /// and returning <typeparamref name="TValue"/> on <paramref name="instance"/>
            /// </summary>
            /// <param name="instance"></param>
            /// <param name="methodName"></param>
            /// <typeparam name="TValue"></typeparam>
            /// <returns></returns>
            public TValue Invoke<TValue>(T instance, string methodName) =>
            InvokeMethod<TValue>(instance, methodName);

        /// <summary>
        /// This method invokes the method <paramref name="methodName"/> with <paramref name="arguments"/>
        /// and returning <typeparamref name="TValue"/> on <paramref name="instance"/>
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="methodName"></param>
        /// <param name="arguments"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public TValue Invoke<TValue>(T instance, string methodName, IEnumerable<dynamic> arguments) =>
            InvokeMethod<TValue>(instance, methodName, arguments);

        /// <summary>
        /// This method invokes the method <paramref name="methodName"/> with <paramref name="arguments"/>
        /// and returning <typeparamref name="TValue"/> on <paramref name="instance"/>
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="methodName"></param>
        /// <param name="arguments"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public TValue Invoke<TValue>(T instance, string methodName, params dynamic[] arguments) =>
            InvokeMethod<TValue>(instance, methodName, arguments);

        /// <summary>
        /// This method invokes the method <paramref name="methodName"/> and returning <typeparamref name="TValue"/> on the existing instance
        /// </summary>
        /// <param name="methodName"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public TValue Invoke<TValue>(string methodName) =>
            InvokeMethod<TValue>(_instance, methodName);

        /// <summary>
        /// This method invokes the method <paramref name="methodName"/> with <paramref name="arguments"/>
        /// and returning <typeparamref name="TValue"/> on the existing instance
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="arguments"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public TValue Invoke<TValue>(string methodName, IEnumerable<object> arguments) =>
            InvokeMethod<TValue>(_instance, methodName, arguments);

        /// <summary>
        /// This method invokes the method <paramref name="methodName"/> with <paramref name="arguments"/>
        /// and returning <typeparamref name="TValue"/> on the existing instance
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="arguments"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public TValue Invoke<TValue>(string methodName, params object[] arguments) =>
            InvokeMethod<TValue>(_instance, methodName, arguments);

        /// <summary>
        /// This method invokes the generic method <paramref name="methodName"/> with a
        /// generic of <paramref name="genericType"/> and returning <typeparamref name="TValue"/> on <paramref name="instance"/>
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="genericType"></param>
        /// <param name="methodName"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public TValue InvokeGeneric<TValue>(T instance, Type genericType, string methodName) =>
            InvokeMethod<TValue>(instance, methodName, null, genericType);

        /// <summary>
        /// This method invokes the generic method <paramref name="methodName"/> with a
        /// generic of <paramref name="genericType"/> and <paramref name="arguments"/>
        /// and returning <typeparamref name="TValue"/> on <paramref name="instance"/>
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="genericType"></param>
        /// <param name="methodName"></param>
        /// <param name="arguments"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public TValue InvokeGeneric<TValue>(T instance, Type genericType, string methodName,
            IEnumerable<object> arguments) =>
            InvokeMethod<TValue>(instance, methodName, arguments, genericType);

        /// <summary>
        /// This method invokes the generic method <paramref name="methodName"/> with a
        /// generic of <paramref name="genericType"/> and <paramref name="arguments"/>
        /// and returning <typeparamref name="TValue"/> on <paramref name="instance"/>
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="genericType"></param>
        /// <param name="methodName"></param>
        /// <param name="arguments"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public TValue InvokeGeneric<TValue>(T instance, Type genericType, string methodName,
            params object[] arguments) =>
            InvokeGeneric<TValue>(instance, genericType, methodName, arguments);

        /// <summary>
        /// This method invokes the generic method <paramref name="methodName"/> with a
        /// generic of <typeparamref name="TType"/> and returning the value on <paramref name="instance"/>
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="methodName"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <typeparam name="TType"></typeparam>
        /// <returns></returns>
        public TValue InvokeGeneric<TValue, TType>(T instance, string methodName) =>
             InvokeMethod<TValue>(instance, methodName, null, typeof(TType));

        /// <summary>
        /// This method invokes the generic method <paramref name="methodName"/> with a
        /// generic of <typeparamref name="TType"/> and <paramref name="arguments"/>
        /// and returning <typeparamref name="TValue"/> on <paramref name="instance"/>
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="methodName"></param>
        /// <param name="arguments"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <typeparam name="TType"></typeparam>
        /// <returns></returns>
        public TValue InvokeGeneric<TValue, TType>(T instance, string methodName, IEnumerable<dynamic> arguments) =>
            InvokeMethod<TValue>(instance, methodName, arguments, typeof(TType));

        /// <summary>
        /// This method invokes the generic method <paramref name="methodName"/> with a
        /// generic of <typeparamref name="TType"/> and <paramref name="arguments"/>
        /// and returning <typeparamref name="TValue"/> on <paramref name="instance"/>
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="methodName"></param>
        /// <param name="arguments"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <typeparam name="TType"></typeparam>
        /// <returns></returns>
        public TValue InvokeGeneric<TValue, TType>(T instance, string methodName, params dynamic[] arguments) =>
            InvokeMethod<TValue>(instance, methodName, arguments, typeof(TType));

        /// <summary>
        /// This method invokes the generic method <paramref name="methodName"/> with a
        /// generic of <paramref name="genericType"/> and <paramref name="arguments"/>
        /// and returning <typeparamref name="TValue"/> on the existing instance
        /// </summary>
        /// <param name="genericType"></param>
        /// <param name="methodName"></param>
        /// <param name="arguments"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public TValue InvokeGeneric<TValue>(Type genericType, string methodName, IEnumerable<dynamic> arguments) =>
            InvokeMethod<TValue>(_instance, methodName, arguments, genericType);

        /// <summary>
        /// This method invokes the generic method <paramref name="methodName"/> with a
        /// generic of <paramref name="genericType"/> and <paramref name="arguments"/>
        /// and returning <typeparamref name="TValue"/> on the existing instance
        /// </summary>
        /// <param name="genericType"></param>
        /// <param name="methodName"></param>
        /// <param name="arguments"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public TValue InvokeGeneric<TValue>(Type genericType, string methodName, params dynamic[] arguments) =>
            InvokeMethod<TValue>(_instance, methodName, arguments, genericType);

        /// <summary>
        /// This method invokes the generic method <paramref name="methodName"/> with a
        /// generic of <typeparamref name="TType"/> returning <typeparamref name="TValue"/>
        /// on the existing instance
        /// </summary>
        /// <param name="methodName"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <typeparam name="TType"></typeparam>
        /// <returns></returns>
        public TValue InvokeGeneric<TValue, TType>(string methodName) =>
            InvokeMethod<TValue>(_instance, methodName, null, typeof(TType));

        /// <summary>
        /// This method invokes the generic method <paramref name="methodName"/> with a
        /// generic of <typeparamref name="TType"/> and <paramref name="arguments"/>
        /// and returning <typeparamref name="TValue"/> on the existing instance
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="arguments"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <typeparam name="TType"></typeparam>
        /// <returns></returns>
        public TValue InvokeGeneric<TValue, TType>(string methodName, IEnumerable<object> arguments) =>
            InvokeMethod<TValue>(_instance, methodName, arguments, typeof(TType));

        /// <summary>
        /// This method invokes the generic method <paramref name="methodName"/> with a
        /// generic of <typeparamref name="TType"/> and <paramref name="arguments"/>
        /// and returning <typeparamref name="TValue"/> on the existing instance
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="arguments"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <typeparam name="TType"></typeparam>
        /// <returns></returns>
        public TValue InvokeGeneric<TValue, TType>(string methodName, params object[] arguments) =>
            InvokeMethod<TValue>(_instance, methodName, arguments, typeof(TType));

        /// <summary>
        /// This method returns the methods from the reflection
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, MethodInfo> Methods() =>
            buildMethodMap();

        /// <summary>
        /// This method returns the properties from the reflection
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, PropertyInfo> Properties() =>
            buildPropertyMap();

        /// <summary>
        /// This method returns the property information for a property
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public PropertyInfo PropertyInfo(string propertyName) =>
            Properties().FirstOrDefault(p => p.Key.Equals(propertyName)).Value;

        /// <summary>
        /// This method returns the property information structure for a property from a lambda expression selector
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        public PropertyInfo PropertyInfo(Expression<Func<T, dynamic>> selector) =>
            ExpressionToPropertyInfo(selector);

        /// <summary>
        /// This method returns the property information structure for a property from a lambda expression selector
        /// </summary>
        /// <param name="selector"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public PropertyInfo PropertyInfo<TValue>(Expression<Func<T, TValue>> selector) =>
            PropertyInfo(selector);

        /// <summary>
        /// This method returns the property name from a lambda selector expression
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        public string PropertyName(Expression<Func<T, dynamic>> selector) =>
            PropertyInfo(selector)?.Name;

        /// <summary>
        /// This method sets the value of property <paramref name="propertyName"/>
        /// to <paramref name="value"/> of type <typeparamref name="TValue"/> on <paramref name="instance"/>
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public Reflection<T> Set<TValue>(T instance, string propertyName, TValue value)
        {
            // Set the property value into the instance
            SetPropertyValue<TValue>(instance, propertyName, value);
            // We're done, return the instance
            return this;
        }

        /// <summary>
        /// This method sets the value of the property found with <paramref name="selector"/>
        /// to <paramref name="value"/> of type <typeparamref name="TValue"/> in <paramref name="instance"/>
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="selector"></param>
        /// <param name="value"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public Reflection<T> Set<TValue>(T instance, Expression<Func<T, TValue>> selector, TValue value)
        {
            // Set the property value into the instance
            SetPropertyValue<TValue>(instance, selector, value);
            // We're done, return the instance
            return this;
        }

        /// <summary>
        /// This method sets the value of property <paramref name="propertyName"/>
        /// to <paramref name="value"/> of type <typeparamref name="TValue"/> on the existing instance
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public Reflection<T> Set<TValue>(string propertyName, TValue value) =>
            Set<TValue>(_instance, propertyName, value);

        /// <summary>
        /// This method sets the value of the property found with <paramref name="selector"/>
        /// to <paramref name="value"/> of type <typeparamref name="TValue"/> on the existing instance
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="value"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public Reflection<T> Set<TValue>(Expression<Func<T, TValue>> selector, TValue value) =>
            Set<TValue>(_instance, selector, value);

        /// <summary>
        /// This method returns the generic type of the reflection
        /// </summary>
        /// <returns></returns>
        public Type Type() => _type;

        /// <summary>
        /// This method adds a constructor argument to the instance
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        public Reflection<T> WithArgument(dynamic argument)
        {
            // Add the argument to the instance
            _arguments.Add(argument);
            // We're done, return the instance
            return this;
        }

        /// <summary>
        /// This method resets all of the arguments into the instance
        /// </summary>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public Reflection<T> WithArguments(IEnumerable<dynamic> arguments)
        {
            // Clear the arguments in the instance
            _arguments.Clear();
            // Reset the arguments into the instance
            _arguments.AddRange(arguments);
            // We're done, return the instance
            return this;
        }

        /// <summary>
        /// This method resets all of the arguments into the instance
        /// </summary>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public Reflection<T> WithArguments(params dynamic[] arguments) => WithArguments(arguments.ToList());
    }
}
