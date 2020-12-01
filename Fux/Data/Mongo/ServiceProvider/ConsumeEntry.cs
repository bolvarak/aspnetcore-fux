using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Driver;
using Newtonsoft.Json;
using Fux.Data.Mongo.Abstract;

namespace Fux.Data.Mongo.ServiceProvider
{
    /// <summary>
    /// This class maintains the structure for a consumer entry
    /// </summary>
    /// <typeparam name="TDocumentObject"></typeparam>
    /// <typeparam name="TDataTranslationObject"></typeparam>
    /// <typeparam name="TDocumentKeyType"></typeparam>
    public class ConsumeEntry<TDocumentObject, TDataTranslationObject, TDocumentKeyType>
        where TDocumentObject : CollectionModel where TDataTranslationObject : DataTranslationObject<TDocumentObject>
    {
        /// <summary>
        /// This property contains the MongoDB document in DTO format
        /// </summary>
        [JsonProperty("document")] public readonly TDataTranslationObject Document;

        /// <summary>
        /// This property contains the expression to match for the filter
        /// </summary>
        [JsonProperty("expression")] public readonly Expression<Func<TDocumentObject, bool>> Expression;

        /// <summary>
        /// This property contains the value to match in MongoDB
        /// </summary>
        [JsonProperty("matchValue")] public readonly TDocumentKeyType MatchValue;

        /// <summary>
        /// This property contains the MongoDB ID of the document
        /// </summary>
        [JsonProperty("mongoId")]
        public string MongoId { get; set; }

        /// <summary>
        /// This property defines whether or not the document was inserted [true] or updated [false]
        /// </summary>
        [JsonProperty("upsert")]
        public bool Upsert { get; set; }

        /// <summary>
        /// This method instantiates the class with a DTO document, replacement express and match value
        /// </summary>
        /// <param name="document"></param>
        /// <param name="expression"></param>
        /// <param name="matchValue"></param>
        public ConsumeEntry(TDataTranslationObject document,
            Expression<Func<TDocumentObject, bool>> expression, TDocumentKeyType matchValue)
        {
            // Set the document into the instance
            Document = document;
            // Set the expression into the instance
            Expression = expression;
            // Set the match value into the instance
            MatchValue = matchValue;
        }

        /// <summary>
        /// This method inserts or replaces the document in MongoDB
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public TDocumentObject Save(IMongoCollection<TDocumentObject> collection)
        {
            // Define our find and replace options
            FindOneAndReplaceOptions<TDocumentObject, TDocumentObject> options =
                new FindOneAndReplaceOptions<TDocumentObject, TDocumentObject>()
                { IsUpsert = true, BypassDocumentValidation = true };
            // Localize the document
            TDocumentObject document = ToDocument();
            // Save the document to MongoDB
            collection.FindOneAndReplace(ToFilter(), document,
                new FindOneAndReplaceOptions<TDocumentObject, TDocumentObject>() { IsUpsert = true });
            // Set the MongoDB ID into the instance
            MongoId = document.MongoId;
            // Repopulate the DTO
            Document.FromDocument(document);
            // We're done, return the document
            return document;
        }

        /// <summary>
        /// This method asynchronously inserts or replace the document in MongoDB
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public async Task<TDocumentObject> SaveAsync(IMongoCollection<TDocumentObject> collection)
        {
            // Define our find and replace options
            FindOneAndReplaceOptions<TDocumentObject, TDocumentObject> options =
                new FindOneAndReplaceOptions<TDocumentObject, TDocumentObject>()
                { IsUpsert = true, BypassDocumentValidation = true };
            // Localize the document
            TDocumentObject document = ToDocument();
            // Save the document to MongoDB
            await collection.ReplaceOneAsync(ToFilter(), document,
                new ReplaceOptions() { IsUpsert = true, BypassDocumentValidation = true });
            // Set the MongoDB ID into the instance
            MongoId = document.MongoId;
            // Repopulate the DTO
            Document.FromDocument(document);
            // We're done, return the document
            return document;
        }

        /// <summary>
        /// This method converts the DTO into a MongoDB document
        /// </summary>
        /// <returns></returns>
        public TDocumentObject ToDocument() => Document.ToDocument();

        /// <summary>
        /// This method generates a MongoDB replacement filter from the instance values
        /// </summary>
        /// <returns></returns>
        public FilterDefinition<TDocumentObject> ToFilter() =>
            Builders<TDocumentObject>.Filter.Where(Expression);

        /// <summary>
        /// This method generates a MongoDB replacement write model from the instance values
        /// </summary>
        /// <returns></returns>
        public ReplaceOneModel<TDocumentObject> ToReplaceOneModel()
        {
            // Define our replacement write model
            ReplaceOneModel<TDocumentObject>
                writeModel = new ReplaceOneModel<TDocumentObject>(ToFilter(), ToDocument()) { IsUpsert = true };
            // We're done, return the write model
            return writeModel;
        }
    }
}
