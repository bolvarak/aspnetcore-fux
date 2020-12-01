using System.Threading.Tasks;

namespace Fux.Core.Extension.Task
{
    /// <summary>
    /// This class maintains our synchronic enforcers
    /// </summary>
    public static class RunNowExtension
    {
        /// <summary>
        /// This method forces an awaitable return value to execute immediately
        /// </summary>
        /// <param name="source"></param>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public static TResult RunNow<TResult>(this Task<TResult> source) =>
            Async.RunSynchronously<TResult>(source);

        /// <summary>
        /// This method forces an awaitable with no return value to execute immediately
        /// </summary>
        /// <param name="source"></param>
        public static void RunNow(this System.Threading.Tasks.Task source) =>
            Async.RunSynchronously(source);
    }
}
