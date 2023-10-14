namespace NeoAgi.Threading.RateLimiting.Test;

using System.Collections.Concurrent;
using NeoAgi.Threading.RateLimiting;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task TestTokenBucketAsync()
    {
        ConcurrentQueue<string> queue = new ConcurrentQueue<string>();
        for (int i = 0; i < 100; i++)
        {
            queue.Enqueue("Iteration: " + i);
        }

        await queue.EnumerateWithRateLimitAsync();

        Assert.Pass();
    }
}
