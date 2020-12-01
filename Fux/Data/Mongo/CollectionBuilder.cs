using System.Collections.Generic;
using MongoDB.Driver;
using Fux.Data.Mongo.Abstract;
using Fux.Data.Mongo.Abstract.Implementation;

namespace Fux.Data.Mongo
{
    /// <summary>
    /// This class maintains the structure of a collection builder interface
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public class CollectionBuilder<TContext> where TContext: DbContext<TContext>, new()
    {
        /// <summary>
        /// This delegate defines the structure for a fluid model configuration using a generic
        /// </summary>
        /// <param name="entity"></param>
        /// <typeparam name="TDocument"></typeparam>
        /// <typeparam name="TDataTranslationObject"></typeparam>
        public delegate void
            DelegateWithEntity<TDocument, TDataTranslationObject>(
                Entity<TContext, TDocument, TDataTranslationObject> entity) where TDocument : CollectionModel, new()
            where TDataTranslationObject : DataTranslationObject<TDocument>, new();

        /// <summary>
        /// This property contains our collection models
        /// </summary>
        public readonly List<dynamic> Collection =
            new List<dynamic>();

        /// <summary>
        /// This property contains the MongoDB context interface
        /// </summary>
        public TContext Context { get; protected set; }

        /// <summary>
        /// This property contains the MongoDB interface
        /// </summary>
        public IMongoDatabase Database { get; protected set; }

        /// <summary>
        /// This property contains the MongoDB collections for the builder
        /// </summary>
        public readonly List<IMongoCollection<ICollectionModel>> MongoCollection =
            new List<IMongoCollection<ICollectionModel>>();

        /// <summary>
        /// This method instantiates an empty class
        /// </summary>
        public CollectionBuilder() { }

        /// <summary>
        /// This method instantiates the CollectionBuilder with context and MongoDB database interface
        /// </summary>
        /// <param name="mongoContext"></param>
        /// <param name="mongoDatabase"></param>
        public CollectionBuilder(TContext mongoContext, IMongoDatabase mongoDatabase)
        {
            // Set the mongo context into the instance
            Context = mongoContext;
            // Set the mongo DB interface into the instance
            Database = mongoDatabase;
        }

        /// <summary>
        /// This method lists the entities from the instance
        /// </summary>
        /// <returns></returns>
        public List<dynamic> ListEntities() =>
            Collection;

        /// <summary>
        /// This method lists the entities from the instance as partially typed entities
        /// </summary>
        /// <returns></returns>
        public List<Entity<TContext, TDocument, DataTranslationObject<TDocument>>> ListEntities<TDocument>()
            where TDocument : CollectionModel, new()
        {
            // Define our response list
            List<Entity<TContext, TDocument, DataTranslationObject<TDocument>>> collection =
                new List<Entity<TContext, TDocument, DataTranslationObject<TDocument>>>();
            // Iterate over the collection
            Collection.ForEach(e => collection.Add(e as Entity<TContext, TDocument, DataTranslationObject<TDocument>>));
            // We're done, return the collection
            return collection;
        }

        /// <summary>
        /// This method lists the entities from the instance as typed entities
        /// </summary>
        /// <typeparam name="TDocument"></typeparam>
        /// <typeparam name="TDataTranslationObject"></typeparam>
        /// <returns></returns>
        public List<Entity<TContext, TDocument, TDataTranslationObject>>
            ListEntities<TDocument, TDataTranslationObject>()
            where TDocument : CollectionModel, new()
            where TDataTranslationObject : DataTranslationObject<TDocument>, new()
        {
            // Define our response list
            List<Entity<TContext, TDocument, TDataTranslationObject>> collection =
                new List<Entity<TContext, TDocument, TDataTranslationObject>>();
            // Iterate over the collection
            collection.ForEach(e => collection.Add(e as Entity<TContext, TDocument, TDataTranslationObject>));
            // We're done, return the collection
            return collection;
        }

        /// <summary>
        /// This method sets the context into the instance
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public CollectionBuilder<TContext> WithContext(TContext context)
        {
            // Set the context into the instance
            Context = context;
            // We're done, return the instance
            return this;
        }

        /// <summary>
        /// This method sets the database into the instance
        /// </summary>
        /// <param name="database"></param>
        /// <returns></returns>
        public CollectionBuilder<TContext> WithDatabase(IMongoDatabase database)
        {
            // Set the database into the instance
            Database = database;
            // We're done, return the instance
            return this;
        }

        /// <summary>
        /// This method adds an existing entity to the instance
        /// </summary>
        /// <param name="entity"></param>
        /// <typeparam name="TDocument"></typeparam>
        /// <typeparam name="TDataTranslationObject"></typeparam>
        /// <returns></returns>
        public CollectionBuilder<TContext>
            WithEntity<TDocument, TDataTranslationObject>(Entity<TContext, TDocument, TDataTranslationObject> entity)
            where TDocument : CollectionModel, new() where TDataTranslationObject : DataTranslationObject<TDocument>, new()
        {
            // Remove any existing entity
            Collection.RemoveAll(e => e.Type == entity.Type);
            // Add the entity to the instance
            Collection.Add(entity);
            // Attache the entity to the context
            entity.ToContextInstanceProperty();
            // We're done, return the instance
            return this;
        }

        /// <summary>
        /// This method registers an entity type with the instance using a generic callback
        /// </summary>
        /// <param name="callback"></param>
        /// <typeparam name="TDocument"></typeparam>
        /// <typeparam name="TDataTranslationObject"></typeparam>
        /// <returns></returns>
        public CollectionBuilder<TContext> WithEntity<TDocument, TDataTranslationObject>(DelegateWithEntity<TDocument, TDataTranslationObject> callback) where TDocument : CollectionModel, new() where TDataTranslationObject: DataTranslationObject<TDocument>, new()
        {
            // Instantiate our entity
            Entity<TContext, TDocument, TDataTranslationObject> entity = new Entity<TContext, TDocument, TDataTranslationObject>(Context);
            // Set the database into the entity
            entity.WithDatabase(Database);
            // Execute the callback after setting the database and context into the entity
            callback.Invoke(entity);
            // We're done, set the entity into the instance
            return WithEntity<TDocument, TDataTranslationObject>(entity);
        }
    }
}
