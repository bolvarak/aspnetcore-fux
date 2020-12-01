using System;
using Newtonsoft.Json;

namespace Fux.Data.Mongo.Abstract
{
    /// <summary>
    /// This class maintains the structure for a MongoDB Data Translation Object (DTO)
    /// </summary>
    /// <typeparam name="TDocument"></typeparam>
    public class DataTranslationObject<TDocument> : IDataTranslationObject<TDocument>
    {
        /// <summary>
        /// This property contain the unique MongoDB document ID
        /// </summary>
        [JsonProperty("_id")]
        public string MongoId { get; set; }

        /// <summary>
        /// This method instantiates an empty DTO
        /// </summary>
        public DataTranslationObject() { }

        /// <summary>
        /// This method instantiates the DTO from an existing MongoDB document
        /// </summary>
        /// <param name="document"></param>
        public DataTranslationObject(TDocument document) => FromDocument(document);

        /// <summary>
        /// This method populates the DTO from an exiting document
        /// </summary>
        /// <param name="document"></param>
        public virtual void FromDocument(TDocument document) =>
            throw new NotImplementedException();

        /// <summary>
        /// This method converts the DTO to a MongoDB document
        /// </summary>
        /// <returns></returns>
        public virtual TDocument ToDocument() =>
            throw new NotImplementedException();
    }
}
