using System;

namespace Fux.Data.Mongo.Abstract.Implementation
{
    /// <summary>
    /// This class provides a mock implementation for a DbContext
    /// </summary>
    public class DbContextImpl : DbContext<DbContextImpl>
    {
        /// <summary>
        /// This method instantiates an empty context
        /// </summary>
        public DbContextImpl() : base() { }

        /// <summary>
        /// This method bootstraps the configuration of the context
        /// </summary>
        /// <param name="clientBuilder"></param>
        /// <exception cref="NotImplementedException"></exception>
        protected override void OnConfiguring(ClientBuilder clientBuilder) =>
            throw new NotImplementedException("This is a Mock Class");

        /// <summary>
        /// This method bootstraps the collection configuration of the context
        /// </summary>
        /// <param name="collectionBuilder"></param>
        /// <exception cref="NotImplementedException"></exception>
        protected override void OnConfiguringCollections(
            CollectionBuilder<DbContextImpl> collectionBuilder) =>
            throw new NotImplementedException("This is a Mock Class");
    }
}
