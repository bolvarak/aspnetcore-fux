using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace Fux.Data.Mongo.Abstract
{
    public interface ICollectionModel
    {
        /// <summary>
        /// This property contains the unique MongoDB ID
        /// </summary>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonProperty("_id")]
        public string MongoId { get; set; }

        /// <summary>
        /// This method returns the collection name from the instance
        /// </summary>
        /// <returns></returns>
        public string CollectionName();

        /// <summary>
        /// This method tells the model which collection it belongs to
        /// </summary>
        /// <param name="collectionName"></param>
        /// <returns></returns>
        public CollectionModel FromCollection(string collectionName);

        /// <summary>
        /// This method pulls the collection name from an existing collection
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public CollectionModel FromCollection<TDocument>(IMongoCollection<TDocument> collection);

        /// <summary>
        /// This method converts the collection model to a MongoDB collection
        /// </summary>
        /// <param name="database"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IMongoCollection<T> ToMongoCollection<T>(IMongoDatabase database) where T : ICollectionModel;
    }
}
