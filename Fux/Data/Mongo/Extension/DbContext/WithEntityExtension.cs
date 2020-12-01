using Fux.Data.Mongo.Abstract;

namespace Fux.Data.Mongo.Extension.DbContext
{
    /// <summary>
    /// This extension provides the ability to add an entity to a context outside of bootstrapping the context
    /// </summary>
    public static class WithEntityExtension
    {
        /// <summary>
        /// This method adds an entity to the context outside of bootstrapping the context
        /// </summary>
        /// <param name="context"></param>
        /// <param name="entity"></param>
        /// <typeparam name="TContext"></typeparam>
        /// <typeparam name="TDocument"></typeparam>
        /// <typeparam name="TDataTranslationObject"></typeparam>
        /// <returns></returns>
        public static TContext WithEntity<TContext, TDocument, TDataTranslationObject>(this TContext context, Entity<TContext, TDocument, TDataTranslationObject> entity) where TDocument : CollectionModel, new() where TContext : DbContext<TContext>, new() where TDataTranslationObject : DataTranslationObject<TDocument>, new()
        {
            // Add the entity to the collection builder in the context
            context.CollectionBuilder.WithEntity<TDocument, TDataTranslationObject>(entity);
            // Attach the entity to the context
            entity.ToContextInstanceProperty();
            // We're done, return the instance
            return context;
        }

        /// <summary>
        /// This method adds an entity to the context outside of bootstrapping the context
        /// </summary>
        /// <param name="context"></param>
        /// <param name="callback"></param>
        /// <typeparam name="TContext"></typeparam>
        /// <typeparam name="TDocument"></typeparam>
        /// <typeparam name="TDataTranslationObject"></typeparam>
        /// <returns></returns>
        public static TContext WithEntity<TContext, TDocument, TDataTranslationObject>(this TContext context,
            CollectionBuilder<TContext>.DelegateWithEntity<TDocument, TDataTranslationObject> callback)
            where TContext : DbContext<TContext>, new()
            where TDocument : CollectionModel, new()
            where TDataTranslationObject : DataTranslationObject<TDocument>, new()
        {
            // Instantiate our entity with this context associated
            Entity<TContext, TDocument, TDataTranslationObject> entity =
                new Entity<TContext, TDocument, TDataTranslationObject>(context);
            // Set the database into the entity
            entity.WithDatabase(context.Database);
            // Execute the callback
            callback.Invoke(entity);
            // We're done, add the entity to the context and return the instance
            return context.WithEntity(entity);
        }
    }
}
