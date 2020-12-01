using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Fux.Core.Attribute;
using Newtonsoft.Json;

namespace Fux.Core
{
    /// <summary>
    /// This class is responsible for converting POCOs between one another
    /// </summary>
    public static class Convert
    {
        /// <summary>
        /// This delegate provides structure for a callback to retrieve the value of a property with strict typing
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="valueType"></param>
        /// <param name="currentValue"></param>
        /// <typeparam name="TAttribute"></typeparam>
        public delegate dynamic DelegateGetValueCallback<in TAttribute>(TAttribute attribute, Type valueType, dynamic currentValue)
            where TAttribute : FromPropertyAttribute;

        /// <summary>
        /// This delegate provides structure for a callback to retrieve the value of a property asynchronously
        /// with strict typing
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="valueType"></param>
        /// <param name="currentValue"></param>
        /// <typeparam name="TAttribute"></typeparam>
        public delegate Task<dynamic> DelegateGetValueCallbackAsync<in TAttribute>(TAttribute attribute, Type valueType, dynamic currentValue)
            where TAttribute : FromPropertyAttribute;

        /// <summary>
        /// This method provides all of the magic for converting <paramref name="source"/> from
        /// an object of type <typeparamref name="TSource"/> to a new object of type <typeparamref name="TTarget"/>
        /// utilizing the From attribute in <typeparamref name="TSourceAttribute"/> and the To attribute in <typeparamref name="TTargetAttribute"/>
        /// </summary>
        /// <param name="source"></param>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TTarget"></typeparam>
        /// <typeparam name="TSourceAttribute"></typeparam>
        /// <typeparam name="TTargetAttribute"></typeparam>
        /// <returns></returns>
        private static TTarget ConvertObjectToTarget<TSource, TTarget, TSourceAttribute, TTargetAttribute>(TSource source)
            where TSource : class, new() where TTarget : class, new() where TSourceAttribute : FromPropertyAttribute where TTargetAttribute : ToPropertyAttribute
        {
            // Reflect our source type
            Reflection<TSource> sourceReflection = new Reflection<TSource>();
            // Reflect our target type
            Reflection<TTarget> targetReflection = new Reflection<TTarget>();
            // Localize our target instance
            TTarget targetInstance = targetReflection.Instance();
            // Flatten and normalize the source's properties
            Dictionary<string, PropertyInfo> flattenedSource = sourceReflection.FlattenAndNormalize();
            // Flatten and normalize the target's properties
            Dictionary<string, PropertyInfo> flattenedTarget = targetReflection.FlattenAndNormalize();
            // Iterate over the flattened target
            foreach (KeyValuePair<string, PropertyInfo> property in flattenedTarget)
            {
                // Localize the property information
                PropertyInfo propertyInfo = property.Value;
                // Set the value on the property of the target instance
                propertyInfo.SetValue(targetInstance,
                    GetPropertyValueForTarget<TSource, TSourceAttribute, TTargetAttribute>(propertyInfo, source, flattenedSource));
            }
            // We're done, return the populated target instance
            return targetInstance;
        }

        /// <summary>
        /// This method grabs the value for the target property from the source instance
        /// </summary>
        /// <param name="targetProperty"></param>
        /// <param name="source"></param>
        /// <param name="flattenedSource"></param>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TSourceAttribute"></typeparam>
        /// <typeparam name="TTargetAttribute"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        private static TValue GetPropertyValueForTarget<TSource, TSourceAttribute, TTargetAttribute, TValue>(PropertyInfo targetProperty, TSource source, Dictionary<string, PropertyInfo> flattenedSource)
                where TSource : class, new() where TTargetAttribute : ToPropertyAttribute where TSourceAttribute : FromPropertyAttribute
        {
            // Localize the source attribute
            TSourceAttribute sourceAttribute =
                (TSourceAttribute) targetProperty.GetCustomAttributes(typeof(TSourceAttribute)).FirstOrDefault();
            // Check for the normalized target name in the source and return the value
            // We do this because we favor the ToPropertyAttribute
            if (flattenedSource.ContainsKey(sourceAttribute?.Name ?? ""))
                return (TValue)flattenedSource
                    .Where(s => s.Key == sourceAttribute?.Name)
                    .Select(s => s.Value)
                    .FirstOrDefault()
                    ?.GetValue(source);

            // Iterate over the properties in the flattened source
            foreach (KeyValuePair<string, PropertyInfo> property in flattenedSource)
            {
                // Localize the normalized name of the property
                string normalizedSourceName = property.Key;
                // Localize the property information
                PropertyInfo propertyInfo = property.Value;
                // Localize the target attribute
                TTargetAttribute targetAttribute =
                    (TTargetAttribute) propertyInfo.GetCustomAttributes(typeof(TTargetAttribute)).FirstOrDefault();
                // Check for a to attribute and return the value
                if (targetAttribute?.Name ==  normalizedSourceName) return (TValue)propertyInfo.GetValue(source);
                // Check the property name and set the value
                if (propertyInfo.Name.ToLower() == targetProperty.Name.ToLower())
                    return (TValue) propertyInfo.GetValue(source);
            }

            // We're done, we have nothing to return
            return default;
        }

        /// <summary>
        /// This method grabs the value for the target property from the source instance
        /// </summary>
        /// <param name="targetProperty"></param>
        /// <param name="source"></param>
        /// <param name="flattenedSource"></param>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TSourceAttribute"></typeparam>
        /// <typeparam name="TTargetAttribute"></typeparam>
        /// <returns></returns>
        private static dynamic GetPropertyValueForTarget<TSource, TSourceAttribute, TTargetAttribute>(PropertyInfo targetProperty, TSource source, Dictionary<string, PropertyInfo> flattenedSource)
                where TSource : class, new() where TTargetAttribute : ToPropertyAttribute where TSourceAttribute : FromPropertyAttribute
        {
            // Localize the source attribute
            TSourceAttribute sourceAttribute =
                (TSourceAttribute) targetProperty.GetCustomAttributes(typeof(TSourceAttribute)).FirstOrDefault();
            // Check for the normalized target name in the source and return the value
            // We do this because we favor the ToPropertyAttribute
            if (flattenedSource.ContainsKey(sourceAttribute?.Name ?? ""))
                return flattenedSource
                    .Where(s => s.Key == sourceAttribute?.Name)
                    .Select(s => s.Value)
                    .FirstOrDefault()
                    ?.GetValue(source);

            // Iterate over the properties in the flattened source
            foreach (KeyValuePair<string, PropertyInfo> property in flattenedSource)
            {
                // Localize the normalized name of the property
                string normalizedSourceName = property.Key;
                // Localize the property information
                PropertyInfo propertyInfo = property.Value;
                // Localize the target attribute
                TTargetAttribute targetAttribute =
                    (TTargetAttribute) propertyInfo.GetCustomAttributes(typeof(TTargetAttribute)).FirstOrDefault();
                // Check for a to attribute and return the value
                if (targetAttribute?.Name == normalizedSourceName) return propertyInfo.GetValue(source);
                // Check the property name and set the value
                if (propertyInfo.Name.ToLower() == targetProperty.Name.ToLower()) return propertyInfo.GetValue(source);
            }

            // We're done, we have nothing to return
            return null;
        }

        /// <summary>
        /// This method flattens and normalizes an object and executes a callback
        /// on each property to get its value
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="setNewValueAfterCallback"></param>
        /// <typeparam name="TTarget"></typeparam>
        /// <typeparam name="TAttribute"></typeparam>
        /// <returns></returns>
        private static TTarget GenerateObjectWithCallback<TTarget, TAttribute>(DelegateGetValueCallback<TAttribute> callback, bool setNewValueAfterCallback = true)
            where TTarget : class, new() where TAttribute : FromPropertyAttribute
        {
            // Reflect our target type
            Reflection<TTarget> targetReflection = new Reflection<TTarget>();
            // Localize the instance
            TTarget instance = targetReflection.Instance();
            // Flatten the object
            Dictionary<string, PropertyInfo> flattenedTarget = targetReflection.FlattenAndNormalize();
            // Iterate over the properties
            foreach (KeyValuePair<string, PropertyInfo> property in flattenedTarget)
            {
                // Localize the property information
                PropertyInfo propertyInfo = property.Value;
                // Check the property's attributes for our namespace
                foreach (System.Attribute attribute in propertyInfo.GetCustomAttributes())
                {
                    // Check the attribute's type
                    if (attribute.GetType() != typeof(TAttribute)) continue;
                    // Localize the attribute
                    TAttribute typedAttribute = (TAttribute)attribute;
                    // Localize the current value
                    dynamic currentValue = propertyInfo.GetValue(instance);
                    // Localize the new value
                    dynamic newValue = callback.Invoke(typedAttribute, propertyInfo.PropertyType, currentValue);
                    // Check the value set flag and reset the value into the property
                    if (setNewValueAfterCallback)
                        propertyInfo.SetValue(instance, newValue);
                }
            }

            // We're done, return the new instance
            return instance;
        }

        /// <summary>
        /// This method asynchronously flattens and normalizes an object and executes a callback
        /// on each property to get its value
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="setNewValueAfterCallback"></param>
        /// <typeparam name="TTarget"></typeparam>
        /// <typeparam name="TAttribute"></typeparam>
        /// <returns></returns>
        private static async Task<TTarget> GenerateObjectWithCallbackAsync<TTarget, TAttribute>(DelegateGetValueCallbackAsync<TAttribute> callback, bool setNewValueAfterCallback = true)
            where TTarget : class, new() where TAttribute : FromPropertyAttribute
        {
            // Reflect our target type
            Reflection<TTarget> targetReflection = new Reflection<TTarget>();
            // Localize the instance
            TTarget instance = targetReflection.Instance();
            // Flatten the object
            Dictionary<string, PropertyInfo> flattenedTarget = targetReflection.FlattenAndNormalize();
            // Iterate over the properties
            foreach (KeyValuePair<string, PropertyInfo> property in flattenedTarget)
            {
                // Localize the normalized property name
                string normalizedName = property.Key;
                // Localize the property information
                PropertyInfo propertyInfo = property.Value;
                // Check the property's attributes for our namespace
                foreach (System.Attribute attribute in propertyInfo.GetCustomAttributes())
                {
                    // Check the attribute's type
                    if (attribute.GetType() == typeof(TAttribute))
                    {
                        // Localize the attribute
                        TAttribute typedAttribute = (TAttribute)attribute;
                        // Localize the current value
                        dynamic currentValue = propertyInfo.GetValue(instance);
                        // Localize the new value
                        dynamic newValue = await callback.Invoke(typedAttribute, propertyInfo.PropertyType, currentValue);
                        // Check the value set flag and reset the value into the property
                        if (setNewValueAfterCallback)
                            propertyInfo.SetValue(instance, newValue);
                    }
                }
            }

            // We're done, return the new instance
            return instance;
        }

        /// <summary>
        /// This method converts a string to a type either through a
        /// built-in <code>ToString()</code> or JSON serialization
        /// </summary>
        /// <param name="targetType"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static dynamic FromString(Type targetType, string source)
        {
            // Localize our nullable type
            Type nullableType = Nullable.GetUnderlyingType(targetType);
            // Check for a nullable type and reset the source type
            if (nullableType != null)
                targetType = nullableType;
            // Check the nullable type and the value of the source
            if (nullableType != null && source == null)
                return Activator.CreateInstance(typeof(Nullable<>).MakeGenericType(nullableType));
            // Check the value of the source
            if (string.IsNullOrEmpty(source) || string.IsNullOrWhiteSpace(source)) return null;
            // Check for a system type and return the system converted value
            if (Fux.Core.Reflection.IsSystemType(targetType))
                return System.Convert.ChangeType(source, targetType);
            // Return the deserialized value of the object
            return JsonConvert.DeserializeObject(source, targetType);
        }

        /// <summary>
        /// This method converts a string to a type either through a
        /// built-in <code>ToString()</code> or JSON serialization
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static TTarget FromString<TTarget>(string source)
        {
            // Localize the target type
            Type targetType = typeof(TTarget);
            // Localize our nullable type
            Type nullableType = Nullable.GetUnderlyingType(targetType);
            // Check for a nullable type and reset the source type
            if (nullableType != null)
                targetType = nullableType;
            // Check the nullable type and the value of the source
            if (nullableType != null && source == null)
                return (TTarget)Activator.CreateInstance(typeof(Nullable<>).MakeGenericType(nullableType));
            // Check the value of the source
            if (string.IsNullOrEmpty(source) || string.IsNullOrWhiteSpace(source)) return default;
            // Check for a system type and return the system converted value
            if (Fux.Core.Reflection.IsSystemType(targetType))
                return (TTarget)System.Convert.ChangeType(source, targetType);
            // Return the deserialized value of the object
            return JsonConvert.DeserializeObject<TTarget>(source);
        }

        /// <summary>
        /// This method converts <paramref name="source"/> of type <typeparamref name="TSource"/>
        /// to a new object of type <typeparamref name="TTarget"/>
        /// </summary>
        /// <param name="source"></param>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TTarget"></typeparam>
        /// <typeparam name="TSourceAttribute"></typeparam>
        /// <typeparam name="TTargetAttribute"></typeparam>
        /// <returns></returns>
        public static TTarget Map<TSource, TTarget, TSourceAttribute, TTargetAttribute>(TSource source)
            where TSource : class, new() where TTarget : class, new() where TSourceAttribute : FromPropertyAttribute where TTargetAttribute : ToPropertyAttribute =>
                ConvertObjectToTarget<TSource, TTarget, TSourceAttribute, TTargetAttribute>(source);

        /// <summary>
        /// This method maps values to a new object instance <typeparamref name="TTarget"/> from an external
        /// source using <paramref name="callback"/> with a strict return type
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="setNewValueAfterCallback"></param>
        /// <typeparam name="TTarget"></typeparam>
        /// <typeparam name="TAttribute"></typeparam>
        /// <returns></returns>
        public static TTarget
            MapWithValueGetter<TTarget, TAttribute>(DelegateGetValueCallback<TAttribute> callback, bool setNewValueAfterCallback = true)
                where TTarget : class, new() where TAttribute : FromPropertyAttribute =>
                    GenerateObjectWithCallback<TTarget, TAttribute>(callback, setNewValueAfterCallback);

        /// <summary>
        /// This method asynchronously maps values to a new object instance <typeparamref name="TTarget"/> from an external
        /// source using <paramref name="callback"/> with a strict return type
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="setNewValueAfterCallback"></param>
        /// <typeparam name="TTarget"></typeparam>
        /// <typeparam name="TAttribute"></typeparam>
        /// <returns></returns>
        public static Task<TTarget>
            MapWithValueGetterAsync<TTarget, TAttribute>(
                DelegateGetValueCallbackAsync<TAttribute> callback, bool setNewValueAfterCallback = true)
                    where TTarget : class, new() where TAttribute : FromPropertyAttribute =>
                        GenerateObjectWithCallbackAsync<TTarget, TAttribute>(callback, setNewValueAfterCallback);

        /// <summary>
        /// This method converts a type to a string either through a
        /// built-in <code>ToString()</code> or JSON serialization
        /// </summary>
        /// <param name="sourceType"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ToString(Type sourceType, dynamic source)
        {
            // Localize our target type
            Type targetType = typeof(string);
            // Check for a system type and return the system converted value
            if (Fux.Core.Reflection.IsSystemType(sourceType))
                return System.Convert.ChangeType(source, targetType);
            // Return the serialized value of the object
            return JsonConvert.SerializeObject(source, Fux.Core.Global.JsonSerializerSettings);
        }

        /// <summary>
        /// This method converts a type to a string either through a
        /// built-in <code>ToString()</code> or JSON serialization
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ToString<TSource>(TSource source)
        {
            // Localize our source type
            Type sourceType = typeof(TSource);
            // Localize our target type
            Type targetType = typeof(string);
            // Check for a system type and return the system converted value
            if (Fux.Core.Reflection.IsSystemType(sourceType))
                return (string)System.Convert.ChangeType(source, targetType);
            // Return the serialized value of the object
            return JsonConvert.SerializeObject(source, Fux.Core.Global.JsonSerializerSettings);
        }
    }

    /// <summary>
    /// This class provides a generic for convertible objects
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    public static class Convert<TSource> where TSource : class, new()
    {
        /// <summary>
        /// This method maps <typeparamref name="TSource"/> <paramref name="source"/> to a
        /// new instance of <typeparamref name="TTarget"/>
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        /// <typeparam name="TTarget"></typeparam>
        /// <typeparam name="TSourceAttribute"></typeparam>
        /// <typeparam name="TTargetAttribute"></typeparam>
        public static TTarget Map<TTarget, TSourceAttribute, TTargetAttribute>(TSource source)
                where TTarget : class, new() where TSourceAttribute : FromPropertyAttribute where TTargetAttribute : ToPropertyAttribute =>
            Convert<TSource, TTarget>.Map<TSourceAttribute, TTargetAttribute>(source);
    }

    /// <summary>
    /// This class provides a generic for convertible objects
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TTarget"></typeparam>
    public static class Convert<TSource, TTarget> where TSource : class, new() where TTarget : class, new()
    {
        /// <summary>
        /// This method maps <typeparamref name="TSource"/> <paramref name="source"/> to a
        /// new instance of <typeparamref name="TTarget"/>
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static TTarget Map<TSourceAttribute, TTargetAttribute>(TSource source)
            where TSourceAttribute : FromPropertyAttribute where TTargetAttribute : ToPropertyAttribute =>
            Convert.Map<TSource, TTarget, TSourceAttribute, TTargetAttribute>(source);
    }
}
