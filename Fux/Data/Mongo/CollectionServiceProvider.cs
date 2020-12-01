using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Fux.Data.Mongo.Abstract;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using Fux.Data.Mongo.ServiceProvider;
using Fux.Data.Mongo.ServiceProvider.Consume;

namespace Fux.Data.Mongo
{
    /// <summary>
    /// This class maintains the structure of a MongoDB Collection Service Provider
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <typeparam name="TDocumentObject"></typeparam>
    /// <typeparam name="TDataTranslationObject"></typeparam>
    public class CollectionServiceProvider<TContext, TDocumentObject, TDataTranslationObject> : ICollectionServiceProvider<TDocumentObject, TDataTranslationObject> where TContext : DbContext<TContext>, new() where TDocumentObject : CollectionModel, new() where TDataTranslationObject : DataTranslationObject<TDocumentObject>, new()
    {
        /// <summary>
        /// This structure maintains the time table for a task
        /// </summary>
        public struct TimeTable
        {
            /// <summary>
            /// This property contains the task has been running
            /// </summary>
            [JsonProperty("elapsed")]
            public TimeSpan Elapsed { get; set; }

            /// <summary>
            /// This property contains the estimated time remaining on the task
            /// </summary>
            [JsonProperty("remaining")]
            public TimeSpan Remaining { get; set; }

            /// <summary>
            /// This property contains the total estimated time the process will take
            /// </summary>
            [JsonProperty("total")]
            public TimeSpan Total { get; set; }
        }

        /// <summary>
        /// This property contains the MongoDB collection interface
        /// </summary>
        public IMongoCollection<TDocumentObject> Collection { get; private set; }

        /// <summary>
        /// This property contains the logger instance
        /// </summary>
        protected readonly ILogger<CollectionServiceProvider<TContext, TDocumentObject, TDataTranslationObject>> Logger;

        /// <summary>
        /// This method instantiates the service provider with a MongoDB collection
        /// </summary>
        /// <param name="collectionName"></param>
        /// <param name="logger"></param>
        public CollectionServiceProvider(string collectionName, ILogger<CollectionServiceProvider<TContext, TDocumentObject, TDataTranslationObject>> logger = null)
        {
            // Set the collection into the instance
            Collection = Engine.Instance().Context<TContext>().Collection<TDocumentObject>(collectionName);
            // Set the logger into the instance
            Logger = logger;
        }

        /// <summary>
        /// This method gets the value of <typeparamref name="TDocumentKeyType"/>
        /// from the <paramref name="document"/> MongoDB document using <paramref name="expression"/>
        /// </summary>
        /// <param name="document"></param>
        /// <param name="expression"></param>
        /// <typeparam name="TDocumentKeyType"></typeparam>
        /// <returns></returns>
        protected TDocumentKeyType GetDocumentKeyTypeValueFromDocument<TDocumentKeyType>(TDocumentObject document,
            Expression<Func<TDocumentObject, TDocumentKeyType>> expression)
        {
            // Localize the member expression
            MemberExpression memberExpression = (expression.Body as MemberExpression);
            // Make sure we have a member expression
            if (memberExpression != null)
            {
                // Localize the property information
                PropertyInfo property = (memberExpression.Member as PropertyInfo);
                // Make sure we have property information and set the value
                if (property != null) return (TDocumentKeyType)property.GetValue(document);
            }

            // We're done, nothing to return
            return default;
        }

        /// <summary>
        /// This method gets the value of <typeparamref name="TDocumentKeyType"/>
        /// from the <paramref name="document"/> DTO using <paramref name="expression"/>
        /// </summary>
        /// <param name="document"></param>
        /// <param name="expression"></param>
        /// <typeparam name="TDocumentKeyType"></typeparam>
        /// <returns></returns>
        protected TDocumentKeyType GetDocumentKeyTypeValueFromDto<TDocumentKeyType>(TDataTranslationObject document,
            Expression<Func<TDataTranslationObject, TDocumentKeyType>> expression)
        {
            // Localize the member expression
            MemberExpression memberExpression = (expression.Body as MemberExpression);
            // Make sure we have a member expression
            if (memberExpression != null)
            {
                // Localize the property information
                PropertyInfo property = (memberExpression.Member as PropertyInfo);
                // Make sure we have property information and set the value
                if (property != null) return (TDocumentKeyType)property.GetValue(document);
            }
            // We're done, nothing to return
            return default;
        }

        /// <summary>
        /// This method builds the message
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private string buildMessage(string message)
        {
            // Reset the message
            return $"[{DateTime.Now:d}, {DateTime.Now:t}]:\t{message}";
        }

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
        /// This method generates a time table for a service
        /// </summary>
        /// <param name="timer"></param>
        /// <param name="iteration"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        protected TimeTable TimeTableService(Stopwatch timer, int? iteration = null, int? total = null)
        {
            // Define our structure
            TimeTable response = new TimeTable();
            // Stop the timer
            timer.Stop();
            // Set the elapsed time into the response
            response.Elapsed = timer.Elapsed;
            // Check for an iteration and total
            if (iteration.HasValue && total.HasValue)
            {
                // Set the time remaining into the response
                response.Remaining = TimeSpan.FromMilliseconds((timer.ElapsedMilliseconds / iteration.Value) *
                                                               (total.Value - iteration.Value));
                // Set the total into the response
                response.Total = response.Elapsed.Add(response.Remaining);
            }
            // Start the timer
            timer.Start();
            // We're done, send the response
            return response;
        }

        /// <summary>
        /// This method generates a time table for a task
        /// </summary>
        /// <param name="timer"></param>
        /// <param name="iteration"></param>
        /// <param name="total"></param>
        /// <param name="restart"></param>
        /// <returns></returns>
        protected TimeTable TimeTableTask(Stopwatch timer, int? iteration = null, int? total = null, bool restart = true)
        {
            // Define our structure
            TimeTable response = new TimeTable();
            // Stop the timer
            timer.Stop();
            // Set the elapsed time into the response
            response.Elapsed = timer.Elapsed;
            // Check for an iteration and total
            if (iteration.HasValue && total.HasValue)
            {
                // Set the time remaining into the response
                response.Remaining = TimeSpan.FromMilliseconds((timer.ElapsedMilliseconds / iteration.Value) *
                                                               (total.Value - iteration.Value));
                // Set the total into the response
                response.Total = response.Elapsed.Add(response.Remaining);
            }

            // Check the restart flag and restart the timer
            if (restart) timer.Restart();
            // We're done, send the response
            return response;
        }

        /// <summary>
        /// This method consumes many documents into MongoDB from an external source
        /// </summary>
        /// <param name="stopwatch"></param>
        /// <returns></returns>
        public virtual List<TDataTranslationObject> ConsumeMany(Stopwatch stopwatch)
        {
            // This method is not implemented on the base class
            throw new NotImplementedException();
        }

        /// <summary>
        /// This method consumes many documents into MongoDB from an external source
        /// </summary>
        /// <param name="collection"></param>
        /// <typeparam name="TDocumentKeyType"></typeparam>
        /// <returns></returns>
        public List<TDataTranslationObject> ConsumeMany<TDocumentKeyType>(
            ConsumableCollection<TDocumentObject, TDataTranslationObject, TDocumentKeyType> collection) =>
            collection.Synchronize(Collection).ToListOfModels();

        /// <summary>
        /// This method consumes many documents into MongoDB from an external source using a collection builder callback
        /// </summary>
        /// <param name="callback"></param>
        /// <typeparam name="TDocumentKeyType"></typeparam>
        /// <returns></returns>
        public List<TDataTranslationObject> ConsumeMany<TDocumentKeyType>(
            ICollectionServiceProvider<TDocumentObject, TDataTranslationObject>.DelegateConsumableCollectionBuilder<
                TDocumentKeyType> callback)
        {
            // Instantiate our collection
            ConsumableCollection<TDocumentObject, TDataTranslationObject, TDocumentKeyType> collection =
                new ConsumableCollection<TDocumentObject, TDataTranslationObject, TDocumentKeyType>();
            // Invoke the callback with the collection
            callback.Invoke(collection);
            // We're done, consume the collection
            return ConsumeMany(collection);
        }


        /// <summary>
        /// This method asynchronously consumes many documents into MongoDB from an external source
        /// </summary>
        /// <param name="stopwatch"></param>
        /// <param name="noLog"></param>
        /// <returns></returns>
        public virtual Task<List<TDataTranslationObject>> ConsumeManyAsync(Stopwatch stopwatch, bool noLog = false)
        {
            // This method is not implemented on the base class
            throw new NotImplementedException();
        }

        /// <summary>
        /// This method asynchronously consumes many documents into MongoDB from an external source
        /// </summary>
        /// <param name="collection"></param>
        /// <typeparam name="TDocumentKeyType"></typeparam>
        /// <returns></returns>
        public async Task<List<TDataTranslationObject>> ConsumeManyAsync<TDocumentKeyType>(
            ConsumableCollection<TDocumentObject, TDataTranslationObject, TDocumentKeyType> collection) =>
            (await collection.SynchronizeAsync(Collection)).ToListOfModels();

        /// <summary>
        /// This method asynchronously consumes many documents into MongoDB from an external source using a collection builder callback
        /// </summary>
        /// <param name="callback"></param>
        /// <typeparam name="TDocumentKeyType"></typeparam>
        /// <returns></returns>
        public Task<List<TDataTranslationObject>> ConsumeManyAsync<TDocumentKeyType>(
            ICollectionServiceProvider<TDocumentObject, TDataTranslationObject>.DelegateConsumableCollectionBuilder<TDocumentKeyType> callback)
        {
            // Instantiate our collection
            ConsumableCollection<TDocumentObject, TDataTranslationObject, TDocumentKeyType> collection =
                new ConsumableCollection<TDocumentObject, TDataTranslationObject, TDocumentKeyType>();
            // Invoke the callback with the collection
            callback.Invoke(collection);
            // We're done, consume the collection
            return ConsumeManyAsync(collection);
        }

        /// <summary>
        /// This method asynchronously consumes many documents into MongoDB from an external source using an asynchronous collection builder callback
        /// </summary>
        /// <param name="callback"></param>
        /// <typeparam name="TDocumentKeyType"></typeparam>
        /// <returns></returns>
        public async Task<List<TDataTranslationObject>> ConsumeManyAsync<TDocumentKeyType>(
            ICollectionServiceProvider<TDocumentObject, TDataTranslationObject>.DelegateConsumableCollectionBuilderAsync<TDocumentKeyType> callback)
        {
            // Instantiate our collection
            ConsumableCollection<TDocumentObject, TDataTranslationObject, TDocumentKeyType> collection =
                new ConsumableCollection<TDocumentObject, TDataTranslationObject, TDocumentKeyType>();
            // Invoke the callback with the collection
            await callback.Invoke(collection);
            // We're done, consume the collection
            return await ConsumeManyAsync(collection);
        }

        /// <summary>
        /// This method consumes a single document into MongoDB from an external source
        /// </summary>
        /// <param name="stopwatch"></param>
        /// <returns></returns>
        public virtual TDataTranslationObject ConsumeOne(Stopwatch stopwatch)
        {
            // This method is not implemented on the base class
            throw new NotImplementedException();
        }

        /// <summary>
        /// This method consumes a single document into MongoDB from an external source
        /// as well as expressions to check for existing documents before saving
        /// </summary>
        /// <param name="document"></param>
        /// <typeparam name="TDocumentKeyType"></typeparam>
        /// <returns></returns>
        public TDataTranslationObject ConsumeOne<TDocumentKeyType>(
            ConsumeEntry<TDocumentObject, TDataTranslationObject, TDocumentKeyType> document)
        {
            // Save the document to MongoDB
            document.Save(Collection);
            // We're done, return the document
            return document.Document;
        }

        /// <summary>
        /// This method asynchronously consumes a single document into MongoDB from an external source
        /// </summary>
        /// <param name="stopwatch"></param>
        /// <returns></returns>
        public virtual Task<TDataTranslationObject> ConsumeOneAsync(Stopwatch stopwatch)
        {
            // This method is not implemented on the base class
            throw new NotImplementedException();
        }

        /// <summary>
        /// This method asynchronously consumes a single document into MongoDB from an external source
        /// as well as expressions to check for existing documents before saving
        /// </summary>
        /// <param name="document"></param>
        /// <typeparam name="TDocumentKeyType"></typeparam>
        /// <returns></returns>
        public async Task<TDataTranslationObject> ConsumeOneAsync<TDocumentKeyType>(ConsumeEntry<TDocumentObject, TDataTranslationObject, TDocumentKeyType> document)
        {
            // Save the document to MongoDB
            await document.SaveAsync(Collection);
            // We're done, return the document
            return document.Document;
        }

        /// <summary>
        /// This method deletes a MongoDB document that matches <paramref name="filter"/>
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public TDataTranslationObject Delete(FilterDefinition<TDocumentObject> filter)
        {
            // Localize the document
            TDataTranslationObject document = Find<TDataTranslationObject>(filter, d => d);
            // Delete the document from MongoDB
            Collection.DeleteOne(filter);
            // We're done, return the deleted item
            return document;
        }

        /// <summary>
        /// This method deletes a MongoDB document by its unique MongoDB ID
        /// </summary>
        /// <param name="mongoId"></param>
        /// <returns></returns>
        public TDataTranslationObject Delete(string mongoId) =>
            Delete(Builders<TDocumentObject>.Filter.Eq(d => d.MongoId, mongoId));

        /// <summary>
        /// This method deletes a MongoDB document by its unique MongoDB ID
        /// </summary>
        /// <param name="mongoId"></param>
        /// <returns></returns>
        public TDataTranslationObject Delete(ObjectId mongoId) => Delete(mongoId.ToString());

        /// <summary>
        /// This method deletes all MongoDB documents that match <paramref name="filter"/>
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public List<TDataTranslationObject> DeleteAll(FilterDefinition<TDocumentObject> filter)
        {
            // Localize the list of documents
            List<TDataTranslationObject> documents = List(filter);
            // Delete the documents from MongoDB
            Collection.DeleteMany(filter);
            // We're done, send the deleted objects
            return documents;
        }

        /// <summary>
        /// This method asynchronously deletes all MongoDB documents that match <paramref name="filter"/>
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<List<TDataTranslationObject>> DeleteAllAsync(FilterDefinition<TDocumentObject> filter)
        {
            // Localize the list of documents
            List<TDataTranslationObject> documents = await ListAsync(filter);
            // Delete the documents from MongoDB
            await Collection.DeleteManyAsync(filter);
            // We're done, send the deleted objects
            return documents;
        }

        /// <summary>
        /// This method asynchronously deletes a MongoDB document that matches <paramref name="filter"/>
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<TDataTranslationObject> DeleteAsync(FilterDefinition<TDocumentObject> filter)
        {
            // Localize the document
            TDataTranslationObject document =
                (Activator.CreateInstance(typeof(TDataTranslationObject), new[] { await FindAsync(filter) }) as
                    TDataTranslationObject);
            // Delete the document from MongoDB
            await Collection.DeleteOneAsync(filter);
            // We're done, return the deleted item
            return document;
        }

        /// <summary>
        /// This method asynchronously deletes a MongoDB document by its unique MongoDB ID
        /// </summary>
        /// <param name="mongoId"></param>
        /// <returns></returns>
        public Task<TDataTranslationObject> DeleteAsync(string mongoId) =>
            DeleteAsync(Builders<TDocumentObject>.Filter.Eq(d => d.MongoId, mongoId));

        /// <summary>
        /// This method asynchronously deletes a MongoDB document by its unique MongoDB ID
        /// </summary>
        /// <param name="mongoId"></param>
        /// <returns></returns>
        public Task<TDataTranslationObject> DeleteAsync(ObjectId mongoId) => DeleteAsync(mongoId.ToString());

        /// <summary>
        /// This method finds a MongoDB document that matches <paramref name="filter"/> with a transformer callback
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="transformer"></param>
        /// <typeparam name="TDocument"></typeparam>
        /// <returns></returns>
        public TDocument Find<TDocument>(FilterDefinition<TDocumentObject> filter,
            ICollectionServiceProvider<TDocumentObject, TDataTranslationObject>.DelegateTransformer<TDocument>
                transformer)
        {
            // Query for the document
            TDocumentObject document = Collection.Aggregate().Match(filter).FirstOrDefault();
            // Define our response
            TDocument response = default;
            // Check the document object and execute the transformer
            if (document != null && !string.IsNullOrEmpty(document.MongoId) &&
                !string.IsNullOrWhiteSpace(document.MongoId))
                response =
                    transformer.Invoke(
                        Activator.CreateInstance(typeof(TDataTranslationObject), document) as TDataTranslationObject);
            // We're done, return the document
            return response;
        }

        /// <summary>
        /// This method finds a MongoDB document by its unique MongoDB ID with a transformer callback
        /// </summary>
        /// <param name="mongoId"></param>
        /// <param name="transformer"></param>
        /// <typeparam name="TDocument"></typeparam>
        /// <returns></returns>
        public TDocument Find<TDocument>(string mongoId,
            ICollectionServiceProvider<TDocumentObject, TDataTranslationObject>.DelegateTransformer<TDocument>
                transformer) =>
            Find<TDocument>(Builders<TDocumentObject>.Filter.Eq(d => d.MongoId, mongoId), transformer);

        /// <summary>
        /// This method finds a MongoDB document by its unique MongoDB ID with a transformer callback
        /// </summary>
        /// <param name="mongoId"></param>
        /// <param name="transformer"></param>
        /// <typeparam name="TDocument"></typeparam>
        /// <returns></returns>
        public TDocument Find<TDocument>(ObjectId mongoId,
            ICollectionServiceProvider<TDocumentObject, TDataTranslationObject>.DelegateTransformer<TDocument>
                transformer) =>
            Find<TDocument>(mongoId.ToString(), transformer);

        /// <summary>
        /// This method finds a MongoDB document that matches <paramref name="filter"/>
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public TDataTranslationObject Find(FilterDefinition<TDocumentObject> filter) =>
            Find<TDataTranslationObject>(filter, d => d);

        /// <summary>
        /// This method finds a MongoDB document by its unique MongoDB ID
        /// </summary>
        /// <param name="mongoId"></param>
        /// <returns></returns>
        public TDataTranslationObject Find(string mongoId) => Find<TDataTranslationObject>(mongoId, d => d);

        /// <summary>
        /// This method finds a MongoDB document by its unique MongoDB ID
        /// </summary>
        /// <param name="mongoId"></param>
        /// <returns></returns>
        public TDataTranslationObject Find(ObjectId mongoId) => Find<TDataTranslationObject>(mongoId, d => d);

        /// <summary>
        /// This method asynchronously finds a MongoDB document that matches <paramref name="filter"/>
        /// with a transformer callback
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="transformer"></param>
        /// <typeparam name="TDocument"></typeparam>
        /// <returns></returns>
        public async Task<TDocument> FindAsync<TDocument>(FilterDefinition<TDocumentObject> filter,
            ICollectionServiceProvider<TDocumentObject, TDataTranslationObject>.DelegateTransformer<TDocument>
                transformer)
        {
            // Query for the document
            TDocumentObject document = await Collection.Aggregate().Match(filter).FirstOrDefaultAsync();
            // Define our response
            TDocument response = default;
            // Check the document object and execute the transformer
            if (document != null && !string.IsNullOrEmpty(document.MongoId) &&
                !string.IsNullOrWhiteSpace(document.MongoId))
                response =
                    transformer.Invoke(
                        Activator.CreateInstance(typeof(TDataTranslationObject), document) as TDataTranslationObject);
            // We're done, return the document
            return response;
        }

        /// <summary>
        /// This method asynchronously finds a MongoDB document that matches <paramref name="filter"/>
        /// with an asynchronous transformer callback
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="transformer"></param>
        /// <typeparam name="TDocument"></typeparam>
        /// <returns></returns>
        public async Task<TDocument> FindAsync<TDocument>(FilterDefinition<TDocumentObject> filter,
            ICollectionServiceProvider<TDocumentObject, TDataTranslationObject>.DelegateTransformerAsync<TDocument>
                transformer) =>
            await transformer.Invoke(
                Activator.CreateInstance(typeof(TDataTranslationObject),
                        await Collection.Aggregate().Match(filter).FirstOrDefaultAsync())
                    as TDataTranslationObject);

        /// <summary>
        /// This method asynchronously finds a MongoDB document by its unique MongoDB ID with a transformer callback
        /// </summary>
        /// <param name="mongoId"></param>
        /// <param name="transformer"></param>
        /// <typeparam name="TDocument"></typeparam>
        /// <returns></returns>
        public Task<TDocument> FindAsync<TDocument>(string mongoId,
            ICollectionServiceProvider<TDocumentObject, TDataTranslationObject>.DelegateTransformer<TDocument>
                transformer) =>
            FindAsync<TDocument>(Builders<TDocumentObject>.Filter.Eq(d => d.MongoId, mongoId), transformer);

        /// <summary>
        /// This method asynchronously finds a MongoDB document by its unique MongoDB ID with an asynchronous transformer callback
        /// </summary>
        /// <param name="mongoId"></param>
        /// <param name="transformer"></param>
        /// <typeparam name="TDocument"></typeparam>
        /// <returns></returns>
        public Task<TDocument> FindAsync<TDocument>(string mongoId,
            ICollectionServiceProvider<TDocumentObject, TDataTranslationObject>.DelegateTransformerAsync<TDocument>
                transformer) =>
            FindAsync<TDocument>(mongoId, transformer);

        /// <summary>
        /// This method asynchronously finds a MongoDB document by its unique MongoDB ID with a transformer callback
        /// </summary>
        /// <param name="mongoId"></param>
        /// <param name="transformer"></param>
        /// <typeparam name="TDocument"></typeparam>
        /// <returns></returns>
        public Task<TDocument> FindAsync<TDocument>(ObjectId mongoId,
            ICollectionServiceProvider<TDocumentObject, TDataTranslationObject>.DelegateTransformer<TDocument>
                transformer) =>
            FindAsync<TDocument>(mongoId.ToString(), transformer);

        /// <summary>
        /// This method asynchronously finds a MongoDB document by its unique MongoDB ID with an asynchronous transformer callback
        /// </summary>
        /// <param name="mongoId"></param>
        /// <param name="transformer"></param>
        /// <typeparam name="TDocument"></typeparam>
        /// <returns></returns>
        public Task<TDocument> FindAsync<TDocument>(ObjectId mongoId,
            ICollectionServiceProvider<TDocumentObject, TDataTranslationObject>.DelegateTransformerAsync<TDocument>
                transformer) =>
            FindAsync<TDocument>(mongoId.ToString(), transformer);

        /// <summary>
        /// This method asynchronously finds a MongoDB document that matches <paramref name="filter"/>
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public Task<TDataTranslationObject> FindAsync(FilterDefinition<TDocumentObject> filter) =>
            FindAsync<TDataTranslationObject>(filter, d => d);

        /// <summary>
        /// This method asynchronously finds a MongoDB document by its unique MongoDB ID
        /// </summary>
        /// <param name="mongoId"></param>
        /// <returns></returns>
        public Task<TDataTranslationObject> FindAsync(string mongoId) =>
            FindAsync<TDataTranslationObject>(mongoId, d => d);

        /// <summary>
        /// This method asynchronously finds a MongoDB document by its unique MongoDB ID
        /// </summary>
        /// <param name="mongoId"></param>
        /// <returns></returns>
        public Task<TDataTranslationObject> FindAsync(ObjectId mongoId) =>
            FindAsync<TDataTranslationObject>(mongoId, d => d);

        /// <summary>
        /// This method lists the MongoDB documents that match <paramref name="filter"/> with a transformer callback
        /// as well as a result set transformer callback
        /// </summary>
        /// <param name="transformer"></param>
        /// <param name="transformerResults"></param>
        /// <param name="filter"></param>
        /// <typeparam name="TDocument"></typeparam>
        /// <typeparam name="TResultSet"></typeparam>
        /// <returns></returns>
        public TResultSet List<TDocument, TResultSet>(
            ICollectionServiceProvider<TDocumentObject, TDataTranslationObject>.DelegateTransformer<TDocument>
                transformer,
            ICollectionServiceProvider<TDocumentObject, TDataTranslationObject>.DelegateTransformerResults<TDocument,
                TResultSet> transformerResults,
            FilterDefinition<TDocumentObject> filter = null)
        {
            // Define our results
            List<TDocument> results = new List<TDocument>();
            // Localize our aggregation
            IAggregateFluent<TDocumentObject> aggregation = Collection.Aggregate();
            // Check for a filter and reset the cursor
            if (filter != null)
                aggregation = aggregation.Match(filter);
            // Query MongoDB and iterate over the results
            aggregation.ToList().ForEach(d =>
                results.Add(transformer.Invoke(
                    Activator.CreateInstance(typeof(TDataTranslationObject), new[] { d }) as TDataTranslationObject)));
            // Execute the results transformer
            TResultSet response = transformerResults.Invoke(results, results.LongCount());
            // We're done, send the response
            return response;
        }

        /// <summary>
        /// This method lists the MongoDB documents that match <paramref name="filter"/>
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public List<TDataTranslationObject> List(FilterDefinition<TDocumentObject> filter = null) =>
            List<TDataTranslationObject, List<TDataTranslationObject>>(d => d, (r, t) => r, filter);

        /// <summary>
        /// This method asynchronously lists the MongoDB documents that match <paramref name="filter"/>
        /// with a transformer callback as well as a result set transformer callback
        /// </summary>
        /// <param name="transformer"></param>
        /// <param name="transformerResults"></param>
        /// <param name="filter"></param>
        /// <typeparam name="TDocument"></typeparam>
        /// <typeparam name="TResultSet"></typeparam>
        /// <returns></returns>
        public async Task<TResultSet> ListAsync<TDocument, TResultSet>(
            ICollectionServiceProvider<TDocumentObject, TDataTranslationObject>.DelegateTransformer<TDocument>
                transformer,
            ICollectionServiceProvider<TDocumentObject, TDataTranslationObject>.DelegateTransformerResults<TDocument,
                TResultSet> transformerResults,
            FilterDefinition<TDocumentObject> filter = null)
        {
            // Define our results
            List<TDocument> results = new List<TDocument>();
            // Localize our aggregation
            IAggregateFluent<TDocumentObject> aggregation = Collection.Aggregate();
            // Check for a filter and reset the cursor
            if (filter != null)
                aggregation = aggregation.Match(filter);
            // Query MongoDB and iterate over the results
            await aggregation.ForEachAsync(d =>
                results.Add(transformer.Invoke(
                    Activator.CreateInstance(typeof(TDataTranslationObject), new[] { d }) as TDataTranslationObject)));
            // Execute the results transformer
            TResultSet response = transformerResults.Invoke(results, results.LongCount());
            // We're done, send the response
            return response;
        }

        /// <summary>
        /// This method asynchronously lists the MongoDB documents that match <paramref name="filter"/>
        /// with an asynchronous transformer callback as well as an asynchronous result set callback
        /// </summary>
        /// <param name="transformer"></param>
        /// <param name="transformerResults"></param>
        /// <param name="filter"></param>
        /// <typeparam name="TDocument"></typeparam>
        /// <typeparam name="TResultSet"></typeparam>
        /// <returns></returns>
        public async Task<TResultSet> ListAsync<TDocument, TResultSet>(
            ICollectionServiceProvider<TDocumentObject, TDataTranslationObject>.DelegateTransformerAsync<TDocument>
                transformer,
            ICollectionServiceProvider<TDocumentObject, TDataTranslationObject>.DelegateTransformerResultsAsync<
                TDocument, TResultSet> transformerResults, FilterDefinition<TDocumentObject> filter = null)
        {
            // Define our results
            List<TDocument> results = new List<TDocument>();
            // Localize our aggregation
            IAggregateFluent<TDocumentObject> aggregation = Collection.Aggregate();
            // Check for a filter and reset the cursor
            if (filter != null)
                aggregation = aggregation.Match(filter);
            // Query MongoDB and iterate over the results
            await aggregation.ForEachAsync(async d =>
                results.Add(await transformer.Invoke(
                    Activator.CreateInstance(typeof(TDataTranslationObject), new[] { d }) as TDataTranslationObject)));
            // Execute the results transformer
            TResultSet response = await transformerResults.Invoke(results, results.LongCount());
            // We're done, send the response
            return response;
        }

        /// <summary>
        /// This method asynchronously lists the MongoDB documents that match <paramref name="filter"/>
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public Task<List<TDataTranslationObject>> ListAsync(FilterDefinition<TDocumentObject> filter = null) =>
            ListAsync<TDataTranslationObject, List<TDataTranslationObject>>(d => d, (r, t) => r, filter);

        /// <summary>
        /// This method creates or replaces a MongoDB document
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public TDataTranslationObject Save(TDataTranslationObject document)
        {
            // Check the document for a unique MongoDB ID and replace the document in MongoDB
            if (!string.IsNullOrEmpty(document.MongoId) && !string.IsNullOrWhiteSpace(document.MongoId))
                return Save(document.MongoId, document);
            // Localize the document
            TDocumentObject mongoDocument = document.ToDocument();
            // Save the document to MongoDB
            Collection.InsertOne(mongoDocument);
            // We're done, return the document with the MongoDB ID
            return (Activator.CreateInstance(typeof(TDataTranslationObject), new[] { mongoDocument }) as
                TDataTranslationObject);
        }

        /// <summary>
        /// This method replaces a MongoDB document using its unique ID
        /// </summary>
        /// <param name="mongoId"></param>
        /// <param name="document"></param>
        /// <returns></returns>
        public TDataTranslationObject Save(string mongoId, TDataTranslationObject document)
        {
            // Set the unique MongoDB ID into the document
            document.MongoId = mongoId;
            // Replace the document in MongoDB
            Collection.ReplaceOne(Builders<TDocumentObject>.Filter.Eq(d => d.MongoId, document.MongoId),
                document.ToDocument());
            // We're done, return the document
            return document;
        }

        /// <summary>
        /// This method replaces a MongoDB document using its unique ID
        /// </summary>
        /// <param name="mongoId"></param>
        /// <param name="document"></param>
        /// <returns></returns>
        public TDataTranslationObject Save(ObjectId mongoId, TDataTranslationObject document) =>
            Save(mongoId.ToString(), document);

        /// <summary>
        /// This method asynchronously creates or replaces a MongoDB document
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public async Task<TDataTranslationObject> SaveAsync(TDataTranslationObject document)
        {
            // Check the document for a unique MongoDB ID and replace the document in MongoDB
            if (!string.IsNullOrEmpty(document.MongoId) && !string.IsNullOrWhiteSpace(document.MongoId))
                return await SaveAsync(document.MongoId, document);
            // Localize the document
            TDocumentObject mongoDocument = document.ToDocument();
            // Save the document to MongoDB
            await Collection.InsertOneAsync(mongoDocument);
            // We're done, return the document with the MongoDB ID
            return (Activator.CreateInstance(typeof(TDataTranslationObject), new[] { mongoDocument }) as
                TDataTranslationObject);
        }

        /// <summary>
        /// This method asynchronously replaces a MongoDB document using its unique ID
        /// </summary>
        /// <param name="mongoId"></param>
        /// <param name="document"></param>
        /// <returns></returns>
        public async Task<TDataTranslationObject> SaveAsync(string mongoId, TDataTranslationObject document)
        {
            // Set the unique MongoDB ID into the document
            document.MongoId = mongoId;
            // Replace the document in MongoDB
            await Collection.ReplaceOneAsync(Builders<TDocumentObject>.Filter.Eq(d => d.MongoId, document.MongoId),
                document.ToDocument());
            // We're done, return the document
            return document;
        }

        /// <summary>
        /// This method asynchronously replaces a MongoDB document using its unique ID
        /// </summary>
        /// <param name="mongoId"></param>
        /// <param name="document"></param>
        /// <returns></returns>
        public Task<TDataTranslationObject> SaveAsync(ObjectId mongoId, TDataTranslationObject document) =>
            SaveAsync(mongoId.ToString(), document);
    }
}
