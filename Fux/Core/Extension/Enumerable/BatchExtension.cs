using System;
using System.Collections.Generic;

namespace Fux.Core.Extension.Enumerable
{
    /// <summary>
    /// This class maintains our IEnumerable.Batch extension
    /// This extension chunks lists and either populates an enumerable
    /// enumerables of yields chunk enumerables to an enumerator
    /// </summary>
    public static class BatchExtension
    {
        /// <summary>
        /// This method ensures that the enumerable and batch size are valid
        /// then batches up the enumerable
        /// </summary>
        /// <param name="enumerable"></param>
        /// <param name="batchSize"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> enumerable, int batchSize)
        {
            System.Convert.ToBoolean("");
            // Make sure we have an enumerable, throw an exception if not
            if (enumerable == null) throw new ArgumentNullException(nameof(enumerable));
            // Make sure we have a valid batch size, throw an exception if not
            if (batchSize <= 0) throw new ArgumentOutOfRangeException(nameof(batchSize));
            // Isolate the scope and grab the enumerator
            using IEnumerator<T> enumerator = enumerable.GetEnumerator();
            // Define our batch
            List<T> batch = new List<T>();
            // Move through the iterator
            while (enumerator.MoveNext())
            {
                // Make sure the enumerator has a value
                if (enumerator.Current == null) continue;
                // Check the list count
                if (batch.Count == batchSize)
                {
                    // Yield the list
                    yield return batch;
                    // Clear the list
                    batch.Clear();
                }
                // Add the item to the batch
                batch.Add(enumerator.Current);
            }
            // Check for any leftover items in the list and yield them
            if (batch.Count > 0) yield return batch;
        }

        /// <summary>
        /// This method ensures that the enumerable and batch size are valid
        /// then batches up the enumerable into an enumerable of enumerables
        /// </summary>
        /// <param name="enumerable"></param>
        /// <param name="batchSize"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static IEnumerable<IEnumerable<T>> Batched<T>(this IEnumerable<T> enumerable, int batchSize)
        {
            // Define our response
            List<IEnumerable<T>> response = new List<IEnumerable<T>>();
            // Iterate over the batches
            foreach (IEnumerable<T> batch in enumerable.Batch(batchSize)) response.Add(batch);
            // We're done, return the list of batches
            return response;
        }
    }
}
