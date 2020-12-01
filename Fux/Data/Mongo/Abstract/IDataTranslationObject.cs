using Newtonsoft.Json;

namespace Fux.Data.Mongo.Abstract
{
    /// <summary>
    /// This interface maintains the structure for a DataTranslation Mongo DB object
    /// </summary>
    public interface IDataTranslationObject<TDocument>
    {
        /// <summary>
        /// This method
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public void FromDocument(TDocument document);

        /// <summary>
        /// This method is responsible for converting the
        /// class to a MongoDB document structure
        /// </summary>
        /// <returns></returns>
        public TDocument ToDocument();
    }
}
