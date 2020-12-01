using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fux.Core;
using Fux.Data.Mongo.Abstract;
using Fux.Data.Mongo.Abstract.Implementation;

namespace Fux.Data.Mongo
{
    /// <summary>
    /// This class maintains our MongoDB client engine
    /// </summary>
    public class Engine
    {
        /// <summary>
        /// This delegate provides a structure for building
        /// a client with the fluid interface
        /// </summary>
        /// <param name="clientBuilder"></param>
        public delegate void DelegateWithClient(ClientBuilder clientBuilder);

        /// <summary>
        /// This delegate provides a structure for asynchronously
        /// building a client with the fluid interface
        /// </summary>
        /// <param name="clientBuilder"></param>
        public delegate Task DelegateWithClientAsync(ClientBuilder clientBuilder);

        /// <summary>
        /// This property contains our configuration
        /// </summary>
        private List<dynamic> _contexts = new List<dynamic>();

        /// <summary>
        /// This method returns a singleton for the engine
        /// </summary>
        /// <returns></returns>
        public static Engine Instance() =>
            Singleton<Engine>.Instance();

        /// <summary>
        /// This method instantiates our engine
        /// </summary>
        public Engine() { }

        /// <summary>
        /// This method returns a context from the instance
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public dynamic Context(Type type)
        {
            // Iterate over the context, check the type and return the context
            foreach (dynamic context in _contexts) if (context.GetType() == type) return context;
            // We're done, return null
            return null;
        }

        /// <summary>
        /// This method returns a context from the instance using a generic
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <returns></returns>
        public TContext Context<TContext>() where TContext : DbContext<TContext>, new()
        {
            // Iterate over the context, check the type and return the context
            foreach (dynamic context in _contexts) if (context.GetType() == typeof(TContext)) return context;
            // We're done, return null
            return null;
        }

        /// <summary>
        /// This method adds a context to the instance using a generic
        /// after instantiating it
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <returns></returns>
        public Engine WithContext<TContext>() where TContext : DbContext<TContext>, new()
        {
            // Remove any existing context(s)
            _contexts.RemoveAll(c => c.GetType() == typeof(TContext));
            // Add the context to the instance
            _contexts.Add(Reflection.Instance<TContext>());
            // We're done, return the instance
            return this;
        }
    }
}
