using System;
using System.Threading.Tasks;

namespace Fux.Core
{
    /// <summary>
    /// This class provides a form of forking, of sorts
    /// </summary>
    public static class Fork
    {
        /// <summary>
        /// This method handles a task that was forked
        /// </summary>
        /// <param name="task"></param>
        private static void whenDone(Task task)
        {
            // Define our prefix
            string prefix = $"Task [{task.Id.ToString()}]";
            // Check the cancelled flag
            if (task.IsCanceled) Console.WriteLine($"{prefix} Is Cancelled");
            // Check the completed successfully flag
            if (task.IsCompletedSuccessfully) Console.WriteLine($"{prefix} Completed Successfully");
            // Check the faulted flag
            if (task.IsFaulted) Console.WriteLine($"{prefix} Has Error:  {task.Exception?.Message}");
            // Check the completed flag
            if (task.IsCompleted) Console.WriteLine($"{prefix} Completed");
            // Dispose of the task
            task.Dispose();
        }

        /// <summary>
        /// This method provides our default callback for generics
        /// </summary>
        /// <param name="task"></param>
        /// <typeparam name="T"></typeparam>
        private static void whenDone<T>(Task<T> task) =>
            whenDone(task);

        /// <summary>
        /// This method forks the asynchronous task with a callback to be executed whenever
        /// </summary>
        /// <param name="task"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static Task This(Task task, Action<Task> callback = null) =>
            task.ContinueWith(callback ?? whenDone);

        /// <summary>
        /// This method forks the asynchronous task with a callback to be executed whenever
        /// </summary>
        /// <param name="task"></param>
        /// <param name="callback"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Task<T> This<T>(Task<T> task, Action<Task<T>> callback = null) =>
            (Task<T>) task.ContinueWith(callback ?? whenDone<T>);
    }
}
