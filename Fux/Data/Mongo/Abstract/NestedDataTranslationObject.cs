namespace Fux.Data.Mongo.Abstract
{
    /// <summary>
    /// This class maintains the structure for a MongoDB Data Translation Object (DTO)
    /// </summary>
    /// <typeparam name="TDocument"></typeparam>
    public abstract class NestedDataTranslationObject<TDocument> : IDataTranslationObject<TDocument>
    {
        /// <summary>
        /// This method instantiates an empty DTO
        /// </summary>
        public NestedDataTranslationObject() { }

        /// <summary>
        /// This method instantiates the DTO from an existing MongoDB document
        /// </summary>
        /// <param name="document"></param>
        public NestedDataTranslationObject(TDocument document) => FromDocument(document);

        /// <summary>
        /// This method populates the DTO from an exiting document
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public abstract void FromDocument(TDocument document);

        /// <summary>
        /// This method converts the DTO to a MongoDB document
        /// </summary>
        /// <returns></returns>
        public abstract TDocument ToDocument();
    }
}
