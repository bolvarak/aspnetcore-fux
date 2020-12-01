using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Fux.Example.Helper
{
    /// <summary>
    /// This class maintains the structure for a message from a runner
    /// </summary>
    public class Message
    {
        /// <summary>
        /// This property contains the content of the message
        /// </summary>
        [JsonProperty("content")]
        public string Content { get; private set; } = null;

        /// <summary>
        /// This property contains the data object of the message
        /// </summary>
        [JsonProperty("data")] public readonly Dictionary<string, dynamic> Data =
            new Dictionary<string, dynamic>();

        /// <summary>
        /// This property contains the debug flag
        /// </summary>
        [JsonProperty("debug")]
        public bool Debug { get; private set; } = false;

        /// <summary>
        /// This property denotes that the object came from an exception
        /// </summary>
        [JsonProperty("exception")]
        public bool Exception { get; private set; } = false;

        /// <summary>
        /// This property contains the debug data when <code>Debug</code> is <code>true</code>
        /// </summary>
        [JsonProperty("debugData")]
        public List<string> DebugData =>
            !Debug
                ? new List<string>()
                : Data
                    .Select(p => $"{p.Key} - {Core.Convert.ToString(p.Value)}")
                    .ToList();

        /// <summary>
        /// This property contains the unique ID of the message
        /// </summary>
        [JsonProperty("id")] public readonly Guid Id = Guid.NewGuid();

        /// <summary>
        /// This property contains the timestamp in which the message created
        /// </summary>
        public readonly DateTime Logged = DateTime.UtcNow;

        /// <summary>
        /// This property contains the priority flag which means it was immediately sent to outputs
        /// </summary>
        [JsonProperty("priority")]
        public bool Priority { get; private set; } = false;

        /// <summary>
        /// This property contains the timestamp in which the message was sent to output
        /// </summary>
        [JsonProperty("sent")]
        public DateTime? Sent { get; set; } = null;

        /// <summary>
        /// This method provides a static factory for a new message
        /// </summary>
        /// <returns></returns>
        public static Message New() =>
            new Message();

        /// <summary>
        /// This method provides a static factory for a new message with content
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static Message New(string content) =>
            new Message(content);

        /// <summary>
        /// This method provides a static factory for a new message
        /// built from a system exception
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static Message New(System.Exception exception) =>
            new Message(exception);

        /// <summary>
        /// This method provides a static factory for a new
        /// message with only a data list
        /// </summary>
        /// <param name="listData"></param>
        /// <returns></returns>
        public static Message New(IEnumerable<dynamic> listData) =>
            new Message(listData);

        /// <summary>
        /// This method provides a static factory for a new
        /// message with content and prioritization
        /// </summary>
        /// <param name="content"></param>
        /// <param name="prioritize"></param>
        /// <returns></returns>
        public static Message New(string content, bool prioritize = false) =>
            new Message(content, prioritize);

        /// <summary>
        /// This method provides a static factory for a new
        /// message with content, data and prioritization
        /// </summary>
        /// <param name="content"></param>
        /// <param name="data"></param>
        /// <param name="prioritize"></param>
        /// <returns></returns>
        public static Message New(string content, IDictionary<string, dynamic> data, bool prioritize = false) =>
            new Message(content, data, prioritize);

        /// <summary>
        /// This method instantiates the message with a logger
        /// </summary>
        public Message()
        {
        }

        /// <summary>
        /// This method instantiates a new message with content
        /// </summary>
        /// <param name="content"></param>
        public Message(string content) =>
            WithContent(content);

        /// <summary>
        /// This method instantiates a new message from a system exception
        /// </summary>
        /// <param name="exception"></param>
        public Message(System.Exception exception)
        {
            // Check for an inner exception and add it to the data objects
            if (exception.InnerException != null)
                WithDataObject($"InnerException[{exception.InnerException.GetType().Name}]",
                    Message.New(exception));
            // Set the source into the data objects
            WithDataObject("Source", exception.Source ?? "Unavailable");
            // Set the target site into the data objects
            WithDataObject("TargetSite", exception.TargetSite?.Name ?? "Unavailable");
            // Split and iterate over the stack trace
            exception.StackTrace?.Split("\n", StringSplitOptions.TrimEntries)
                .ToList().ForEach(s => WithDataObject(s, "stack-trace-list-item"));
            // Reset the exception flag into the instance
            Exception = true;
        }

        /// <summary>
        /// This method instantiates a new message with only a data list
        /// </summary>
        /// <param name="listData"></param>
        public Message(IEnumerable<dynamic> listData) =>
            listData.ToList().ForEach(i => WithDataListObject(i));

        /// <summary>
        /// This method instantiates a new message with content and prioritization
        /// </summary>
        /// <param name="content"></param>
        /// <param name="prioritize"></param>
        public Message(string content, bool prioritize = false) =>
            WithContent(content)
                .WithPriorityFlag(prioritize);

        /// <summary>
        /// This method instantiates a new Message with content, data and prioritization
        /// </summary>
        /// <param name="content"></param>
        /// <param name="data"></param>
        /// <param name="prioritize"></param>
        public Message(string content, IDictionary<string, dynamic> data = null, bool prioritize = false) =>
            WithContent(content)
                .WithData(data)
                .WithPriorityFlag(prioritize);

        /// <summary>
        /// This method serializes a named variable into a string representation
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <param name="padding"></param>
        private string MarshalDataObject(string key, dynamic data, int padding) =>
            ("\n" + new string('\t', padding) + $"[{key}] => " + MarshalDataObject(data, padding));

        /// <summary>
        /// This method loads a dynamic data object from a string interpolation reference
        /// </summary>
        /// <param name="reference"></param>
        /// <returns></returns>
        private dynamic DataObjectFromReference(string reference) =>
            DataObject(DataObjectReferenceToKey(reference));

        /// <summary>
        /// This method loads a typed data object from a string interpolation reference
        /// </summary>
        /// <typeparam name="TDataObject"></typeparam>
        /// <param name="reference"></param>
        /// <returns></returns>
        private TDataObject DataObjectFromReference<TDataObject>(string reference) =>
            DataObject<TDataObject>(DataObjectReferenceToKey(reference));

        /// <summary>
        /// This method extracts a data object key name from a string interpolation
        /// </summary>
        /// <param name="reference"></param>
        /// <returns></returns>
        private string DataObjectReferenceToKey(string reference) =>
            reference.Replace("${", "").Replace("}", "").Trim();

        /// <summary>
        /// This method serializes a variable list item into a string representation
        /// </summary>
        /// <param name="dataObject"></param>
        /// <param name="padding"></param>
        /// <returns></returns>
        private string MarshalDataObject(dynamic dataObject, int padding)
        {
            // Localize the type of the data
            Type type = (dataObject as object).GetType();
            // Define our line start
            string linePrefix = ("\n" + new string('\t', padding));
            // Check for a nested Runner Message
            if (type.Equals(this.GetType()))
                return ((Message) dataObject)
                    .ToString(padding + 1)
                    .Replace("\n", linePrefix);
            // Check for a system exception
            if (type.Equals(typeof(System.Exception)))
                return Message.New((System.Exception) dataObject)
                    .ToString(padding + 1)
                    .Replace("\n", linePrefix);
            // Check for a time span
            if (type.Equals(typeof(TimeSpan)))
                return $"{((TimeSpan) dataObject):g}";
            // If we get here, convert it using the core serializer
            return Core.Convert
                .ToString(dataObject).Replace("\n", linePrefix);
        }

        /// <summary>
        /// This method marshals the data objects into string representations
        /// </summary>
        /// <param name="source"></param>
        /// <param name="padding"></param>
        private void MarshalDataObjects(ref string source, int padding)
        {
            // Localize the formatting of the JSON engine
            Newtonsoft.Json.Formatting originalFormatting =
                Core.Global.JsonSerializerSettings.Formatting;
            // Reset the formatting of the JSON engine
            Core.Global.JsonSerializerSettings.Formatting =
                Newtonsoft.Json.Formatting.None;
            // Iterate over the items in the data list
            foreach (KeyValuePair<string, dynamic> data in Data)
            {
                // Check for a list item and skip if Debug isn't enabled
                if (!data.Key.ToLower().StartsWith("list-item-") && !Debug)
                    continue;
                // Check the key for a list item and reset the object string
                if (data.Key.ToLower().StartsWith("list-item-"))
                    source += MarshalDataObject(data.Value, padding + 1);
                // Check for a stack trace item
                if (data.Key.ToLower().EndsWith("-list-item") && Debug)
                    source += MarshalDataObject(data.Value, padding + 1);
                // Make sure debug is on and marshal the object as normal
                else if (Debug)
                    source += MarshalDataObject(data.Key, data.Value, padding);
            }

            // Reset the formatting of the JSON engine
            Core.Global.JsonSerializerSettings.Formatting = originalFormatting;
        }

        /// <summary>
        /// This method interpolates the data objects into the message
        /// </summary>
        /// <param name="source"></param>
        private void InterpolateDataObjects(ref string source)
        {
            // Localize the formatting of the JSON engine
            Newtonsoft.Json.Formatting originalFormatting =
                Core.Global.JsonSerializerSettings.Formatting;
            // Reset the formatting of the JSON engine
            Core.Global.JsonSerializerSettings.Formatting =
                Newtonsoft.Json.Formatting.None;
            // Define our RegEx patter
            Regex regularExpression = new Regex(@"(\$\{.*?\})", RegexOptions.Compiled);
            // Iterate over the matches
            foreach (Match match in regularExpression.Matches(Content))
            {
                // Count the captures
                if (!match.Captures.Any()) continue;
                // Localize the interpolation reference
                string reference = match.Value;
                // Localize the value
                dynamic value = DataObjectFromReference(reference);
                // Localize the stringified value
                string stringValue = Core.Convert.ToString(Type.GetTypeFromHandle(Type.GetTypeHandle(value)), value);
                // Make the replacement in the source string
                source = source.Replace(reference, stringValue);
            }

            // Replace any dates and times
            source = source.Replace("${DateTime.Now}", DateTime.Now.ToString(),
                StringComparison.CurrentCultureIgnoreCase);
            // Replace any UTC dates and times
            source = source.Replace("${DateTime.UtcNow}", DateTime.UtcNow.ToString(),
                StringComparison.CurrentCultureIgnoreCase);
            // Reset the formatting of the JSON engine
            Core.Global.JsonSerializerSettings.Formatting = originalFormatting;
        }

        /// <summary>
        /// This method interpolates the data objects into the message
        /// </summary>
        /// <param name="source"></param>
        /// <typeparam name="TValue"></typeparam>
        private void InterpolateDataObjects<TValue>(ref string source)
        {
            // Localize the formatting of the JSON engine
            Newtonsoft.Json.Formatting originalFormatting =
                Core.Global.JsonSerializerSettings.Formatting;
            // Reset the formatting of the JSON engine
            Core.Global.JsonSerializerSettings.Formatting =
                Newtonsoft.Json.Formatting.Indented;
            // Define our RegEx patter
            Regex regularExpression = new Regex(@"(\$\{.*?\})", RegexOptions.Compiled);
            // Iterate over the matches
            foreach (Match match in regularExpression.Matches(Content))
            {
                // Count the captures
                if (!match.Captures.Any()) continue;
                // Localize the interpolation reference
                string reference = match.Value;
                // Localize the value
                dynamic value = DataObjectFromReference(reference);
                // Make the replacement in the source string
                source = source.Replace(reference, Core.Convert.ToString<TValue>(value));
            }

            // Replace any dates and times
            source = source.Replace("${DateTime.Now}", DateTime.Now.ToString(),
                StringComparison.CurrentCultureIgnoreCase);
            // Replace any UTC dates and times
            source = source.Replace("${DateTime.UtcNow}", DateTime.UtcNow.ToString(),
                StringComparison.CurrentCultureIgnoreCase);
            // Reset the formatting of the JSON engine
            Core.Global.JsonSerializerSettings.Formatting = originalFormatting;
        }

        /// <summary>
        /// This method retrieves a data object from the instance by its key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public dynamic DataObject(string key) =>
            Data.FirstOrDefault(d => d.Key.ToLower().Equals(key.ToLower().Trim())).Value ?? null;

        /// <summary>
        /// This method retrieves a data object from the instance by its key
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public TType DataObject<TType>(string key) =>
            (TType) Data.FirstOrDefault(d => d.Key.ToLower().Equals(key.ToLower().Trim())).Value ?? default;

        /// <summary>
        /// This method retrieves a data object from the instance by its type
        /// NOTE:  You should only use this if you expect a unique and specific type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public dynamic DataObject(Type type) =>
            Data.FirstOrDefault(d => d.Value.GetType().Equals(type)).Value ?? null;

        /// <summary>
        /// This method retrieves a data object from the instance by its type
        /// NOTE:  You should only use this if you expect a unique and specific type
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <returns></returns>
        public TType DataObject<TType>() =>
            Data.FirstOrDefault(d => d.Value.GetType().Equals(typeof(TType))).Value ?? default;

        /// <summary>
        /// This method generates a string representation of a Runner Message
        /// </summary>
        /// <param name="padding"></param>
        /// <returns></returns>
        public string ToString(int padding = 1)
        {
            // Define our response string
            string response = Content;
            // Interpolate the data objects into the string
            InterpolateDataObjects(ref response);
            // Marshal the data objects
            MarshalDataObjects(ref response, padding + 1);
            // Define our level
            string level = (Priority ? "PRIO" : "info");
            // Define our date
            string date = Logged.ToLocalTime().ToString("dddd MMMM d, yyyy");
            // Define our time
            string time = Logged.ToLocalTime().ToString("HH:mm:ss");
            // Define our microseconds
            string microseconds = Logged.ToLocalTime().ToString("ffffff");
            // We're done, send the response
            return $"[{level}] [{date} @ {time} ({microseconds})]\n\t{response}";
        }

        /// <summary>
        /// This method resets the content into the instance
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public Message WithContent(string content)
        {
            // Set the content into the instance
            Content = content;
            // We're done, return the instance
            return this;
        }

        /// <summary>
        /// This method sets many data objects into the instance
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public Message WithData(IDictionary<string, dynamic> data)
        {
            // Set the data objects into the instance
            data.Keys.ToList().ForEach(k => WithDataObject(k, data[k]));
            // We're done, return the instance
            return this;
        }

        /// <summary>
        /// This method sets a list-item data object into the instance
        /// </summary>
        /// <param name="value"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public Message WithDataListObject(dynamic value, string prefix = "list-item") =>
            WithDataObject($"{prefix}-{((System.Object) value).GetType().Name}-{Guid.NewGuid()}", value);

        /// <summary>
        /// This method sets a list-item data object into the instance
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <param name="value"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public Message WithDataListObject<TType>(TType value, string prefix = "list-item") =>
            WithDataObject($"{prefix}-{typeof(TType).Name}-{Guid.NewGuid()}", value);

        /// <summary>
        /// This method sets a data object into the instance
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Message WithDataObject(string name, dynamic value)
        {
            // Set the data object into the instance
            Data[name.Trim()] = value;
            // We're done, return the instance
            return this;
        }

        /// <summary>
        /// This method sets a data object into the instance
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Message WithDataObject<TType>(string name, TType value)
        {
            // Set the data object into the instance
            Data[name.Trim()] = value;
            // We're done, return the instance
            return this;
        }

        /// <summary>
        /// This method resets the debug flag into the instance
        /// </summary>
        /// <param name="flag"></param>
        /// <returns></returns>
        public Message WithDebugFlag(bool flag)
        {
            // Reset the debug flag into the instance
            Debug = flag;
            // We're done, return the instance
            return this;
        }

        /// <summary>
        /// This method forces the debug flag to <code>true</code>
        /// </summary>
        /// <returns></returns>
        public Message WithDebugging() =>
            WithDebugFlag(true);

        /// <summary>
        /// This property forces the debug flag <code>false</code>
        /// </summary>
        /// <returns></returns>
        public Message WithoutDebugging() =>
            WithDebugFlag(false);

        /// <summary>
        /// This method forces the priority flag to <code>false</code>
        /// </summary>
        /// <returns></returns>
        public Message WithoutPriority() =>
            WithPriorityFlag(false);

        /// <summary>
        /// This method forces the priority flag to <code>true</code>
        /// </summary>
        /// <returns></returns>
        public Message WithPriority() =>
            WithPriorityFlag(true);

        /// <summary>
        /// This method resets the priority flag into the instance
        /// </summary>
        /// <param name="flag"></param>
        /// <returns></returns>
        public Message WithPriorityFlag(bool flag)
        {
            // Reset the priority flag into the instance
            Priority = flag;
            // We're done, return the instance
            return this;
        }

        /// <summary>
        /// This method resets the output timestamp into the instance
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <returns></returns>
        public Message WithSentTimeStamp(DateTime? timeStamp)
        {
            // Check the timestamp and reset it into the instance
            if (timeStamp.HasValue) Sent = timeStamp.Value.ToUniversalTime();
            // We're done, return the instance
            return this;
        }
    }
}
