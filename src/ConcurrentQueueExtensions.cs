using System.Collections.Concurrent;
using System.Threading.RateLimiting;

namespace NeoAgi.Threading.RateLimiting
{
    /// <summary>
    /// Extensions exposing RateLimiting Primitives to run off of a ConcurrentQueue
    /// </summary>
    public static class ConcurrentQueueExtensions
    {
        /// <summary>
        /// Processed the Queue with a strict TPS Limit enforced using a BucketToken Rate Limiter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queue"></param>
        /// <param name="tps">Transactions Per Second Limit</param>
        /// <param name="concurrency">Maximum number of resources to process at once.  Set this higher for longer running tasks to reach TPS set.</param>
        /// <param name="dequeueFunc"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task ProcessWithTPSAsync<T>(this ConcurrentQueue<T> queue, int tps, int concurrency, Func<T, Task> dequeueFunc, CancellationToken cancellationToken)
        {
            await queue.ProcessWithRateLimitAsync<T>(new ThroughputConcurrencyOptions()
            {
                ConcurrencyLimit = concurrency,
                ReplinshmentPeriod = TimeSpan.FromMilliseconds(250),
                TasksPerPeriod = tps / 4
            }, dequeueFunc
            , cancellationToken);
        }

        /// <summary>
        /// Process the queue with the default ThroughputOptions settings using a BucketToken Rate Limiter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queue"></param>
        /// <param name="dequeueFunc"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task ProcessWithRateLimitAsync<T>(this ConcurrentQueue<T> queue, Func<T, Task> dequeueFunc, CancellationToken cancellationToken)
        {
            await queue.ProcessWithRateLimitAsync(new ThroughputConcurrencyOptions(), dequeueFunc, cancellationToken);
        }

        /// <summary>
        /// Process the queue with a customizaeable set of ThroughputOptions using a BucketToken Rate Limiter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queue"></param>
        /// <param name="options"></param>
        /// <param name="dequeueFunc"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task ProcessWithRateLimitAsync<T>(this ConcurrentQueue<T> queue, ThroughputConcurrencyOptions options, Func<T, Task> dequeueFunc, CancellationToken cancellationToken)
        {
            using (RateLimiter limiter = new TokenBucketRateLimiter(options.GetTokenBucketOptions()))
            {
                while (!queue.IsEmpty)
                {
                    using RateLimitLease lease = await limiter.AcquireAsync();
                    if (lease.IsAcquired)
                    {
                        if (queue.TryDequeue(out var s))
                            _ = dequeueFunc.Invoke(s);
                    }

                    if (cancellationToken.IsCancellationRequested)
                        break;
                }
            }
        }
    }
}
