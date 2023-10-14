using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.RateLimiting;
using System.Threading.Tasks;

namespace NeoAgi.Threading.RateLimiting
{
    public static class IEnumerableExtensions
    {
        private static Random random = new Random();

        public static async Task EnumerateWithRateLimitAsync<T>(this ConcurrentQueue<T> queue) 
        {
            using (RateLimiter limiter = new TokenBucketRateLimiter(new TokenBucketRateLimiterOptions()
            {
                TokenLimit = 10,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 1,
                ReplenishmentPeriod = TimeSpan.FromSeconds(1),
                TokensPerPeriod = 30,
                AutoReplenishment = true
            }))
            {
                while (queue.Count() > 0)
                {
                    Stopwatch swStart = Stopwatch.StartNew();
                    DateTime lastTick = DateTime.Now;

                    // will complete after ~5 seconds
                    using RateLimitLease lease = await limiter.AcquireAsync();
                    if (lease.IsAcquired)
                    {
                        queue.TryDequeue(out var s);

                        Console.WriteLine($"Acquired Lease on {s}.  Time: {DateTime.Now.ToString("O")}.  Time since last process {(DateTime.Now - lastTick).TotalMilliseconds}");
                        Task.Delay(random.Next(50, 150)).Wait();
                        lastTick = DateTime.Now;
                    }
                    else
                    {
                        Console.WriteLine("Failed to acquire lease.");
                    }
                }
            }
        }
    }
}
