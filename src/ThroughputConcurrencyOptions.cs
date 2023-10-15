using System.Threading.RateLimiting;

namespace NeoAgi.Threading.RateLimiting
{
    /// <summary>
    /// A set of ConcurrencyOptions used to control Throughput type Rate Limiting such as Bucket Tokens
    /// </summary>
    public class ThroughputConcurrencyOptions : ConcurrencyOptions
    {
        /// <summary>
        /// Period of time of when to trigger more resources to be free
        /// </summary>
        public TimeSpan ReplinshmentPeriod { get; set; } = TimeSpan.FromSeconds(1);

        /// <summary>
        /// Number of tasks to release at the RelinquishmentPeriod Interval
        /// </summary>
        public int TasksPerPeriod { get; set; } = 20;

        internal TokenBucketRateLimiterOptions GetTokenBucketOptions()
        {
            return new TokenBucketRateLimiterOptions()
            {
                TokenLimit = ConcurrencyLimit,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 1,
                ReplenishmentPeriod = ReplinshmentPeriod,
                TokensPerPeriod = TasksPerPeriod,
                AutoReplenishment = true
            };
        }
    }
}
