# Razensoft.Faktory

.NET implementation of [Faktory](https://github.com/contribsys/faktory) worker and client

## Installation

Nuget package TBD. For now you need to compile it yourself.

## Usage

Use FaktoryClient for job creation

```csharp
using Razensoft.Faktory;
...
var client = new FaktoryClient
{
    Password = "pass"
};
await client.ConnectAsync("127.0.0.1");
await client.PublishAsync(new Job
{
    Type = "TestJob",
    Id = Guid.NewGuid().ToString(),
    Args = new object[] { 1, 2, "Hello"}
});
```

And FaktoryWorker for job consumption

```csharp
using Razensoft.Faktory;
...
var worker = new FaktoryWorker
{
    Password = "pass",
    SubscribedQueues = { "default" }
};
worker.Register("TestJob", TestJob);
await worker.ConnectAsync("127.0.0.1");
await worker.RunAsync();
...
private static void TestJob(object[] args)
{
    throw new NotImplementedException();
}
```
