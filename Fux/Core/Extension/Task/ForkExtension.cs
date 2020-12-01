using System;
using System.Threading.Tasks;

namespace Fux.Core.Extension.Task
{
    /// <summary>
    /// This extension provides forking capabilities for tasks
    /// </summary>
    public static class ForkExtension
    {
        /// <summary>
        /// This method forks a task to finish whenever it finishes
        /// </summary>
        /// <param name="task"></param>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public static Task<TResult> Fork<TResult>(this Task<TResult> task) =>
            Core.Fork.This<TResult>(task, null);

        /// <summary>
        /// This method forks a task with a custom callback to be executed whenever it finishes
        /// </summary>
        /// <param name="task"></param>
        /// <param name="callback"></param>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public static Task<TResult> Fork<TResult>(this Task<TResult> task, Action<Task<TResult>> callback) =>
            Core.Fork.This<TResult>(task, callback);
    }
}
