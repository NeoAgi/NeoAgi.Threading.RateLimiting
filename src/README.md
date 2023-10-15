# Overview

Simple Abstractions over the [System.Threading.RateLimiting](https://learn.microsoft.com/en-us/dotnet/api/system.threading.ratelimiting) namespace with the goal of simplifying the API for callers.

This library does not intend to replace the System.Threading.RateLimiting Namespace.  Simply make working with some aspects of it easier under some contexts.  

# Extension Methods

## ConcurrentQueue<T>

Processing a ConcurrentQueue will continue until the cancellation token is cancelled allowing for work to be added to the queue at any time and worked.

### ProcessWithTPSAsync()

This abtraction calls `TokenBucketRateLimiter` with options designed to keep the calls to this function below a set Transactions Per Second by using assumptions.

This call does not attmpet to keep the throughput at the set TPS, but will ensure it will not go over that set amount.  In testing TPS can only be reached with a high concurrency value.

Concurrency does not directly imply the number of threas as that depends on how the work is performed within the callback.  General guidance is to set concurrency proprotational to the complexity of the call, lower values for callbacks that complete quickly, larger for callbacks that require more time. 

Example Call:

```csharp
var stringQueue = new ConcurrentQueue<string>();
// Fill the queue
await stringQueue.ProcessWithTPSAsync(maxTransactionsPerSecond, concurrentResourceCount, async (action) =>
{
    // Perform work once TPS falls below the threshold
});
```

Note: This extension limits processing of the queue to a single thread, fanning out processing to sub-threads.

### ProcessWithRateLimitAsync()

This abstraction resides just above the `TokenBucketRateLimiter` exposing most of the core API with added clarity to the options.  

If processing of the queue is intended to be handled by multiple threads or further tuning of the delegation to worker threads, this extension is more appropriate to use.

Example Call:

```csharp
var stringQueue = new ConcurrentQueue<string>();
// Fill the queue
await stringQueue.ProcessWithRateLimitAsync(new ThroughputConcurrencyOptions()
{
    ConcurrencyLimit = 20,
    ReplinshmentPeriod = TimeSpan.FromSeconds(10),
    TasksPerPeriod = 50
}, async (value) =>
{
    // Do work
});
```
