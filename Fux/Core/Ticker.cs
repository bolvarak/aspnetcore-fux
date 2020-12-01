using System;
using System.Diagnostics;
using Newtonsoft.Json;

namespace Fux.Core
{

    /// <summary>
    /// This structure maintains the time table for a task
    /// </summary>
    public struct TickerTimeTable
    {
        /// <summary>
        /// This property contains the task has been running
        /// </summary>
        [JsonProperty("elapsed")]
        public TimeSpan Elapsed { get; set; }

        /// <summary>
        /// This property contains the iteration the ticker is on [if there is one]
        /// </summary>
        [JsonProperty("iteration")]
        public int? Iteration { get; set; }

        /// <summary>
        /// This property contains the total number of iterations
        /// </summary>
        [JsonProperty("iterationTotal")]
        public int? IterationTotal { get; set; }

        /// <summary>
        /// This property contains the estimated time remaining on the task
        /// </summary>
        [JsonProperty("remaining")]
        public TimeSpan Remaining { get; set; }

        /// <summary>
        /// This property contains the total estimated time the process will take
        /// </summary>
        [JsonProperty("total")]
        public TimeSpan Total { get; set; }
    }

    /// <summary>
    /// This class maintains the structure for a timer
    /// /// </summary>
    public class Ticker
    {
        /// <summary>
        /// This property contains the internal stopwatch of the ticker
        /// </summary>
        private readonly Stopwatch _stopwatch = new Stopwatch();

        /// <summary>
        /// This method stops the internal stopwatch then generates and returns a response
        /// </summary>
        /// <returns></returns>
        public TickerTimeTable Halt()
        {
            // Define our structure
            TickerTimeTable response = new TickerTimeTable();
            // Stop the timer
            _stopwatch.Stop();
            // Set the elapsed time into the response
            response.Elapsed = _stopwatch.Elapsed;
            // We're done, send the response
            return response;
        }

        /// <summary>
        /// This method stops the internal stopwatch then generates and returns a response
        /// </summary>
        /// <param name="iteration"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public TickerTimeTable Halt(int iteration, int total)
        {
            // Define our structure
            TickerTimeTable response = new TickerTimeTable();
            // Stop the timer
            _stopwatch.Stop();
            // Set the elapsed time into the response
            response.Elapsed = _stopwatch.Elapsed;
            // Set the iteration into the response
            response.Iteration = iteration;
            // Set the iteration count into the response
            response.IterationTotal = total;
            // Set the time remaining into the response
            response.Remaining =
                TimeSpan.FromMilliseconds((_stopwatch.ElapsedMilliseconds / iteration) * (total - iteration));
            // Set the total into the response
            response.Total = response.Elapsed.Add(response.Remaining);
            // We're done, send the response
            return response;
        }

        /// <summary>
        /// This method stops the internal stopwatch, generates a response, then resets and starts the internal stopwatch
        /// </summary>
        /// <returns></returns>
        public TickerTimeTable Reset()
        {
            // Define our structure
            TickerTimeTable response = new TickerTimeTable();
            // Stop the timer
            _stopwatch.Stop();
            // Set the elapsed time into the response
            response.Elapsed = _stopwatch.Elapsed;
            // Start the timer
            _stopwatch.Start();
            // We're done, send the response
            return response;
        }

        /// <summary>
        /// This method stops the internal stopwatch, generates a response, then resets and starts the internal stopwatch
        /// </summary>
        /// <param name="iteration"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public TickerTimeTable Reset(int iteration, int total)
        {
            // Define our structure
            TickerTimeTable response = new TickerTimeTable();
            // Stop the timer
            _stopwatch.Stop();
            // Set the elapsed time into the response
            response.Elapsed = _stopwatch.Elapsed;
            // Set the iteration into the response
            response.Iteration = iteration;
            // Set the iteration count into the response
            response.IterationTotal = total;
            // Set the time remaining into the response
            response.Remaining =
                TimeSpan.FromMilliseconds((_stopwatch.ElapsedMilliseconds / iteration) * (total - iteration));
            // Set the total into the response
            response.Total = response.Elapsed.Add(response.Remaining);
            // Reset the timer
            _stopwatch.Reset();
            // Start the timer
            _stopwatch.Start();
            // We're done, send the response
            return response;
        }

        /// <summary>
        /// This method stops the internal stopwatch, generates a response and then starts the internal stopwatch
        /// </summary>
        /// <returns></returns>
        public TickerTimeTable Tick()
        {
            // Define our structure
            TickerTimeTable response = new TickerTimeTable();
            // Stop the timer
            _stopwatch.Stop();
            // Set the elapsed time into the response
            response.Elapsed = _stopwatch.Elapsed;
            // Start the timer
            _stopwatch.Start();
            // We're done, send the response
            return response;
        }

        /// <summary>
        /// This method stops the internal stopwatch, generates a response and then starts the internal stopwatch
        /// </summary>
        /// <param name="iteration"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public TickerTimeTable Tick(int iteration, int total)
        {
            // Define our structure
            TickerTimeTable response = new TickerTimeTable();
            // Stop the timer
            _stopwatch.Stop();
            // Set the elapsed time into the response
            response.Elapsed = _stopwatch.Elapsed;
            // Set the iteration into the response
            response.Iteration = iteration;
            // Set the iteration count into the response
            response.IterationTotal = total;
            // Set the time remaining into the response
            response.Remaining =
                TimeSpan.FromMilliseconds((_stopwatch.ElapsedMilliseconds / iteration) * (total - iteration));
            // Set the total into the response
            response.Total = response.Elapsed.Add(response.Remaining);
            // Start the timer
            _stopwatch.Start();
            // We're done, send the response
            return response;
        }
    }
}
