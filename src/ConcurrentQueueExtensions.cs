using System.Collections.Concurrent;
using System.Threading.RateLimiting;

namespace NeoAgi.Threading.RateLimiting
{
    public static class ConcurrentQueueExtensions
    {
        private static Random random = new Random();

        public static async Task EnumerateWithTPS<T>(this ConcurrentQueue<T> queue, int tps, int concurrency, Func<T, Task> dequeueFunc)
        {
            await queue.EnumerateWithRateLimitAsync<T>(new ThroughputConcurrencyOptions()
            {
                ConcurrencyLimit = concurrency,
                ReplinshmentPeriod = TimeSpan.FromMilliseconds(250),
                TasksPerPeriod = tps / 4
            }, dequeueFunc);
        }

        public static async Task EnumerateWithRateLimitAsync<T>(this ConcurrentQueue<T> queue, Func<T, Task> dequeueFunc)
        {
            await queue.EnumerateWithRateLimitAsync(new ThroughputConcurrencyOptions(), dequeueFunc);
        }

        public static async Task EnumerateWithRateLimitAsync<T>(this ConcurrentQueue<T> queue, ThroughputConcurrencyOptions options, Func<T, Task> dequeueFunc)
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
                }
            }
        }
    }
}
