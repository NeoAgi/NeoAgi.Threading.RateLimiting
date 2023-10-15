namespace NeoAgi.Threading.RateLimiting.Test;

using System.Collections.Concurrent;
using NeoAgi.Threading.RateLimiting;

public class Tests
{
    protected static Random random = new Random();

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

        await queue.EnumerateWithRateLimitAsync(async (str) =>
        {
            await PrintStringWithRandomDelay(str);
        });

        Assert.Pass();
    }

    protected async Task PrintStringWithRandomDelay(string str)
    {
        int delayAmount = random.Next(200, 3000);
        Console.WriteLine($"Received {str} from delegate.  Waited {delayAmount}ms.");
    }
}
