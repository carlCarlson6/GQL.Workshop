Mutations/Mutations.cs
```
using GQL.Workshop.App.Data;

namespace GQL.Workshop.App.Mutations;

public class Mutations
{
    public Task<Book> AddBook([Service] BooksDb db,  AddBookInput input)
    {
        var book = new Book(Guid.NewGuid(), input.Title, DateTime.UtcNow, input.AuthorId);
        
        // more validations ...
        
        db.Add(book);

        return Task.FromResult(book);
    }
}

public record AddBookInput(string Title, Guid AuthorId);
```

Startup.cs
```
public void ConfigureServices(IServiceCollection services) 
{
    ...

    services
        .AddGraphQLServer()
        .AddQueryType<Queries.Queries>()
        .AddMutationType<Mutations.Mutations>();
}
```

Execute
```
dotnet graphql update
```