using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Fux.Data.Mongo.ServiceProvider;
using Fux.Data.Mongo.ServiceProvider.Consume;

namespace Fux.Data.Mongo.Abstract
{
    /// <summary>
    /// This interface maintains the structure for a MongoDB collection service provider
    /// </summary>
    /// <typeparam name="TDocumentObject"></typeparam>
    /// <typeparam name="TDataTranslationObject"></typeparam>
    public interface ICollectionServiceProvider<TDocumentObject, TDataTranslationObject> where TDocumentObject : CollectionModel, new() where TDataTranslationObject : DataTranslationObject<TDocumentObject>, new()
    {
        /// <summary>
        /// This property contains the MongoDB collection interface
        /// </summary>
        public IMongoCollection<TDocumentObject> Collection { get; }

        /// <summary>
        /// This delegate provides the structure for a consumable collection builder callback
        /// </summary>
        /// <param name="collection"></param>
        /// <typeparam name="TDocumentKeyType"></typeparam>
        public delegate void DelegateConsumableCollectionBuilder<TDocumentKeyType>(
            ConsumableCollection<TDocumentObject, TDataTranslationObject, TDocumentKeyType> collection);

        /// <summary>
        /// This delegate provides the structure for an asynchronous consumable collection builder callback
        /// </summary>
        /// <param name="collection"></param>
        /// <typeparam name="TDocumentKeyType"></typeparam>
        public delegate Task DelegateConsumableCollectionBuilderAsync<TDocumentKeyType>(
            ConsumableCollection<TDocumentObject, TDataTranslationObject, TDocumentKeyType> collection);

        /// <summary>
        /// this delegate provides the structure for a transformerCallback
        /// </summary>
        /// <param name="document"></param>
        /// <typeparam name="TDocument"></typeparam>
        public delegate TDocument DelegateTransformer<out TDocument>(TDataTranslationObject document);

        /// <summary>
        /// This delegate provides the structure for an asynchronous transformerCallback
        /// </summary>
        /// <param name="document"></param>
        /// <typeparam name="TDocument"></typeparam>
        public delegate Task<TDocument> DelegateTransformerAsync<TDocument>(TDataTranslationObject document);

        /// <summary>
        /// This delegate provides the structure for a onBeforeSave callback
        /// </summary>
        /// <param name="document"></param>
        public delegate void DelegateTransformerBeforeSave(TDataTranslationObject document);

        /// <summary>
        /// This delegate provides the structure for an asynchronous onBeforeSave callback
        /// </summary>
        /// <param name="document"></param>
        public delegate Task DelegateTransformerBeforeSaveAsync(TDataTranslationObject document);

        /// <summary>
        /// This delegate provides the structure for a result set transformerCallback
        /// </summary>
        /// <param name="documents"></param>
        /// <param name="totalResults"></param>
        /// <typeparam name="TDocument"></typeparam>
        /// <typeparam name="TResultSet"></typeparam>
        public delegate TResultSet DelegateTransformerResults<TDocument, out TResultSet>(List<TDocument> documents,
            long totalResults);

        /// <summary>
        /// This delegate provides the structure for an asynchronous transformerCallback
        /// </summary>
        /// <param name="documents"></param>
        /// <param name="totalResults"></param>
        /// <typeparam name="TDocument"></typeparam>
        /// <typeparam name="TResultSet"></typeparam>
        public delegate Task<TResultSet> DelegateTransformerResultsAsync<TDocument, TResultSet>(
            List<TDocument> documents, long totalResults);

        /// <summary>
        /// This method consumes many documents into MongoDB from an external source
        /// </summary>
        /// <param name="stopwatch"></param>
        /// <returns></returns>
        public List<TDataTranslationObject> ConsumeMany(Stopwatch stopwatch);

        /// <summary>
        /// This method consumes many documents into MongoDB from an external source
        /// </summary>
        /// <param name="collection"></param>
        /// <typeparam name="TDocumentKeyType"></typeparam>
        /// <returns></returns>
        public List<TDataTranslationObject> ConsumeMany<TDocumentKeyType>(ConsumableCollection<TDocumentObject, TDataTranslationObject, TDocumentKeyType> collection);

        /// <summary>
        /// This method consumes many documents into MongoDB from an external source using a collection builder callback
        /// </summary>
        /// <param name="callback"></param>
        /// <typeparam name="TDocumentKeyType"></typeparam>
        /// <returns></returns>
        public List<TDataTranslationObject> ConsumeMany<TDocumentKeyType>(DelegateConsumableCollectionBuilder<TDocumentKeyType> callback);

        /// <summary>
        /// This method asynchronously consumes many documents into MongoDB from an external source
        /// </summary>
        /// <param name="noLog"></param>
        /// <param name="stopwatch"></param>
        /// <returns></returns>
        public Task<List<TDataTranslationObject>> ConsumeManyAsync(Stopwatch stopwatch, bool noLog = false);

        /// <summary>
        /// This method asynchronously consumes many documents into MongoDB from an external source
        /// </summary>
        /// <param name="collection"></param>
        /// <typeparam name="TDocumentKeyType"></typeparam>
        /// <returns></returns>
        public Task<List<TDataTranslationObject>> ConsumeManyAsync<TDocumentKeyType>(ConsumableCollection<TDocumentObject, TDataTranslationObject, TDocumentKeyType> collection);

        /// <summary>
        /// This method asynchronously consumes many documents into MongoDB from an external source using a collection builder callback
        /// </summary>
        /// <param name="callback"></param>
        /// <typeparam name="TDocumentKeyType"></typeparam>
        /// <returns></returns>
        public Task<List<TDataTranslationObject>> ConsumeManyAsync<TDocumentKeyType>(DelegateConsumableCollectionBuilder<TDocumentKeyType> callback);

        /// <summary>
        /// This method asynchronously consumes many documents into MongoDB from an external source using an asynchronous collection builder callback
        /// </summary>
        /// <param name="callback"></param>
        /// <typeparam name="TDocumentKeyType"></typeparam>
        /// <returns></returns>
        public Task<List<TDataTranslationObject>> ConsumeManyAsync<TDocumentKeyType>(DelegateConsumableCollectionBuilderAsync<TDocumentKeyType> callback);

        /// <summary>
        /// This method consumes a single document into MongoDB from an external source
        /// </summary>
        /// <param name="stopwatch"></param>
        /// <returns></returns>
        public TDataTranslationObject ConsumeOne(Stopwatch stopwatch);

        /// <summary>
        /// This method consumes a single document into MongoDB from an external source
        /// as well as expressions to check for existing documents before saving
        /// </summary>
        /// <param name="document"></param>
        /// <typeparam name="TDocumentKeyType"></typeparam>
        /// <returns></returns>
        public TDataTranslationObject ConsumeOne<TDocumentKeyType>(
            ConsumeEntry<TDocumentObject, TDataTranslationObject, TDocumentKeyType> document);

        /// <summary>
        /// This method asynchronously consumes a single document into MongoDB from an external source
        /// </summary>
        /// <param name="stopwatch"></param>
        /// <returns></returns>
        public Task<TDataTranslationObject> ConsumeOneAsync(Stopwatch stopwatch);

        /// <summary>
        /// This method asynchronously consumes a single document into MongoDB from an external source
        /// as well as expressions to check for existing documents before saving
        /// </summary>
        /// <param name="document"></param>
        /// <typeparam name="TDocumentKeyType"></typeparam>
        /// <returns></returns>
        public Task<TDataTranslationObject> ConsumeOneAsync<TDocumentKeyType>(
            ConsumeEntry<TDocumentObject, TDataTranslationObject, TDocumentKeyType> document);

        /// <summary>
        /// This method deletes a MongoDB document that matches <paramref name="filter"/>
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public TDataTranslationObject Delete(FilterDefinition<TDocumentObject> filter);

        /// <summary>
        /// This method deletes a MongoDB document by its unique MongoDB ID
        /// </summary>
        /// <param name="mongoId"></param>
        /// <returns></returns>
        public TDataTranslationObject Delete(string mongoId);

        /// <summary>
        /// This method deletes a MongoDB document by its unique MongoDB ID
        /// </summary>
        /// <param name="mongoId"></param>
        /// <returns></returns>
        public TDataTranslationObject Delete(ObjectId mongoId);

        /// <summary>
        /// This method deletes all MongoDB documents that match <paramref name="filter"/>
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public List<TDataTranslationObject> DeleteAll(FilterDefinition<TDocumentObject> filter);

        /// <summary>
        /// This method asynchronously deletes all MongoDB documents that match <paramref name="filter"/>
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public Task<List<TDataTranslationObject>> DeleteAllAsync(FilterDefinition<TDocumentObject> filter);

        /// <summary>
        /// This method asynchronously deletes a MongoDB document that matches <paramref name="filter"/>
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public Task<TDataTranslationObject> DeleteAsync(FilterDefinition<TDocumentObject> filter);

        /// <summary>
        /// This method asynchronously deletes a MongoDB document by its unique MongoDB ID
        /// </summary>
        /// <param name="mongoId"></param>
        /// <returns></returns>
        public Task<TDataTranslationObject> DeleteAsync(string mongoId);

        /// <summary>
        /// This method asynchronously deletes a MongoDB document by its unique MongoDB ID
        /// </summary>
        /// <param name="mongoId"></param>
        /// <returns></returns>
        public Task<TDataTranslationObject> DeleteAsync(ObjectId mongoId);

        /// <summary>
        /// This method finds a MongoDB document that matches <paramref name="filter"/> with a transformer callback
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="transformer"></param>
        /// <typeparam name="TDocument"></typeparam>
        /// <returns></returns>
        public TDocument Find<TDocument>(FilterDefinition<TDocumentObject> filter,
            DelegateTransformer<TDocument> transformer);

        /// <summary>
        /// This method finds a MongoDB document by its unique MongoDB ID with a transformer callback
        /// </summary>
        /// <param name="mongoId"></param>
        /// <param name="transformer"></param>
        /// <typeparam name="TDocument"></typeparam>
        /// <returns></returns>
        public TDocument Find<TDocument>(string mongoId, DelegateTransformer<TDocument> transformer);

        /// <summary>
        /// This method finds a MongoDB document by its unique MongoDB ID with a transformer callback
        /// </summary>
        /// <param name="mongoId"></param>
        /// <param name="transformer"></param>
        /// <typeparam name="TDocument"></typeparam>
        /// <returns></returns>
        public TDocument Find<TDocument>(ObjectId mongoId, DelegateTransformer<TDocument> transformer);

        /// <summary>
        /// This method finds a MongoDB document that matches <paramref name="filter"/>
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public TDataTranslationObject Find(FilterDefinition<TDocumentObject> filter);

        /// <summary>
        /// This method finds a MongoDB document by its unique MongoDB ID
        /// </summary>
        /// <param name="mongoId"></param>
        /// <returns></returns>
        public TDataTranslationObject Find(string mongoId);

        /// <summary>
        /// This method finds a MongoDB document by its unique MongoDB ID
        /// </summary>
        /// <param name="mongoId"></param>
        /// <returns></returns>
        public TDataTranslationObject Find(ObjectId mongoId);

        /// <summary>
        /// This method asynchronously finds a MongoDB document that matches <paramref name="filter"/>
        /// with a transformer callback
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="transformer"></param>
        /// <typeparam name="TDocument"></typeparam>
        /// <returns></returns>
        public Task<TDocument> FindAsync<TDocument>(FilterDefinition<TDocumentObject> filter,
            DelegateTransformer<TDocument> transformer);

        /// <summary>
        /// This method asynchronously finds a MongoDB document that matches <paramref name="filter"/>
        /// with an asynchronous transformer callback
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="transformer"></param>
        /// <typeparam name="TDocument"></typeparam>
        /// <returns></returns>
        public Task<TDocument> FindAsync<TDocument>(FilterDefinition<TDocumentObject> filter,
            DelegateTransformerAsync<TDocument> transformer);

        /// <summary>
        /// This method asynchronously finds a MongoDB document by its unique MongoDB ID with a transformer callback
        /// </summary>
        /// <param name="mongoId"></param>
        /// <param name="transformer"></param>
        /// <typeparam name="TDocument"></typeparam>
        /// <returns></returns>
        public Task<TDocument> FindAsync<TDocument>(string mongoId, DelegateTransformer<TDocument> transformer);

        /// <summary>
        /// This method asynchronously finds a MongoDB document by its unique MongoDB ID with an asynchronous transformer callback
        /// </summary>
        /// <param name="mongoId"></param>
        /// <param name="transformer"></param>
        /// <typeparam name="TDocument"></typeparam>
        /// <returns></returns>
        public Task<TDocument> FindAsync<TDocument>(string mongoId, DelegateTransformerAsync<TDocument> transformer);

        /// <summary>
        /// This method asynchronously finds a MongoDB document by its unique MongoDB ID with a transformer callback
        /// </summary>
        /// <param name="mongoId"></param>
        /// <param name="transformer"></param>
        /// <typeparam name="TDocument"></typeparam>
        /// <returns></returns>
        public Task<TDocument> FindAsync<TDocument>(ObjectId mongoId, DelegateTransformer<TDocument> transformer);

        /// <summary>
        /// This method asynchronously finds a MongoDB document by its unique MongoDB ID with an asynchronous transformer callback
        /// </summary>
        /// <param name="mongoId"></param>
        /// <param name="transformer"></param>
        /// <typeparam name="TDocument"></typeparam>
        /// <returns></returns>
        public Task<TDocument> FindAsync<TDocument>(ObjectId mongoId, DelegateTransformerAsync<TDocument> transformer);

        /// <summary>
        /// This method asynchronously finds a MongoDB document that matches <paramref name="filter"/>
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public Task<TDataTranslationObject> FindAsync(FilterDefinition<TDocumentObject> filter);

        /// <summary>
        /// This method asynchronously finds a MongoDB document by its unique MongoDB ID
        /// </summary>
        /// <param name="mongoId"></param>
        /// <returns></returns>
        public Task<TDataTranslationObject> FindAsync(string mongoId);

        /// <summary>
        /// This method asynchronously finds a MongoDB document by its unique MongoDB ID
        /// </summary>
        /// <param name="mongoId"></param>
        /// <returns></returns>
        public Task<TDataTranslationObject> FindAsync(ObjectId mongoId);

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
        public TResultSet List<TDocument, TResultSet>(DelegateTransformer<TDocument> transformer,
            DelegateTransformerResults<TDocument, TResultSet> transformerResults,
            FilterDefinition<TDocumentObject> filter = null);

        /// <summary>
        /// This method lists the MongoDB documents that match <paramref name="filter"/>
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public List<TDataTranslationObject> List(FilterDefinition<TDocumentObject> filter = null);

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
        public Task<TResultSet> ListAsync<TDocument, TResultSet>(DelegateTransformer<TDocument> transformer,
            DelegateTransformerResults<TDocument, TResultSet> transformerResults,
            FilterDefinition<TDocumentObject> filter = null);

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
        public Task<TResultSet> ListAsync<TDocument, TResultSet>(DelegateTransformerAsync<TDocument> transformer,
            DelegateTransformerResultsAsync<TDocument, TResultSet> transformerResults,
            FilterDefinition<TDocumentObject> filter = null);

        /// <summary>
        /// This method asynchronously lists the MongoDB documents that match <paramref name="filter"/>
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public Task<List<TDataTranslationObject>> ListAsync(FilterDefinition<TDocumentObject> filter = null);

        /// <summary>
        /// This method creates or replaces a MongoDB document
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public TDataTranslationObject Save(TDataTranslationObject document);

        /// <summary>
        /// This method replaces a MongoDB document using its unique ID
        /// </summary>
        /// <param name="mongoId"></param>
        /// <param name="document"></param>
        /// <returns></returns>
        public TDataTranslationObject Save(string mongoId, TDataTranslationObject document);

        /// <summary>
        /// This method replaces a MongoDB document using its unique ID
        /// </summary>
        /// <param name="mongoId"></param>
        /// <param name="document"></param>
        /// <returns></returns>
        public TDataTranslationObject Save(ObjectId mongoId, TDataTranslationObject document);

        /// <summary>
        /// This method asynchronously creates or replaces a MongoDB document
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public Task<TDataTranslationObject> SaveAsync(TDataTranslationObject document);

        /// <summary>
        /// This method asynchronously replaces a MongoDB document using its unique ID
        /// </summary>
        /// <param name="mongoId"></param>
        /// <param name="document"></param>
        /// <returns></returns>
        public Task<TDataTranslationObject> SaveAsync(string mongoId, TDataTranslationObject document);

        /// <summary>
        /// This method asynchronously replaces a MongoDB document using its unique ID
        /// </summary>
        /// <param name="mongoId"></param>
        /// <param name="document"></param>
        /// <returns></returns>
        public Task<TDataTranslationObject> SaveAsync(ObjectId mongoId, TDataTranslationObject document);
    }
}
