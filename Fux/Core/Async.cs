using System.Threading.Tasks;

namespace Fux.Core
{
    /// <summary>
    /// This class maintains our async helpers
    /// </summary>
    public static class Async
    {
        /// <summary>
        /// This method synchronously executes a task with a typed return value
        /// </summary>
        /// <param name="task"></param>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public static TResult RunSynchronously<TResult>(Task<TResult> task)
        {
            // Make sure the task hasn't completed with a fault
            if (task.Status.Equals(TaskStatus.Faulted)) throw task.Exception;
            // Make sure the task hasn't completed
            if (!task.Status.Equals(TaskStatus.RanToCompletion))
            {
                // Reconfigure the awaiters on the task
                task.ConfigureAwait(false);
                // Wait for the task to completed
                task.Wait();
            }
            // We're done return the result
            return task.Result;
        }

        /// <summary>
        /// This method synchronously executes a task with a dynamic return value
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public static dynamic RunSynchronously(Task<dynamic> task) =>
            RunSynchronously<dynamic>(task);

        /// <summary>
        /// This method synchronously executes a task with no return value
        /// </summary>
        /// <param name="task"></param>
        public static void RunSynchronously(Task task)
        {
            // Make sure the task hasn't completed with a fault
            if (task.Status.Equals(TaskStatus.Faulted)) throw task.Exception;
            // Make sure the task hasn't completed
            if (!task.Status.Equals(TaskStatus.RanToCompletion))
            {
                // Reconfigure the awaiters on the task
                task.ConfigureAwait(false);
                // Wait for the task to completed
                task.Wait();
            }
        }
    }
}
