# LINQPadHelpers
Libraries to remove repetitive code from LINQPad scripts


LINQPadHelpers.Core contains extension methods and utilities that take no external dependencies.  The one I use most often is Task.Then(Task|value)

Instead of this:

    var result = await SomeMethodAsync();
    var result2 = await SomethingAsync(result);
    var someProperty = result2.Property;
    
Do this:

    var someProperty = await SomeMethodAsync()
        .Then(SomethingAsync) 
        .Then(result => result.Property);
        
When mixed with LINQ methods,this is very handy:

    var userDetails = await client.ListUsersAsync()
      .Then(results => results.Where(x => !x.IsAsmin)
         .Select(client.GetUserDetailAsync)
         .WhenAll()
      );

