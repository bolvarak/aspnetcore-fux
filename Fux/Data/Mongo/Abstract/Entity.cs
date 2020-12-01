using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Fux.Core;
using Fux.Data.Mongo.Abstract.Implementation;
using MongoDB.Driver;

namespace Fux.Data.Mongo.Abstract
{
    /// <summary>
    /// This class maintains the structure and utilities for working with MongoDB entities
    /// </summary>
    /// <typeparam name="TDocument"></typeparam>
    /// <typeparam name="TContext"></typeparam>
    /// <typeparam name="TDataTranslationObject"></typeparam>
    public class Entity<TContext, TDocument, TDataTranslationObject> where TContext : DbContext<TContext>, new()
        where TDocument : CollectionModel, new()
        where TDataTranslationObject : DataTranslationObject<TDocument>, new()
    {
        /// <summary>
        /// This property contains the type for the context
        /// </summary>
        public Type ContextType => GetContext()?.GetType();

        /// <summary>
        /// This property contains the type for the collection document
        /// </summary>
        public readonly Type Type = typeof(TDocument);

        /// <summary>
        /// This property contains the MongoDB collection interface
        /// </summary>
        public IMongoCollection<TDocument> Collection { get; protected set; }

        /// <summary>
        /// This property contains the name of the collection
        /// </summary>
        public string CollectionName { get; protected set; }

        /// <summary>
        /// This property contains the context interface
        /// </summary>
        public readonly TContext Context;

        /// <summary>
        /// This property contains the reflection of our context
        /// </summary>
        public readonly Reflection<TContext> ContextReflection = new Reflection<TContext>();

        /// <summary>
        /// This property contains the MongoDB interface
        /// </summary>
        public IMongoDatabase Database { get; protected set; }

        /// <summary>
        /// This property contains the instance of our DTO
        /// </summary>
        public TDataTranslationObject DataTranslationObject { get; protected set; }

        /// <summary>
        /// This property contain the reflection of our DTO
        /// </summary>
        public readonly Reflection<TDataTranslationObject> DataTranslationObjectReflection =
            new Reflection<TDataTranslationObject>();

        /// <summary>
        /// This property contains the model associated with the entity
        /// </summary>
        public TDocument Model { get; protected set; }

        /// <summary>
        /// This property contains the reflection of our model
        /// </summary>
        public readonly Reflection<TDocument> ModelReflection = new Reflection<TDocument>();

        /// <summary>
        /// This method instantiates our class with an instance of the model
        /// <param name="context"></param>
        /// </summary>
        public Entity(TContext context)
        {
            // Set the context into the instance
            Context = context;
            // Set the instance of the DTO into the instance
            DataTranslationObject = DataTranslationObjectReflection.Instance();
            // Set the instance of the model into the instance
            Model = ModelReflection.Instance();
        }

        /// <summary>
        /// This method attaches the MongoDB collection to a property on the context <typeparamref name="TContext"/>
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public Entity<TContext, TDocument, TDataTranslationObject> AttachToContext(
            Expression<Func<TContext, IMongoCollection<TDocument>>> expression)
        {
            // Set the property value
            ContextReflection.Set<IMongoCollection<TDocument>>(Context, expression, GetCollection());
            // We're done, return the instance
            return this;
        }

        /// <summary>
        /// This method fluidly sets the collection into the instance from the collection's name
        /// </summary>
        /// <param name="collectionName"></param>
        /// <returns></returns>
        public Entity<TContext, TDocument, TDataTranslationObject> FromCollection(string collectionName)
        {
            // Reset the collection name into the model
            Model.FromCollection(collectionName);
            // Set the collection name into the instance
            CollectionName = Model.CollectionName();
            // We're done, return the instance
            return ToMongoCollection();
        }

        /// <summary>
        /// This method fluidly sets the MongoDB collection interface into the instance
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public Entity<TContext, TDocument, TDataTranslationObject> FromCollection(
            IMongoCollection<TDocument> collection)
        {
            // Set the collection into the instance
            Collection = collection;
            // Set the collection into the instance
            CollectionName = Model.CollectionName();
            // We're done, return the instance
            return this;
        }

        /// <summary>
        /// This method returns the loose-typed context from the instance
        /// </summary>
        /// <returns></returns>
        public TContext GetContext() =>
            Context;

        /// <summary>
        /// This method returns the MongoDB collection interface from the instance
        /// </summary>
        /// <returns></returns>
        public IMongoCollection<TDocument> GetCollection() =>
            Collection;

        /// <summary>
        /// This method returns the collection name from the instance
        /// </summary>
        /// <returns></returns>
        public string GetCollectionName() =>
            CollectionName;

        /// <summary>
        /// This method returns the MongoDB database interface from the instance
        /// </summary>
        /// <returns></returns>
        public IMongoDatabase GetDatabase() =>
            Database;

        /// <summary>
        /// This method returns the model object from the instance
        /// </summary>
        /// <returns></returns>
        public TDocument GetModel() =>
            Model;

        /// <summary>
        /// This method returns the default service provider for the entity
        /// </summary>
        /// <returns></returns>
        public CollectionServiceProvider<TContext, TDocument, TDataTranslationObject> ServiceProvider() =>
            new CollectionServiceProvider<TContext, TDocument, TDataTranslationObject>(CollectionName);

        /// <summary>
        /// This method sets the collection interface into the context property
        /// </summary>
        /// <returns></returns>
        public Entity<TContext, TDocument, TDataTranslationObject> ToContextInstanceProperty()
        {
            // Iterate over the properties
            foreach (KeyValuePair<string, System.Reflection.PropertyInfo> property in ContextReflection.Properties())
            {
                // Check the property type and skip the iteration if it's not a match
                if (!property.Value.PropertyType.ToString().Equals("IMongoCollection")) continue;
                // Check the type of the generic and set the value into the context
                if (property.Value.PropertyType.GenericTypeArguments.First() == typeof(TDocument))
                {
                    // Set the property value to the collection
                    ContextReflection.Set<IMongoCollection<TDocument>>(Context, property.Value.Name, GetCollection());
                    // We're done with the loop
                    break;
                }
            }

            // We're done, return the instance
            return this;
        }

        /// <summary>
        /// This method converts the model to a MongoDB collection interface
        /// </summary>
        /// <returns></returns>
        public Entity<TContext, TDocument, TDataTranslationObject> ToMongoCollection() =>
            FromCollection(Model.ToMongoCollection<TDocument>(GetDatabase()));

        /// <summary>
        /// This method sets the database into the instance
        /// </summary>
        /// <param name="database"></param>
        /// <returns></returns>
        public Entity<TContext, TDocument, TDataTranslationObject> WithDatabase(IMongoDatabase database)
        {
            // Set the database into the instance
            Database = database;
            // We're done, return the instance
            return this;
        }

        /// <summary>
        /// This method sets the model into the instance
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Entity<TContext, TDocument, TDataTranslationObject> WithModel(TDocument model)
        {
            // Set the model into the instance
            Model = model;
            // We're done, return the instance
            return this;
        }
    }
}
