namespace NeoAgi.Threading.RateLimiting
{
    /// <summary>
    /// Base Options class for all Rate Limiting Routines
    /// </summary>
    public class ConcurrencyOptions
    {
        /// <summary>
        /// Sets the maximum number of requests that can access a resource at once.  Defaults to 5.
        /// </summary>
        public int ConcurrencyLimit { get; set; } = 5;
    }
}
