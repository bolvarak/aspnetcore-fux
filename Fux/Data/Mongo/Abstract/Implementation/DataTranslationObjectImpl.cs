using System;

namespace Fux.Data.Mongo.Abstract.Implementation
{
    /// <summary>
    /// This class provides a mock implementation for a document DTO
    /// </summary>
    public class DataTranslationObjectImpl : DataTranslationObject<CollectionModel>
    {
        /// <summary>
        /// This method populates the instance from an existing document
        /// </summary>
        /// <param name="document"></param>
        /// <exception cref="NotImplementedException"></exception>
        public override void FromDocument(CollectionModel document) =>
            throw new System.NotImplementedException("This is a Mock DTO");

        /// <summary>
        /// This method converts the instance to a document
        /// </summary>
        /// <returns></returns>
        public override CollectionModel ToDocument() =>
            throw new System.NotImplementedException("This is a Mock DTO");
    }
}
