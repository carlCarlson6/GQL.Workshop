```csharp
namespace GQL.Workshop.App;

public abstract class AppError : Exception
{
    public string ErrorCode { get; }

    protected AppError(string message, string errorCode) : base(message) => ErrorCode = errorCode;
}
```

```csharp
namespace GQL.Workshop.App.Filters;

public class AppErrorFilter : IErrorFilter
{
    public IError OnError(IError error) => error.Exception switch
    {
        null => error,
        AppError appError => error.WithMessage(appError.Message).WithCode(appError.ErrorCode),
        _ => error.WithMessage(error.Exception.Message)
    };
}
```

on Startup.cs
```csharp
public void ConfigureServices(IServiceCollection services) 
{
    ....

    services
        .AddGraphQLServer()
        .AddQueryType(d => d.Name(AppObjectTypes.Query))
            .AddTypeExtension<BookQueries>()
            .AddTypeExtension<BookExtensions>()
            .AddTypeExtension<AuthorQueries>()
            .AddTypeExtension<AuthorExtensions>()
        .AddMutationType(d => d.Name(AppObjectTypes.Mutation))
            .AddTypeExtension<BookMutations>()
        .AddErrorFilter(_ => new AppErrorFilter());
}
```

---

```csharp
using GQL.Workshop.App.Data;

namespace GQL.Workshop.App.Mutations;

[ExtendObjectType(AppObjectTypes.Mutation)]
public class BookMutations
{
    [Error(typeof(AuthorNotFound))]
    [Error(typeof(DuplicatedBook))]
    //[UseMutationConvention]
    public Task<Book> AddBook([Service] BooksDb db, [Service] AuthorsDb authorsDb,  AddBookInput input)
    {
        if (authorsDb.Get(input.AuthorId) is null)
            throw new AuthorNotFound(input.AuthorId);

        if (db.Query(book => book.Title == input.Title).Any())
            throw new DuplicatedBook(input.Title);
        
        var book = new Book(Guid.NewGuid(), input.Title, DateTime.UtcNow, input.AuthorId);

        db.Add(book);

        return Task.FromResult(book);
    }
}

public class DuplicatedBook : AppError
{
    public DuplicatedBook(string title) : base(
        $"book with title {title} already in the DB", 
        "DUPLICATED_BOOK") { }
}

public class AuthorNotFound : AppError
{
    public AuthorNotFound(Guid author) : base(
        $"cannot find author with id {author}", 
        "AUTHOR_NOT_FOUND") { }
}

public record AddBookInput(string Title, Guid AuthorId);
```

on Startup.cs
```csharp
public void ConfigureServices(IServiceCollection services) 
{
    ....

    services
        .AddGraphQLServer()
        .AddMutationConventions(applyToAllMutations: true)
        .AddQueryType(d => d.Name(AppObjectTypes.Query))
            .AddTypeExtension<BookQueries>()
            .AddTypeExtension<BookExtensions>()
            .AddTypeExtension<AuthorQueries>()
            .AddTypeExtension<AuthorExtensions>()
        .AddMutationType(d => d.Name(AppObjectTypes.Mutation))
            .AddTypeExtension<BookMutations>()
        .AddErrorFilter(_ => new AppErrorFilter());
}
```