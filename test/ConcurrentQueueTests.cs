namespace NeoAgi.Threading.RateLimiting.Test;

using System.Collections.Concurrent;
using NeoAgi.Threading.RateLimiting;

public class ConcurrentQueueTests
{
    protected static Random random = new Random();

    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task TestTokenBucketAsync()
    {
        CancellationTokenSource source = new CancellationTokenSource();

        ConcurrentQueue<string> queue = new ConcurrentQueue<string>();
        for (int i = 0; i < 30; i++)
        {
            queue.Enqueue("Iteration: " + i);
        }

        await queue.ProcessWithRateLimitAsync(async (str) =>
        {
            await PrintStringWithRandomDelay(str);
        }, source.Token);

        Assert.Pass();
    }

    [Test]
    public async Task TestTokenBucketContinualRunAsync()
    {
        CancellationTokenSource source = new CancellationTokenSource();
        ConcurrentQueue<string> queue = new ConcurrentQueue<string>();
        for (int i = 0; i < 10; i++)
        {
            queue.Enqueue("Iteration: " + i);
        }

        await Task.Factory.StartNew(() => queue.ProcessWithRateLimitAsync(async (str) =>
        {
            await PrintStringWithRandomDelay(str);
        }, source.Token));

        // Wait 2 seconds
        Task.Delay(2000).Wait();

        for (int i = 11; i < 20; i++)
        {
            queue.Enqueue("Resume Iteration: " + i);
        }

        Task.Delay(2000).Wait();

        Assert.Pass();
    }

    protected async Task PrintStringWithRandomDelay(string str)
    {
        int delayAmount = random.Next(50, 150);
        Console.WriteLine($"Received {str} from delegate.  Waited {delayAmount}ms.");

        await Task.CompletedTask;
    }
}
