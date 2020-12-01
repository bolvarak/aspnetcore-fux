using System.Linq;
using MongoDB.Driver;

namespace Fux.Data.Mongo.Abstract
{
    /// <summary>
    /// This class maintains the structure for a MongoDB database context
    /// </summary>
    public abstract class DbContext<TContext> where TContext: DbContext<TContext>, new()
    {
        /// <summary>
        /// This property contains our MongoDB client connection
        /// </summary>
        public  MongoClient Client { get; protected set; }

        /// <summary>
        /// This property contains the collection builder instance
        /// </summary>
        public CollectionBuilder<TContext> CollectionBuilder { get; protected set; }

        /// <summary>
        /// This property contains the MongoDB database interface for the context
        /// </summary>
        public IMongoDatabase Database { get; protected set; }

        /// <summary>
        /// This property contains our client builder
        /// </summary>
        public readonly ClientBuilder ClientBuilder;

        /// <summary>
        /// This method instantiates our database context class with a ClientBuilder instance
        /// </summary>
        public DbContext()
        {
            // Set the client builder interface into the instance
            ClientBuilder = new ClientBuilder();
            // Configure the context
            Configure();
        }

        /// <summary>
        /// This method instantiates our database context class with an
        /// existing ClientBuilder interface instance
        /// </summary>
        /// <param name="clientBuilder"></param>
        public DbContext(ClientBuilder clientBuilder)
        {
            // Set the client builder interface into the instance
            ClientBuilder = clientBuilder;
            // Configure the context
            Configure();
        }

        /// <summary>
        /// This method instantiates our database context with a database
        /// and new ClientBuilder interface instance
        /// </summary>
        /// <param name="database"></param>
        public DbContext(string database)
        {
            // Set the client builder into the instance
            ClientBuilder = new ClientBuilder();
            // Configure the context
            Configure(database);
        }

        /// <summary>
        /// This method instantiates our database context with a database
        /// name and existing ClientBuilder interface instance
        /// </summary>
        /// <param name="database"></param>
        /// <param name="clientBuilder"></param>
        public DbContext(string database, ClientBuilder clientBuilder)
        {
            // Set the client builder into the instance
            ClientBuilder = clientBuilder;
            // Configure the context
            Configure(database);
        }

        /// <summary>
        /// This method configures the context for connections
        /// </summary>
        /// <param name="database"></param>
        /// <returns></returns>
        protected void Configure(string database = null)
        {
            // Execute the configuration callback
            OnConfiguring(ClientBuilder);
            // Set our client connection into the instance
            Client = ClientBuilder.ToMongoClient();
            // Tell the collection builder to use this instance of the context
            // Check for a database name
            if (!string.IsNullOrEmpty(database) && !string.IsNullOrWhiteSpace(database))
            {
                // Load the database into the instance
                Use(database);
            }
        }

        /// <summary>
        /// This method provides a hook for intercepting the client
        /// builder and making custom changes
        /// </summary>
        /// <param name="clientBuilder"></param>
        protected abstract void OnConfiguring(ClientBuilder clientBuilder);

        /// <summary>
        /// This method configures collection
        /// </summary>
        /// <param name="collectionBuilder"></param>
        protected abstract void OnConfiguringCollections(CollectionBuilder<TContext> collectionBuilder);

        /// <summary>
        /// This method selects a database from the MongoDB server
        /// </summary>
        /// <param name="database"></param>
        /// <returns></returns>
        protected DbContext<TContext> Use(string database)
        {
            // Load the database into the instance
            Database = Client.GetDatabase(database);
            // Set the database into the collection builder
            CollectionBuilder.WithDatabase(Database);
            // Setup the collections in the instance
            SetupCollections();
            // We're done, return the instance
            return this;
        }

        /// <summary>
        /// This method localizes the collection for an entity by its type
        /// </summary>
        /// <typeparam name="TDocument"></typeparam>
        /// <returns></returns>
        public IMongoCollection<TDocument> Collection<TDocument>() where TDocument : CollectionModel, new()
        {
            // Iterate over the entities
            foreach (var entity in CollectionBuilder.ListEntities())
                if (entity.Type == typeof(TDocument))
                    return (entity.GetCollection() as IMongoCollection<TDocument>);
            // We're done, no collection found
            return null;
        }

        /// <summary>
        /// This method returns a collection interface for <paramref name="collection"/>
        /// from the database in the connection
        /// </summary>
        /// <param name="collection"></param>
        /// <typeparam name="TDocument"></typeparam>
        /// <returns></returns>
        public IMongoCollection<TDocument> Collection<TDocument>(string collection) =>
            Database.GetCollection<TDocument>(collection);

        /// <summary>
        /// This method returns a dynamic entity from the collection
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public Entity<TContext, CollectionModel, DataTranslationObject<CollectionModel>> Entity(System.Type type) =>
            CollectionBuilder.Collection.FirstOrDefault(e => e.GetType() == type);

        /// <summary>
        /// This method returns a typed entity from the collection
        /// </summary>
        /// <typeparam name="TDocument"></typeparam>
        /// <typeparam name="TDataTranslationObject"></typeparam>
        /// <returns></returns>
        public Entity<TContext, TDocument, TDataTranslationObject> Entity<TDocument, TDataTranslationObject>()
            where TDocument : CollectionModel, new()
            where TDataTranslationObject : DataTranslationObject<TDocument>, new() =>
            (Entity<TContext, TDocument, TDataTranslationObject>) CollectionBuilder.Collection.FirstOrDefault(e =>
                e.GetType() == typeof(Entity<TContext, TDocument, TDataTranslationObject>));

        /// <summary>
        /// This method returns the MongoDB database interface from the instance
        /// </summary>
        /// <returns></returns>
        public IMongoDatabase GetDatabase() => Database;

        /// <summary>
        /// This method returns a database interface for <paramref name="database"/> the client connection
        /// </summary>
        /// <param name="database"></param>
        /// <returns></returns>
        public IMongoDatabase GetDatabase(string database) =>
            Client.GetDatabase(database);

        /// <summary>
        /// This method sets up the collection properties on the context instance
        /// </summary>
        public void SetupCollections()
        {
            // Check the database
            if (Database != null)
            {
                // Set the context into the builder
                CollectionBuilder = new CollectionBuilder<TContext>(this as TContext, Database);
                // Configure the collections
                OnConfiguringCollections(CollectionBuilder);
            }
        }
    }
}
