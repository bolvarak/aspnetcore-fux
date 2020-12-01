using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using Fux.Data.Mongo.Abstract;

namespace Fux.Data.Mongo.ServiceProvider.Consume
{
    /// <summary>
    /// This class maintains the structure for a consumable collection item
    /// </summary>
    /// <typeparam name="TDocumentObject"></typeparam>
    /// <typeparam name="TDataTranslationObject"></typeparam>
    /// <typeparam name="TDocumentKeyType"></typeparam>
    public class ConsumableCollectionItem<TDocumentObject, TDataTranslationObject, TDocumentKeyType> where TDocumentObject : CollectionModel where TDataTranslationObject : DataTranslationObject<TDocumentObject>
    {
        /// <summary>
        /// This property contains the MongoDB document DTO
        /// </summary>
        [JsonProperty("document")] public readonly TDataTranslationObject Document;

        /// <summary>
        /// This property contains the unique MongoDB ID of the document
        /// </summary>
        [JsonProperty("mongoId")] public string MongoId { get; set; }

        /// <summary>
        /// This property contains the unique match value for checking
        /// duplicate records when a MongoDB ID is not available
        /// </summary>
        [JsonProperty("uniqueMatchValue")] public TDocumentKeyType UniqueMatchValue { get; set; }

        /// <summary>
        /// This property denotes a match value has been set
        /// </summary>
        private bool _hasUniqueMatchValue = false;

        /// <summary>
        /// This method instantiates the class with a Document DTO
        /// </summary>
        /// <param name="document"></param>
        public ConsumableCollectionItem(TDataTranslationObject document)
        {
            // Set the document DTO into the instance
            Document = document;
            // Ensure the unique MongoDB ID
            WithMongoId(document.MongoId);
        }

        /// <summary>
        /// This method instantiates the class with a Document DTO and unique MongoDB ID
        /// </summary>
        /// <param name="document"></param>
        /// <param name="mongoId"></param>
        public ConsumableCollectionItem(TDataTranslationObject document, string mongoId)
        {
            // Set the document DTO into the instance
            Document = document;
            // Set the unique MongoDB ID into the instance
            WithMongoId(mongoId);
        }

        /// <summary>
        /// This method instantiates the class with a Document DTO and unique MongoDB ID
        /// </summary>
        /// <param name="document"></param>
        /// <param name="mongoId"></param>
        public ConsumableCollectionItem(TDataTranslationObject document, ObjectId mongoId)
        {
            // Set the document DTO into the instance
            Document = document;
            // Set the unique MongoDB ID into the instance
            WithMongoId(mongoId.ToString());
        }

        /// <summary>
        /// This method instantiates the class with a Document DTO and unique match value
        /// </summary>
        /// <param name="document"></param>
        /// <param name="uniqueMatchValue"></param>
        public ConsumableCollectionItem(TDataTranslationObject document, TDocumentKeyType uniqueMatchValue)
        {
            // Set the document DTO into the instance
            Document = document;
            // Set the unique match value into the instance
            WithUniqueMatchValue(uniqueMatchValue);
        }

        /// <summary>
        /// This method instantiates the class with a Document DTO, unique MongoDB ID and unique match value
        /// </summary>
        /// <param name="document"></param>
        /// <param name="mongoId"></param>
        /// <param name="uniqueMatchValue"></param>
        public ConsumableCollectionItem(TDataTranslationObject document, string mongoId,
            TDocumentKeyType uniqueMatchValue)
        {
            // Set the document DTO into the instance
            Document = document;
            // Set the unique MongoDB ID into the instance
            WithMongoId(mongoId);
            // Set the unique match value into the instance
            WithUniqueMatchValue(uniqueMatchValue);
        }

        /// <summary>
        /// This method instantiates the class with a Document DTO, unique MongoDB ID and unique match value
        /// </summary>
        /// <param name="document"></param>
        /// <param name="mongoId"></param>
        /// <param name="uniqueMatchValue"></param>
        public ConsumableCollectionItem(TDataTranslationObject document, ObjectId mongoId,
            TDocumentKeyType uniqueMatchValue)
        {
            // Set the document DTO into the instance
            Document = document;
            // Set the unique MongoDB ID into the instance
            MongoId = mongoId.ToString();
            // Set the unique match value into the instance
            UniqueMatchValue = uniqueMatchValue;
        }

        /// <summary>
        /// This method returns the match value set flag
        /// </summary>
        /// <returns></returns>
        public bool HasUniqueMatchValueSet() => _hasUniqueMatchValue;

        /// <summary>
        /// This method determines whether or not the document is an insertion
        /// </summary>
        /// <returns></returns>
        public bool IsInsertion() => !IsUpsert();

        /// <summary>
        /// This method determine whether or not the document is a replacement
        /// </summary>
        /// <returns></returns>
        public bool IsReplacement() => IsUpsert();

        /// <summary>
        /// This method determines whether or not item is an upsert
        /// </summary>
        /// <returns></returns>
        public bool IsUpsert()
        {
            // Check for a MongoDB ID in the instance and document
            if ((string.IsNullOrEmpty(MongoId) || string.IsNullOrWhiteSpace(MongoId)) &&
                (string.IsNullOrEmpty(Document.MongoId) || string.IsNullOrWhiteSpace(Document.MongoId)))
                return false;
            // Check for a MongoDB ID in the document
            if ((string.IsNullOrEmpty(MongoId) || string.IsNullOrWhiteSpace(MongoId)) &&
                (!string.IsNullOrEmpty(Document.MongoId) && !string.IsNullOrWhiteSpace(Document.MongoId)))
            {
                // Set the unique MongoDB ID into the instance
                WithMongoId(Document.MongoId);
            }
            // Check the unique MongoDB ID for null or empty and return
            if (string.IsNullOrEmpty(MongoId) || string.IsNullOrWhiteSpace(MongoId)) return false;
            // We've made it this far, the MongoDB ID is solid
            // which means that this document needs replacing
            return true;
        }

        /// <summary>
        /// This method generates a $where filter from the instance values
        /// </summary>
        /// <returns></returns>
        public FilterDefinition<TDocumentObject> ToFilter() =>
            Builders<TDocumentObject>.Filter.Where(d => d.MongoId.Equals(MongoId));

        /// <summary>
        /// This method generates an insertOne or replaceOne writeModel from the instance values
        /// </summary>
        /// <returns></returns>
        public WriteModel<TDocumentObject> ToWriteModel() =>
            (IsUpsert() ? ToWriteModelReplaceOne() : ToWriteModelInsertOne() as WriteModel<TDocumentObject>);

        /// <summary>
        /// This method generates an insertOne writeModel from the instance values
        /// </summary>
        /// <returns></returns>
        public InsertOneModel<TDocumentObject> ToWriteModelInsertOne() =>
            new InsertOneModel<TDocumentObject>(Document.ToDocument());

        /// <summary>
        /// This method generates a replaceOne writeModel from the instance values
        /// </summary>
        /// <returns></returns>
        public ReplaceOneModel<TDocumentObject> ToWriteModelReplaceOne() =>
            new ReplaceOneModel<TDocumentObject>(ToFilter(), Document.ToDocument());

        /// <summary>
        /// This method resets the unique MongoDB ID into the instance and document DTO
        /// </summary>
        /// <param name="mongoId"></param>
        /// <returns></returns>
        public ConsumableCollectionItem<TDocumentObject, TDataTranslationObject, TDocumentKeyType> WithMongoId(string mongoId)
        {
            // Reset the unique MongoDB ID into the instance
            MongoId = mongoId;
            // Reset the unique MongoDB ID into the document DTO
            Document.MongoId = MongoId;
            // We're done, return the instance
            return this;
        }

        /// <summary>
        /// This method resets the unique MongoDB ID into the instance and document DTO
        /// </summary>
        /// <param name="mongoId"></param>
        /// <returns></returns>
        public ConsumableCollectionItem<TDocumentObject, TDataTranslationObject, TDocumentKeyType> WithMongoId(
            ObjectId mongoId) =>
            WithMongoId(mongoId.ToString());

        /// <summary>
        /// This method resets the unique and statically typed match value into the instance
        /// </summary>
        /// <param name="uniqueMatchValue"></param>
        /// <returns></returns>
        public ConsumableCollectionItem<TDocumentObject, TDataTranslationObject, TDocumentKeyType> WithUniqueMatchValue(
            TDocumentKeyType uniqueMatchValue)
        {
            // Reset the unique match value into the instance
            UniqueMatchValue = uniqueMatchValue;
            // Reset the match value flag
            _hasUniqueMatchValue = true;
            // We're done, return the instance
            return this;
        }
    }
}
