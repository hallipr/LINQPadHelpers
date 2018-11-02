[![Build status](https://hallipr.visualstudio.com/CodeFist/_apis/build/status/LINQPadHelpers?branchName=master)](https://hallipr.visualstudio.com/CodeFist/_build/latest?definitionId=4)

# LINQPadHelpers
Niger libraries to remove some of my repetitive code from LINQPad scripts. 

## [LINQPadHelpers.Core](https://www.nuget.org/packages/LINQPadHelpers.Core/)

LINQPadHelpers.Core contains extension methods and utilities that take no external dependencies.  The one I use most often is Task.Then(Task|value)

Instead of this:

```csharp
var results = await Task.WhenAll((await SomeMethodAsync()).Select(SomeFollowupAsync));
```

or

```csharp
var temp = await SomeMethodAsync();
var tempTasks = temp.Select(SomeFollowupAsync);
var results = await Task.WhenAll(tempTasks);
```

Do this:

```csharp
var results = await SomeMethodAsync()
    .Then(x => x.Select(SomeFollowupAsync));
```

Task.Then will automatically await any returned task or IEnumerable<Task>

All of the standard IEnunmerable<T> extension methods have been made Async. This is code gen'ed from .NET's Enumerable class with simple pass through parameters:

```csharp
public static async Task<IEnumerable<TResult>> SelectAsync<TSource, TResult>(
    this Task<IEnumerable<TSource>> sourceTask, 
    Func<TSource, TResult> selector)
{
    return await sourceTask.Then(source => source.Select(selector));
}
```

## [LINQPadHelpers.AzureRm](https://www.nuget.org/packages/LINQPadHelpers.AzureRm/)

LINQPadHelpers.AzureRm simplifies the use of the Azure management NuGet libraries.  This work currently focuses on simplifying authentication:

```csharp

    var credentials = new UserCredentials(
        "SuperTenant.onmicrosoft.com",
        AzureEnvironment.AzureGlobalCloud,
        PromptBehavior.Auto);

    var azure = await credentials.BuildAzureClientAsync("Subscription-1");

```

Azure SDK for .NET's UserTokenProvider uses Uri.ToString on the provided audience. This adds a slash to the end that KeyVault rejects.  To get around this i use simple token UserTokenProvider that uses Uri.OriginalString instead.  This means that the the same credentials can be used for all of the Azure rest clients:

```csharp
var credentials = new UserCredentials(tenant, AzureEnvironment.AzureGlobalCloud, PromptBehavior.Auto);
var azure = await credentials.BuildAzureClientAsync(subscriptionName);
var sfClient = new ServiceFabricManagementClient(credentials) { SubscriptionId = azure.SubscriptionId };
```
