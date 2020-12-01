using System;
using Newtonsoft.Json;

namespace Fux.Core
{
    /// <summary>
    /// This class maintains the global settings for Fux
    /// </summary>
    public static class Global
    {
        /// <summary>
        /// This property contains Fux's default DateTime format string for JSON serialization
        /// </summary>
        public const string JsonDateFormatDefault = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFK";

        /// <summary>
        /// This proeprty contains our global
        /// </summary>
        public static JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            // Set the date format into the serializer settings
            DateFormatString = new Func<string>(() => {
                // Localize the environment value
                string jsonDateFormat = System.Environment.GetEnvironmentVariable("FUX_JSON_DATE_FORMAT")?.ToString();
                // Check the value and return no formatting
                if (string.IsNullOrEmpty(jsonDateFormat) || string.IsNullOrWhiteSpace(jsonDateFormat))
                    jsonDateFormat = JsonDateFormatDefault;
                // We'r done, return the date format string from the environment
                return jsonDateFormat;
            }).Invoke(),
            // Set the pretty-printing into the serializer settings
            Formatting = new Func<Formatting>(() => {
                // Localize the environment value
                string jsonPrettyPrint = System.Environment.GetEnvironmentVariable("FUX_JSON_PRETTY_PRINT")?.ToString();
                // Check the value and return no formatting
                if (string.IsNullOrEmpty(jsonPrettyPrint) || string.IsNullOrWhiteSpace(jsonPrettyPrint))
                    return Formatting.None;
                // Parse the value as a boolean
                bool castSuccess = Boolean.TryParse(jsonPrettyPrint, out bool prettyPrint);
                // Check the success flag and pretty print flag and return no formatting
                if (!castSuccess || !prettyPrint)
                    return Formatting.None;
                // We're done, indent the formatting
                return Formatting.Indented;
            }).Invoke(),
            // Set the null-value handling into the serializer settings
            NullValueHandling = new Func<NullValueHandling>(() => {
                // Localize the environment value
                string jsonIgnoreNullValues =
                    System.Environment.GetEnvironmentVariable("FUX_JSON_IGNORE_NULL_VALUES")?.ToString();
                // Check the value and return no formatting
                if (string.IsNullOrEmpty(jsonIgnoreNullValues) || string.IsNullOrWhiteSpace(jsonIgnoreNullValues))
                    return NullValueHandling.Include;
                // Parse the value as a boolean
                bool castSuccess = Boolean.TryParse(jsonIgnoreNullValues, out bool ignoreNullValues);
                // Check the success flag and pretty print flag and return no formatting
                if (!castSuccess || !ignoreNullValues)
                    return NullValueHandling.Include;
                // We're done, indent the formatting
                return NullValueHandling.Ignore;
            }).Invoke(),
            // Set the reference loop handling into the serializer settings
            ReferenceLoopHandling = new Func<ReferenceLoopHandling>(() => {
                // Localize the environment value
                string jsonReferenceLoopHandler =
                    System.Environment.GetEnvironmentVariable("FUX_JSON_REFERENCE_LOOP_HANDLING")?.ToString();
                // Check the value and return no reference loop handler
                if (string.IsNullOrEmpty(jsonReferenceLoopHandler) || string.IsNullOrWhiteSpace(jsonReferenceLoopHandler))
                    return ReferenceLoopHandling.Serialize;
                // Parse the value as a boolean
                bool castSuccess =
                    Enum.TryParse<ReferenceLoopHandling>(jsonReferenceLoopHandler, true, out ReferenceLoopHandling referenceLoopHandler);
                // Check the success flag and pretty print flag and return no formatting
                if (!castSuccess) return ReferenceLoopHandling.Serialize;
                // We're done, indent the formatting
                return referenceLoopHandler;
            }).Invoke()
        };
    }
}
