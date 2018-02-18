# LINQPadHelpers
Niger libraries to remove some of my repetitive code from LINQPad scripts. 

- [LINQPadHelpers.Core](https://www.nuget.org/packages/LINQPadHelpers.Core/)
- [LINQPadHelpers.Json](https://www.nuget.org/packages/LINQPadHelpers.Json/)
- [LINQPadHelpers.AzureRm](https://www.nuget.org/packages/LINQPadHelpers.AzureRm/)




LINQPadHelpers.Core contains extension methods and utilities that take no external dependencies.  The ones I use most often are Task.Then(Task|value) and IEnumerable<Task>.WhenAll

Instead of this:

    var result = await SomeMethodAsync();
    var result2 = await Task.WhenAll(result.Select(SomethingAsync));
    var someProperties = result2.Select(x => x.Property);
    
Do this:

    var someProperty = await SomeMethodAsync()
        .Then(x => x.Select(SomethingAsync).WhenAll())
        .Then(result => result.Select(x => x.Property));