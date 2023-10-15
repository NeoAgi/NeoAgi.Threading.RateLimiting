using System.Threading.RateLimiting;

namespace NeoAgi.Threading.RateLimiting
{
    public class ThroughputConcurrencyOptions : ConcurrencyOptions
    {
        public TimeSpan ReplinshmentPeriod { get; set; } = TimeSpan.FromSeconds(1);
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
