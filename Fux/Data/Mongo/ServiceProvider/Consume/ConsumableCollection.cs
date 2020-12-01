using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Newtonsoft.Json;
using Fux.Core.Extension.Enumerable;
using Fux.Data.Mongo.Abstract;

namespace Fux.Data.Mongo.ServiceProvider.Consume
{
    /// <summary>
    /// This class maintains the structure for a collection of consumable MongoDB documents from an external source
    /// </summary>
    public class ConsumableCollection<TDocumentObject, TDataTranslationObject, TDocumentKeyType> where TDocumentObject : CollectionModel, new() where TDataTranslationObject : DataTranslationObject<TDocumentObject>, new()
    {
        /// <summary>
        /// This delegate maintains the structure for a batch synchronization callback
        /// </summary>
        /// <param name="iteration"></param>
        /// <param name="total"></param>
        /// <param name="batch"></param>
        /// <param name="batchSize"></param>
        /// <param name="timeTook"></param>
        public delegate void DelegateSynchronizeBatchCallback(int iteration, int total,
            ConsumableCollection<TDocumentObject, TDataTranslationObject, TDocumentKeyType> batch, int batchSize,
            TimeSpan timeTook);

        /// <summary>
        /// This delegate maintains the structure for an asynchronous batch synchronization callback
        /// </summary>
        /// <param name="iteration"></param>
        /// <param name="total"></param>
        /// <param name="batch"></param>
        /// <param name="batchSize"></param>
        /// <param name="timeTook"></param>
        public delegate Task DelegateSynchronizeBatchCallbackAsync(int iteration, int total,
            ConsumableCollection<TDocumentObject, TDataTranslationObject, TDocumentKeyType> batch, int batchSize,
            TimeSpan timeTook);

        /// <summary>
        /// This property tells the system whether or not bulk operations are ordered
        /// </summary>
        private bool _isOrdered = false;

        /// <summary>
        /// This property tells the instance whether or not to skip document validation on bulk operations
        /// </summary>
        private bool _skipDocumentValidation = false;

        /// <summary>
        /// This property contains the instance of the suppressed logger
        /// </summary>
        private ILogger _suppressedLogger = null;

        /// <summary>
        /// This property tells the instance whether or not to upsert replacements
        /// </summary>
        private bool _upsert = true;

        /// <summary>
        /// This property contains our logger
        /// </summary>
        protected ILogger Logger;

        /// <summary>
        /// This property contains the number of Documents to send to MongoDB in bulk
        /// </summary>
        [JsonProperty("batchSize")] public int BatchSize = 100;

        /// <summary>
        /// This property contains our list of consumables
        /// </summary>
        [JsonProperty("collection")]
        public readonly List<ConsumableCollectionItem<TDocumentObject, TDataTranslationObject, TDocumentKeyType>>
            Collection =
                new List<ConsumableCollectionItem<TDocumentObject, TDataTranslationObject, TDocumentKeyType>>();

        /// <summary>
        /// This method contains document match expression to use when synchronizing the Mongo ID properties
        /// </summary>
        [JsonIgnore]
        public Expression<Func<TDocumentObject, TDocumentKeyType>> DocumentMatchExpression { get; private set; } =
            d => (TDocumentKeyType)Convert.ChangeType(d.MongoId, typeof(TDocumentKeyType));

        /// <summary>
        /// This property contains the match-value to mongo-id map
        /// </summary>
        [JsonProperty("idMap")]
        public readonly Dictionary<TDocumentKeyType, string> IdMap = new Dictionary<TDocumentKeyType, string>();

        /// <summary>
        /// This property contains the match value for all documents
        /// </summary>
        [JsonProperty("matchValue")]
        public TDocumentKeyType MatchValue { get; private set; }

        /// <summary>
        /// This property contains the DTO match expression to use when synchronizing the Mongo ID properties
        /// </summary>
        [JsonIgnore]
        public Expression<Func<TDataTranslationObject, TDocumentKeyType>> MatchValueExpression { get; private set; }

        /// <summary>
        /// This property contains our batch callback
        /// </summary>
        protected DelegateSynchronizeBatchCallback BatchCallback = null;

        /// <summary>
        /// This property contains our asynchronous batch callback
        /// </summary>
        protected DelegateSynchronizeBatchCallbackAsync BatchCallbackAsync = null;

        /// <summary>
        /// This property contains the match all values flag
        /// </summary>
        protected bool MatchAllValues = false;

        /// <summary>
        /// This property contains the mongo ID population flag
        /// </summary>
        protected bool MongoIdsPopulated = false;

        /// <summary>
        /// This method instantiates an empty class
        /// </summary>
        public ConsumableCollection()
        {
        }

        /// <summary>
        /// This method instantiates the class from an existing collection
        /// </summary>
        /// <param name="collection"></param>
        public ConsumableCollection(
            IEnumerable<ConsumableCollectionItem<TDocumentObject, TDataTranslationObject, TDocumentKeyType>> collection)
        {
            // Set the collection into the instance
            Collection = collection.ToList();
        }

        /// <summary>
        /// This method converts a document expression to a value for matching
        /// </summary>
        /// <param name="document"></param>
        /// <param name="valueExpression"></param>
        /// <returns></returns>
        public static string GetBsonElementNameFromDocument(TDocumentObject document,
            Expression<Func<TDocumentObject, TDocumentKeyType>> valueExpression)
        {
            // Localize the member expression
            MemberExpression memberExpression = (MemberExpression)valueExpression.Body;
            // Make sure we have a member expression
            if (memberExpression != null)
            {
                // Localize the property information
                PropertyInfo property = (memberExpression.Member as PropertyInfo);
                // Make sure we have property information and set the value
                if (property != null)
                {
                    // Localize our BSON element
                    BsonElementAttribute bsonElement =
                        (BsonElementAttribute)property.GetCustomAttributes(typeof(BsonElementAttribute), true)
                            .FirstOrDefault();
                    // Check to see if we have a BSON element
                    if (bsonElement != null) return bsonElement.ElementName;
                }
            }

            // We're done, return the default value
            return null;
        }

        /// <summary>
        /// This method converts a document expression to a value for matching
        /// </summary>
        /// <param name="document"></param>
        /// <param name="valueExpression"></param>
        /// <returns></returns>
        public static TDocumentKeyType GetMatchValueFromDocument(TDocumentObject document,
            Expression<Func<TDocumentObject, TDocumentKeyType>> valueExpression)
        {
            // Localize the member expression
            MemberExpression memberExpression = (MemberExpression)valueExpression.Body;
            // Make sure we have a member expression
            if (memberExpression != null)
            {
                // Localize the property information
                PropertyInfo property = (memberExpression.Member as PropertyInfo);
                // Make sure we have property information and set the value
                if (property != null) return (TDocumentKeyType)property.GetValue(document);
            }

            // We're done, return the default value
            return default;
        }

        /// <summary>
        /// This method converts a DTO expression to a value for matching
        /// </summary>
        /// <param name="document"></param>
        /// <param name="valueExpression"></param>
        /// <returns></returns>
        public static TDocumentKeyType GetMatchValueFromModel(TDataTranslationObject document,
            Expression<Func<TDataTranslationObject, TDocumentKeyType>> valueExpression)
        {
            // Localize the member expression
            MemberExpression memberExpression = (MemberExpression)valueExpression.Body;
            // Make sure we have a member expression
            if (memberExpression != null)
            {
                // Localize the property information
                PropertyInfo property = (memberExpression.Member as PropertyInfo);
                // Make sure we have property information and set the value
                if (property != null) return (TDocumentKeyType)property.GetValue(document);
            }

            // We're done, return the default value
            return default;
        }

        /// <summary>
        /// This method builds the message
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private string buildMessage(string message) =>
            $"[{DateTime.Now:d}, {DateTime.Now:t}]:\t{message}";

        /// <summary>
        /// This method logs a debug exception
        /// </summary>
        /// <param name="error"></param>
        /// <param name="arguments"></param>
        protected void LogDebug(Exception error, params object[] arguments) =>
            Logger?.LogDebug(error, buildMessage(error.Message), arguments);

        /// <summary>
        /// This method logs a debug exception with a custom message
        /// </summary>
        /// <param name="error"></param>
        /// <param name="message"></param>
        /// <param name="arguments"></param>
        protected void LogDebug(Exception error, string message, params object[] arguments) =>
            Logger?.LogDebug(error, buildMessage(message), arguments);

        /// <summary>
        /// This method logs a custom debug message
        /// </summary>
        /// <param name="message"></param>
        /// <param name="arguments"></param>
        protected void LogDebug(string message, params object[] arguments) =>
            Logger?.LogDebug(buildMessage(message), arguments);

        /// <summary>
        /// This method logs a debug exception from an event
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="error"></param>
        /// <param name="arguments"></param>
        protected void LogDebug(EventId eventId, Exception error, params object[] arguments) =>
            Logger?.LogDebug(eventId, error, buildMessage(error.Message), arguments);

        /// <summary>
        /// This method logs a debug exception with a custom message from an event
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="error"></param>
        /// <param name="message"></param>
        /// <param name="arguments"></param>
        protected void LogDebug(EventId eventId, Exception error, string message, params object[] arguments) =>
            Logger?.LogDebug(eventId, error, buildMessage(message), arguments);

        /// <summary>
        /// This method logs a custom debug message from an event
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="message"></param>
        /// <param name="arguments"></param>
        protected void LogDebug(EventId eventId, string message, params object[] arguments) =>
            Logger?.LogDebug(eventId, message, arguments);

        /// <summary>
        /// This method logs an error message to the console
        /// </summary>
        /// <param name="error"></param>
        /// <param name="message"></param>
        public void LogError(Exception error, string message = null) =>
            Logger?.LogError(buildMessage(message ?? error.Message), error);

        /// <summary>
        /// This method logs an information message to the console
        /// </summary>
        /// <param name="message"></param>
        /// <param name="arguments"></param>
        public void LogInformation(string message, params object[] arguments) =>
            Logger?.LogInformation(buildMessage(message), arguments);

        /// <summary>
        /// This method populates the MongoDB IDs in the consumables collection
        /// </summary>
        /// <returns></returns>
        protected ConsumableCollection<TDocumentObject, TDataTranslationObject, TDocumentKeyType> PopulateMongoIds()
        {
            // Check for ID map values and skip all of this
            if (!IdMap.Any()) return this;
            // Localize the total number of consumables
            int totalConsumables = Collection.Count;
            // Iterate over the consumables
            for (int i = 0; i < totalConsumables; ++i)
            {
                // Localize the match value key
                TDocumentKeyType matchValueKey =
                    IdMap.Keys.Where(k => k.Equals(Collection[i].UniqueMatchValue)).FirstOrDefault();
                // Check to see if we have a key and reset the unique MongoDB ID
                if (matchValueKey != null && IdMap.ContainsKey(matchValueKey) && IdMap[matchValueKey] != null)
                    Collection[i].WithMongoId(IdMap[matchValueKey]);
            }

            // Reset the population flag
            MongoIdsPopulated = true;
            // We're done, return the instance
            return this;
        }

        /// <summary>
        /// This method returns the batch count
        /// </summary>
        /// <returns></returns>
        public int CountBatches()
        {
            // Define our number of batches
            int batchCount = (Collection.Count / BatchSize);
            // Compensate for integer division
            if (Collection.Count % 10 != 0) ++batchCount;
            // We're done, return the batch count
            return batchCount;
        }

        /// <summary>
        /// This method returns the total number of documents that were inserted
        /// </summary>
        /// <returns></returns>
        public int CountInsertions() => Collection.Count(c => c.IsReplacement() == false);

        /// <summary>
        /// This method returns the total number of documents that were replaced
        /// </summary>
        /// <returns></returns>
        public int CountReplacements() => Collection.Count(c => c.IsReplacement());

        /// <summary>
        /// This method synchronizes the collection with MongoDB
        /// </summary>
        /// <param name="mongoCollection"></param>
        /// <param name="skipBatch"></param>
        /// <returns></returns>
        public ConsumableCollection<TDocumentObject, TDataTranslationObject, TDocumentKeyType> Synchronize(
            IMongoCollection<TDocumentObject> mongoCollection, bool skipBatch = false)
        {
            // Check to see if we need to batch our operations with a callback
            if (!skipBatch && BatchSize > 0 && BatchCallback != null)
                return Synchronize(mongoCollection, BatchCallback);
            // Check to see if we need to batch our operations
            if (!skipBatch && BatchSize > 0)
                return Synchronize(mongoCollection, null);
            // Make sure the MongoDB IDs are populated
            if (!MongoIdsPopulated) SynchronizeMongoIds(mongoCollection);
            // Check to see if we need to match all and set the match values
            if (MatchAllValues) Collection.ForEach(c => c.WithUniqueMatchValue(MatchValue));
            // Write the documents to MongoDB
            mongoCollection.BulkWrite(ToWriteModels());
            // Synchronize the IDs
            SynchronizeMongoIds(mongoCollection);
            // We're done, return the instance
            return this;
        }

        /// <summary>
        /// This method asynchronously synchronizes the collection
        /// with MongoDB in batches with a callback for each batch
        /// </summary>
        /// <param name="mongoCollection"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public ConsumableCollection<TDocumentObject, TDataTranslationObject, TDocumentKeyType>
            Synchronize(IMongoCollection<TDocumentObject> mongoCollection,
                DelegateSynchronizeBatchCallback callback)
        {
            // Define our iteration
            int iteration = 0;
            // Define our total
            int total = CountBatches();
            // Iterate over the batches
            foreach (IEnumerable<ConsumableCollectionItem<TDocumentObject, TDataTranslationObject,
                    TDocumentKeyType>>
                batch in Collection.Batch(BatchSize))
            {
                // Increment the iterator
                ++iteration;
                // Define our timer
                Stopwatch timer = new Stopwatch();
                // Start the timer
                timer.Start();
                // Branch the child instance
                ConsumableCollection<TDocumentObject, TDataTranslationObject, TDocumentKeyType> collection =
                    ToBatchCollection(batch);
                // Synchronize the collection
                collection.Synchronize(mongoCollection, true);
                // Stop the timer
                timer.Stop();
                // Execute our callback it one is provided
                callback?.Invoke(iteration, total, collection, BatchSize, timer.Elapsed);
            }

            // We're done, return the instance
            return this;
        }

        /// <summary>
        /// This method asynchronously synchronizes the collection with MongoDB
        /// </summary>
        /// <param name="mongoCollection"></param>
        /// <param name="skipBatch"></param>
        /// <returns></returns>
        public async Task<ConsumableCollection<TDocumentObject, TDataTranslationObject, TDocumentKeyType>>
            SynchronizeAsync(IMongoCollection<TDocumentObject> mongoCollection, bool skipBatch = false)
        {
            // Check to see if we need to batch our operations with an asynchronous callback
            if (!skipBatch && BatchSize > 0 && BatchCallbackAsync != null)
                return await SynchronizeAsync(mongoCollection, BatchCallbackAsync);
            // Check to see if we need to batch our operations with a callback
            if (!skipBatch && BatchSize > 0 && BatchCallback != null)
                return await SynchronizeAsync(mongoCollection, BatchCallback);
            // Check to see if we need to batch our operations
            if (!skipBatch && BatchSize > 0)
                return await SynchronizeAsync(mongoCollection, null);
            // Make sure the MongoDB IDs are populated
            if (!MongoIdsPopulated) await SynchronizeMongoIdsAsync(mongoCollection);
            // Check to see if we need to match all and set the match values
            if (MatchAllValues) Collection.ForEach(c => c.WithUniqueMatchValue(MatchValue));
            // Write the documents to MongoDB
            await mongoCollection.BulkWriteAsync(ToWriteModels());
            // Synchronize the IDs
            await SynchronizeMongoIdsAsync(mongoCollection);
            // We're done, return the instance
            return this;
        }

        /// <summary>
        /// This method asynchronously synchronizes the collection
        /// with MongoDB in batches with a callback for each batch
        /// </summary>
        /// <param name="mongoCollection"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public async Task<ConsumableCollection<TDocumentObject, TDataTranslationObject, TDocumentKeyType>>
            SynchronizeAsync(IMongoCollection<TDocumentObject> mongoCollection,
                DelegateSynchronizeBatchCallback callback)
        {
            // Define our iteration
            int iteration = 0;
            // Define our total
            int total = CountBatches();
            // Iterate over the batches
            foreach (IEnumerable<ConsumableCollectionItem<TDocumentObject, TDataTranslationObject,
                    TDocumentKeyType>>
                batch in Collection.Batch(BatchSize))
            {
                // Increment the iterator
                ++iteration;
                // Define our timer
                Stopwatch timer = new Stopwatch();
                // Start the timer
                timer.Start();
                // Branch the child instance
                ConsumableCollection<TDocumentObject, TDataTranslationObject, TDocumentKeyType> collection =
                    ToBatchCollection(batch);
                // Synchronize the collection
                await collection.SynchronizeAsync(mongoCollection, true);
                // Stop the timer
                timer.Stop();
                // Execute our callback it one is provided
                callback?.Invoke(iteration, total, collection, BatchSize, timer.Elapsed);
            }

            // We're done, return the instance
            return this;
        }

        /// <summary>
        /// This method asynchronously synchronizes the collection
        /// with MongoDB in batches with an asynchronous callback for each batch
        /// </summary>
        /// <param name="mongoCollection"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public async Task<ConsumableCollection<TDocumentObject, TDataTranslationObject, TDocumentKeyType>>
            SynchronizeAsync(IMongoCollection<TDocumentObject> mongoCollection,
                DelegateSynchronizeBatchCallbackAsync callback)
        {
            // Define our iteration
            int iteration = 0;
            // Define our total
            int total = CountBatches();
            // Iterate over the batches
            foreach (IEnumerable<ConsumableCollectionItem<TDocumentObject, TDataTranslationObject,
                    TDocumentKeyType>>
                batch in Collection.Batch(BatchSize))
            {
                // Increment the iterator
                ++iteration;
                // Define our timer
                Stopwatch timer = new Stopwatch();
                // Start the timer
                timer.Start();
                // Branch the child instance
                ConsumableCollection<TDocumentObject, TDataTranslationObject, TDocumentKeyType> collection =
                    ToBatchCollection(batch);
                // Synchronize the collection
                await collection.SynchronizeAsync(mongoCollection, true);
                // Stop the timer
                timer.Stop();
                // Execute our callback it one is provided
                await callback?.Invoke(iteration, total, collection, BatchSize, timer.Elapsed);
            }

            // We're done, return the instance
            return this;
        }

        /// <summary>
        /// This method synchronizes the unique MongoDB IDs of the consumables that exist into the instance
        /// </summary>
        /// <param name="mongoCollection"></param>
        /// <returns></returns>
        public ConsumableCollection<TDocumentObject, TDataTranslationObject, TDocumentKeyType> SynchronizeMongoIds(
            IMongoCollection<TDocumentObject> mongoCollection)
        {
            // Localize our $in filter
            FilterDefinition<TDocumentObject> filter = ToInFilter();
            // Localize our cursor
            IAsyncCursor<TDocumentObject> cursor = mongoCollection.FindSync(filter);
            // Localize the list
            List<TDocumentObject> matches = cursor.ToList();
            // Check for matches
            if (!matches.Any()) return this;
            // Find the documents that match
            matches.ForEach(document =>
            {
                // Localize the match value
                TDocumentKeyType matchValue = GetMatchValueFromDocument(document, DocumentMatchExpression);
                // Add the match to the instance
                IdMap.Add(matchValue, document.MongoId);
            });
            // Repopulate the MongoDB IDs
            return PopulateMongoIds();
        }

        /// <summary>
        /// This method asynchronously synchronizes the unique MongoDB IDs of the consumables that exist into the instance
        /// </summary>
        /// <param name="mongoCollection"></param>
        /// <returns></returns>
        public async Task<ConsumableCollection<TDocumentObject, TDataTranslationObject, TDocumentKeyType>>
            SynchronizeMongoIdsAsync(
                IMongoCollection<TDocumentObject> mongoCollection)
        {
            // Localize our $in filter
            FilterDefinition<TDocumentObject> filter = ToInFilter();
            // Localize our cursor
            IAsyncCursor<TDocumentObject> cursor = await mongoCollection.FindAsync(filter);
            // Localize the list of matches
            List<TDocumentObject> matches = await cursor.ToListAsync();
            // Check for results and skip the rest of this
            if (!matches.Any()) return this;
            // Iterate over the items in the list
            foreach (TDocumentObject document in matches)
            {
                // Localize the match value
                TDocumentKeyType matchValue = GetMatchValueFromDocument(document, DocumentMatchExpression);
                // Add the match to the instance
                IdMap.Add(matchValue, document.MongoId);
            }
            // Repopulate the MongoDB IDs
            return PopulateMongoIds();
        }

        /// <summary>
        /// This method generates a child instance for batching consumption
        /// </summary>
        /// <param name="collectionItems"></param>
        /// <returns></returns>
        public ConsumableCollection<TDocumentObject, TDataTranslationObject, TDocumentKeyType> ToBatchCollection(
            IEnumerable<ConsumableCollectionItem<TDocumentObject, TDataTranslationObject, TDocumentKeyType>>
                collectionItems) =>
            new ConsumableCollection<TDocumentObject, TDataTranslationObject, TDocumentKeyType>(collectionItems)
                .WithDocumentMatchExpression(DocumentMatchExpression)
                .WithLogger(Logger)
                .WithMatchValue(MatchValue)
                .WithMatchValueFlag(MatchAllValues)
                .WithValueMatchExpression(MatchValueExpression);

        /// <summary>
        /// This method converts the collection to a list of MongoDB document structures
        /// </summary>
        /// <returns></returns>
        public List<TDocumentObject> ToListOfDocuments() => Collection.Select(c => c.Document.ToDocument()).ToList();

        /// <summary>
        /// This method converts the collection to a list of document DTO structures
        /// </summary>
        /// <returns></returns>
        public List<TDataTranslationObject> ToListOfModels() => Collection.Select(c => c.Document).ToList();

        /// <summary>
        /// This method generates an $ea filter for matching all values in the collection
        /// </summary>
        /// <returns></returns>
        public FilterDefinition<TDocumentObject> ToEqualityFilter() =>
            Builders<TDocumentObject>.Filter.Eq(DocumentMatchExpression, MatchValue);

        /// <summary>
        /// This method generates an $in filter for the match values in the collection
        /// </summary>
        /// <returns></returns>
        public FilterDefinition<TDocumentObject> ToInFilter()
        {
            // Localize our match values
            IEnumerable<TDocumentKeyType> matchValues = Collection
                .Where(c => c.IsInsertion())
                .Select(c => c.UniqueMatchValue);
            // Define our filter
            FilterDefinition<TDocumentObject> filterDefinition =
                Builders<TDocumentObject>.Filter.In(DocumentMatchExpression, matchValues);
            // We're done, return the BSON document
            return filterDefinition;
        }

        /// <summary>
        /// This method converts the collection to a list of write models
        /// </summary>
        /// <returns></returns>
        public IEnumerable<WriteModel<TDocumentObject>> ToWriteModels() =>
            Collection.Select(c => c.ToWriteModel()).Where(c => c != null).ToList();

        /// <summary>
        /// This method sets the batch callback into the instance
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        public ConsumableCollection<TDocumentObject, TDataTranslationObject, TDocumentKeyType> WithBatchCallback(
            DelegateSynchronizeBatchCallback callback)
        {
            // Set the batch callback into the instance
            BatchCallback = callback;
            // We're done, return the instance
            return this;
        }

        /// <summary>
        /// This method sets the asynchronous batch callback into the instance
        /// </summary>
        /// <param name="callbackAsync"></param>
        /// <returns></returns>
        public ConsumableCollection<TDocumentObject, TDataTranslationObject, TDocumentKeyType> WithBatchCallbackAsync(
            DelegateSynchronizeBatchCallbackAsync callbackAsync)
        {
            // Set the asynchronous callback into the instance
            BatchCallbackAsync = callbackAsync;
            // We're done, return the instance
            return this;
        }

        /// <summary>
        /// This method resets the document batch or chunking size when sending documents to MongoDB in bulk
        /// </summary>
        /// <param name="batchSize"></param>
        /// <returns></returns>
        public ConsumableCollection<TDocumentObject, TDataTranslationObject, TDocumentKeyType> WithBatchSize(
            int batchSize)
        {
            // Reset the batch size into the instance
            BatchSize = batchSize;
            // We're done, return the instance
            return this;
        }

        /// <summary>
        /// This method resets the collection into the instance
        /// </summary>
        /// <param name="consumables"></param>
        /// <returns></returns>
        public ConsumableCollection<TDocumentObject, TDataTranslationObject, TDocumentKeyType> WithCollection(
            IEnumerable<ConsumableCollectionItem<TDocumentObject, TDataTranslationObject, TDocumentKeyType>>
                consumables)
        {
            // Clear the existing collection
            Collection.Clear();
            // Repopulate the collection
            Collection.AddRange(consumables);
            // We're done, return the instance
            return this;
        }

        /// <summary>
        /// This method adds a consumableCollectionItem directly to the instance
        /// </summary>
        /// <param name="consumableCollectionItem"></param>
        /// <returns></returns>
        public ConsumableCollection<TDocumentObject, TDataTranslationObject, TDocumentKeyType> WithConsumable(
            ConsumableCollectionItem<TDocumentObject, TDataTranslationObject, TDocumentKeyType>
                consumableCollectionItem)
        {
            // Add the consumable to the instance
            Collection.Add(consumableCollectionItem);
            // Reset the MongoDB ID synchronization flag
            MongoIdsPopulated = false;
            // We're done, return the instance
            return this;
        }

        /// <summary>
        /// This method adds a consumable to the instance from a document DTO
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public ConsumableCollection<TDocumentObject, TDataTranslationObject, TDocumentKeyType> WithConsumable(
            TDataTranslationObject document)
        {
            // Define our consumable
            ConsumableCollectionItem<TDocumentObject, TDataTranslationObject, TDocumentKeyType> consumable =
                new ConsumableCollectionItem<TDocumentObject, TDataTranslationObject, TDocumentKeyType>(document);
            // Check for a match all value
            if (MatchAllValues) consumable.WithUniqueMatchValue(MatchValue);
            // Check to see if we need to loads th unique match value from the DTO
            if (!consumable.HasUniqueMatchValueSet() && (MatchValueExpression != null))
                consumable.WithUniqueMatchValue(GetMatchValueFromModel(document, MatchValueExpression));
            // We're done, add the consumable to the instance
            return WithConsumable(consumable);
        }

        /// <summary>
        /// This method adds a consumable to the instance from a document DTO
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public ConsumableCollection<TDocumentObject, TDataTranslationObject, TDocumentKeyType> WithConsumable(
            TDocumentObject document) =>
            WithConsumable(
                Activator.CreateInstance(typeof(TDataTranslationObject), new[] { document }) as TDataTranslationObject);

        /// <summary>
        /// This method adds a consumable to the instance from a document DTO and
        /// unique match value for matching existing records
        /// </summary>
        /// <param name="document"></param>
        /// <param name="uniqueMatchValue"></param>
        /// <returns></returns>
        public ConsumableCollection<TDocumentObject, TDataTranslationObject, TDocumentKeyType> WithConsumable(
            TDataTranslationObject document, TDocumentKeyType uniqueMatchValue) =>
            WithConsumable(
                new ConsumableCollectionItem<TDocumentObject, TDataTranslationObject, TDocumentKeyType>(document,
                    uniqueMatchValue));

        /// <summary>
        /// This method adds a consumable to the instance from a document and
        /// unique match value for matching existing records
        /// </summary>
        /// <param name="document"></param>
        /// <param name="uniqueMatchValue"></param>
        /// <returns></returns>
        public ConsumableCollection<TDocumentObject, TDataTranslationObject, TDocumentKeyType> WithConsumable(
            TDocumentObject document, TDocumentKeyType uniqueMatchValue) => WithConsumable(
            new ConsumableCollectionItem<TDocumentObject, TDataTranslationObject, TDocumentKeyType>(
                (Activator.CreateInstance(typeof(TDataTranslationObject), new[] { document }) as TDataTranslationObject),
                uniqueMatchValue));

        /// <summary>
        /// This method adds a consumable to the instance from a document DTO with a unique match value expression
        /// to pull the value for matching existing documents
        /// </summary>
        /// <param name="document"></param>
        /// <param name="uniqueMatchValueExpression"></param>
        /// <returns></returns>
        public ConsumableCollection<TDocumentObject, TDataTranslationObject, TDocumentKeyType> WithConsumable(
            TDataTranslationObject document,
            Expression<Func<TDataTranslationObject, TDocumentKeyType>> uniqueMatchValueExpression) =>
            WithConsumable(document, GetMatchValueFromModel(document, uniqueMatchValueExpression));

        /// <summary>
        /// This method adds a consumable to the instance from a document with a unique match value expression
        /// to pull the value for matching existing documents
        /// </summary>
        /// <param name="document"></param>
        /// <param name="uniqueValueMatchExpression"></param>
        /// <returns></returns>
        public ConsumableCollection<TDocumentObject, TDataTranslationObject, TDocumentKeyType> WithConsumable(
            TDocumentObject document, Expression<Func<TDocumentObject, TDocumentKeyType>> uniqueValueMatchExpression) =>
            WithConsumable(document, GetMatchValueFromDocument(document, uniqueValueMatchExpression));

        /// <summary>
        /// This method resets the document match expression into the instance for bulk replaceOne operations
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public ConsumableCollection<TDocumentObject, TDataTranslationObject, TDocumentKeyType>
            WithDocumentMatchExpression(Expression<Func<TDocumentObject, TDocumentKeyType>> expression)
        {
            // Set the document match expression into the instance
            DocumentMatchExpression = expression;
            // We're done, return the instance
            return this;
        }

        /// <summary>
        /// This method
        /// </summary>
        /// <param name="idMap"></param>
        /// <returns></returns>
        public ConsumableCollection<TDocumentObject, TDataTranslationObject, TDocumentKeyType> WithIdMap(
            Dictionary<TDocumentKeyType, string> idMap)
        {
            // Clear the existing ID map
            IdMap.Clear();
            // Iterate over the ID map and set the pair into the instance
            foreach (KeyValuePair<TDocumentKeyType, string> pair in idMap) IdMap[pair.Key] = pair.Value;
            // We're done, repopulate the MongoDB IDs
            return PopulateMongoIds();
        }

        /// <summary>
        /// This method resets the _isOrdered flag into the instance
        /// </summary>
        /// <param name="flag"></param>
        /// <returns></returns>
        public ConsumableCollection<TDocumentObject, TDataTranslationObject, TDocumentKeyType>
            WithIsOrderedFlag(bool flag)
        {
            // Reset the flag into the instance
            _isOrdered = flag;
            // We're done, return the instance
            return this;
        }

        /// <summary>
        /// This method sets a logger into the instance
        /// </summary>
        /// <param name="logger"></param>
        /// <returns></returns>
        public ConsumableCollection<TDocumentObject, TDataTranslationObject, TDocumentKeyType>
            WithLogger(ILogger logger) => WithUnSuppressedLogging(logger);

        /// <summary>
        /// This method set the match value into the instance
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public ConsumableCollection<TDocumentObject, TDataTranslationObject, TDocumentKeyType> WithMatchValue(
            TDocumentKeyType value)
        {
            // Reset the match value into the instance
            MatchValue = value;
            // Reset the match all values flag
            MatchAllValues = true;
            // We're done, return the instance
            return this;
        }

        /// <summary>
        /// This method resets the match all values flag in the instance
        /// </summary>
        /// <param name="flag"></param>
        /// <returns></returns>
        public ConsumableCollection<TDocumentObject, TDataTranslationObject, TDocumentKeyType>
            WithMatchValueFlag(bool flag = true)
        {
            // Set the match all values flag
            MatchAllValues = flag;
            // We're done, return the instance
            return this;
        }

        /// <summary>
        /// This method tells the instance TO order documents on bulk operations
        /// </summary>
        /// <returns></returns>
        public ConsumableCollection<TDocumentObject, TDataTranslationObject, TDocumentKeyType> WithOrderedDocuments() =>
            WithIsOrderedFlag(true);

        /// <summary>
        /// This method tells the instance NOT TO order documents on bulk operations
        /// </summary>
        /// <returns></returns>
        public ConsumableCollection<TDocumentObject, TDataTranslationObject, TDocumentKeyType>
            WithoutOrderedDocuments() =>
            WithIsOrderedFlag(false);

        /// <summary>
        /// This method tells the instance NOT TO skip document validation on bulk operations
        /// </summary>
        /// <returns></returns>
        public ConsumableCollection<TDocumentObject, TDataTranslationObject, TDocumentKeyType>
            WithoutSkippedDocumentValidation() =>
            WithSkipDocumentValidationFlag(false);

        /// <summary>
        /// This method tells the instance NOT TO upsert on replacements
        /// </summary>
        /// <returns></returns>
        public ConsumableCollection<TDocumentObject, TDataTranslationObject, TDocumentKeyType> WithoutUpsert() =>
            WithUpsertFlag(false);

        /// <summary>
        /// This method tells the instance TO skip document validation on bulk operations
        /// </summary>
        /// <returns></returns>
        public ConsumableCollection<TDocumentObject, TDataTranslationObject, TDocumentKeyType>
            WithSkippedDocumentValidation() =>
            WithSkipDocumentValidationFlag(true);

        /// <summary>
        /// This method resets the _skipDocumentValidation flag into the instance
        /// </summary>
        /// <param name="flag"></param>
        /// <returns></returns>
        public ConsumableCollection<TDocumentObject, TDataTranslationObject, TDocumentKeyType>
            WithSkipDocumentValidationFlag(bool flag)
        {
            // Reset the flag into the instance
            _skipDocumentValidation = flag;
            // We're done, return the instance
            return this;
        }

        /// <summary>
        /// This method suppresses logging
        /// </summary>
        /// <returns></returns>
        public ConsumableCollection<TDocumentObject, TDataTranslationObject, TDocumentKeyType> WithSuppressedLogging()
        {
            // Set the suppressed logger
            _suppressedLogger = Logger;
            // Clear the logger
            Logger = null;
            // We're done, return the instance
            return this;
        }

        /// <summary>
        /// This method removes the log suppression by moving the previous logger back into place
        /// </summary>
        /// <returns></returns>
        public ConsumableCollection<TDocumentObject, TDataTranslationObject, TDocumentKeyType> WithUnSuppressedLogging()
        {
            // Set the logger back into the instance
            Logger = _suppressedLogger;
            // Clear the suppressed logger
            _suppressedLogger = null;
            // We're done, return the instance
            return this;
        }

        /// <summary>
        /// This method removes the log suppression by setting a new logger instance
        /// </summary>
        /// <param name="logger"></param>
        /// <returns></returns>
        public ConsumableCollection<TDocumentObject, TDataTranslationObject, TDocumentKeyType> WithUnSuppressedLogging(
            ILogger logger)
        {
            // Set the logger into the instance
            Logger = logger;
            // Reset the suppressed logger
            _suppressedLogger = null;
            // We're done, return the instance
            return this;
        }

        /// <summary>
        /// This method resets the _upsert flag into the instance
        /// </summary>
        /// <param name="flag"></param>
        /// <returns></returns>
        public ConsumableCollection<TDocumentObject, TDataTranslationObject, TDocumentKeyType> WithUpsertFlag(bool flag)
        {
            // Reset the flag into the instance
            _upsert = flag;
            // We're done, return the instance
            return this;
        }

        /// <summary>
        /// This method tells the instance TO upsert on replacements
        /// </summary>
        /// <returns></returns>
        public ConsumableCollection<TDocumentObject, TDataTranslationObject, TDocumentKeyType> WithUpsert() =>
            WithUpsertFlag(true);

        /// <summary>
        /// This method resets the match value expression into the instance
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public ConsumableCollection<TDocumentObject, TDataTranslationObject, TDocumentKeyType> WithValueMatchExpression(
            Expression<Func<TDataTranslationObject, TDocumentKeyType>> expression)
        {
            // Reset the match value expression into the instance
            MatchValueExpression = expression;
            // We're done, return the instance
            return this;
        }

        /// <summary>
        /// This method removes the matchAllValues constraint
        /// </summary>
        /// <returns></returns>
        public ConsumableCollection<TDocumentObject, TDataTranslationObject, TDocumentKeyType> WithOutMatchValue()
        {
            // Reset the match value into the instance
            MatchValue = default;
            // Reset the match all values flag
            MatchAllValues = false;
            // We're done, return the instance
            return this;
        }
    }
}
