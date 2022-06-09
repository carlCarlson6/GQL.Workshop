split queries and mutations in different files
```csharp
public class AppObjectTypes
{
    public const string Query = "Query";
    public const string Mutation = "Mutation";
}
```
```csharp
using GQL.Workshop.App.Data;

namespace GQL.Workshop.App.Queries;

[ExtendObjectType(AppObjectTypes.Query)]
public class AuthorQueries
{
    private readonly AuthorsDb _authorsDb;
    
    public AuthorQueries(AuthorsDb authorsDb) => _authorsDb = authorsDb;
    
    public Task<IEnumerable<Author>> GetAuthors() => Task.FromResult(_authorsDb.Get());
}
```
```csharp
using GQL.Workshop.App.Data;

namespace GQL.Workshop.App.Queries;

[ExtendObjectType(AppObjectTypes.Query)]
public class BookQueries
{
    private readonly BooksDb _booksDb;

    public BookQueries(BooksDb booksDb) => _booksDb = booksDb;

    public Task<IEnumerable<Book>> GetBooks()
    {
        var books = _booksDb.Get();
        return Task.FromResult(books);
    }
}
```
```csharp
using GQL.Workshop.App.Data;

namespace GQL.Workshop.App.Mutations;

[ExtendObjectType(AppObjectTypes.Mutation)]
public class BookMutations
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

on Startup.cs
```csharp
public void ConfigureServices(IServiceCollection services) 
{
    services
        .AddSingleton(new BooksDb())
        .AddSingleton(new AuthorsDb());

    services
        .AddGraphQLServer()
        .AddQueryType(d => d.Name(AppObjectTypes.Query))
            .AddTypeExtension<BookQueries>()
            .AddTypeExtension<AuthorQueries>()
        .AddMutationType(d => d.Name(AppObjectTypes.Mutation))
            .AddTypeExtension<BookMutations>();
}
```

---

```csharp
public record Book(
    Guid Id, 
    string Title, 
    DateTime Published, 
    [property: GraphQLIgnore] 
    Guid AuthorId);
```
```csharp
using GQL.Workshop.App.Data;

namespace GQL.Workshop.App.Queries;

[ExtendObjectType(typeof(Book))]
public class BookExtensions
{
    private readonly AuthorsDb _authorsDb;

    public BookExtensions(AuthorsDb authorsDb) => _authorsDb = authorsDb;

    public Task<Author> GetAuthor([Parent] Book book)
    {
        var author = _authorsDb.Get(book.AuthorId);
        if (author is null)
            throw new Exception($"author not found for given book: {book}");
        
        return Task.FromResult(author);
    }
}
```

```csharp
using GQL.Workshop.App.Data;

namespace GQL.Workshop.App.Queries;

[ExtendObjectType(typeof(Author))]
public class AuthorExtensions
{
    private readonly BooksDb _booksDb;

    public AuthorExtensions(BooksDb booksDb) => _booksDb = booksDb;

    public Task<IEnumerable<Book>> GetBooks([Parent] Author author)
    {
        var books = _booksDb.Query(book => book.AuthorId == author.Id);
        return Task.FromResult(books);
    }
}
```

on Startup.cs
```csharp
public void ConfigureServices(IServiceCollection services) 
{
    ...

    services
        .AddGraphQLServer()
        .AddQueryType(d => d.Name(AppObjectTypes.Query))
            .AddTypeExtension<BookQueries>()
            .AddTypeExtension<BookExtensions>()
            .AddTypeExtension<AuthorQueries>()
            .AddTypeExtension<AuthorExtensions>()
        .AddMutationType(d => d.Name(AppObjectTypes.Mutation))
            .AddTypeExtension<BookMutations>();
}
```