```csharp
public record Magazine(
    Guid Id, 
    string Title, 
    DateTime Published, 
    [property: GraphQLIgnore]
    IEnumerable<Guid> AuthorIds) : IPublication;
```

```csharp
using GQL.Workshop.App.Data;

namespace GQL.Workshop.App.Queries;

[ExtendObjectType(typeof(Magazine))]
public class MagazineExtensions
{
    private readonly AuthorsDb _authorsDb;

    public MagazineExtensions(AuthorsDb authorsDb) => _authorsDb = authorsDb;

    public Task<IEnumerable<Author>> GetAuthors([Parent] Magazine magazine)
    {
        var authors = _authorsDb.Query(author => magazine.AuthorIds.Contains(author.Id));
        if (!authors.Any())
            throw new Exception($"authors not for given magazine: {magazine}");

        return Task.FromResult(authors);
    }
}
```

```csharp
using GQL.Workshop.App.Data;

namespace GQL.Workshop.App.DataLoaders;

public class AuthorBatchDataLoader : BatchDataLoader<Guid, Author>
{
    private readonly AuthorsDb _authorsDb;

    public AuthorBatchDataLoader(IBatchScheduler batchScheduler, AuthorsDb authorsDb, DataLoaderOptions? options = null)
        : base(batchScheduler, options) => _authorsDb = authorsDb;

    protected override Task<IReadOnlyDictionary<Guid, Author>> LoadBatchAsync(IReadOnlyList<Guid> authorIds, CancellationToken cancellationToken)
    {
        var authors = _authorsDb
            .Query(author => authorIds.Contains(author.Id))
            .ToDictionary(a => a.Id);
        return Task.FromResult((IReadOnlyDictionary<Guid, Author>)authors);
    }
}
```

```csharp
using GQL.Workshop.App.Data;

namespace GQL.Workshop.App.DataLoaders;

public class BooksByAuthorGroupDataLoader : GroupedDataLoader<Guid, Book>
{
    private readonly BooksDb _booksDb;
    
    public BooksByAuthorGroupDataLoader(IBatchScheduler batchScheduler, BooksDb booksDb, DataLoaderOptions? options = null) 
        : base(batchScheduler, options) => _booksDb = booksDb;
    
    protected override Task<ILookup<Guid, Book>> LoadGroupedBatchAsync(IReadOnlyList<Guid> authorIds, CancellationToken cancellationToken)
    {
        var books = _booksDb.Query(book => authorIds.Contains(book.AuthorId));
        return Task.FromResult(books.ToLookup(b => b.AuthorId));
    }
}
```

```csharp
using GQL.Workshop.App.Data;
using GQL.Workshop.App.DataLoaders;

namespace GQL.Workshop.App.Queries;

[ExtendObjectType(typeof(Book))]
public class BookExtensions
{
    private readonly AuthorBatchDataLoader _authorBatchDataLoader;

    public BookExtensions(AuthorBatchDataLoader authorBatchDataLoader) => _authorBatchDataLoader = authorBatchDataLoader;

    public Task<Author> GetAuthor([Parent] Book book) => _authorBatchDataLoader.LoadAsync(book.AuthorId);
}
```

```csharp
using GQL.Workshop.App.Data;
using GQL.Workshop.App.DataLoaders;

namespace GQL.Workshop.App.Queries;

[ExtendObjectType(typeof(Author))]
public class AuthorExtensions
{
    private readonly BooksByAuthorGroupDataLoader _dataLoader;

    public AuthorExtensions(BooksByAuthorGroupDataLoader dataLoader) => _dataLoader = dataLoader;

    public async Task<IEnumerable<Book>> GetBooks([Parent] Author author) => await _dataLoader.LoadAsync(author.Id);
}
```

on Startup.cs
```csharp
    public void ConfigureServices(IServiceCollection services) 
    {
        services
            .AddSingleton(new BooksDb())
            .AddSingleton(new AuthorsDb())
            .AddSingleton(new MagazinesDb())
            .AddInMemorySubscriptions();

        services
            .AddGraphQLServer()
            .AddDataLoader<AuthorBatchDataLoader>()
            .AddDataLoader<BooksByAuthorGroupDataLoader>()
            .AddMutationConventions(applyToAllMutations: true)
            .AddType<Magazine>()
            .AddType<Book>()
            .AddQueryType(d => d.Name(AppObjectTypes.Query))
                .AddTypeExtension<BookQueries>()
                .AddTypeExtension<BookExtensions>()
                .AddTypeExtension<AuthorQueries>()
                .AddTypeExtension<AuthorExtensions>()
                .AddTypeExtension<PublicationQueries>()
                .AddTypeExtension<MagazineExtensions>()
            .AddMutationType(d => d.Name(AppObjectTypes.Mutation))
                .AddTypeExtension<BookMutations>()
            .AddTypeExtension<PublicationMutation>()
            .AddSubscriptionType(d => d.Name(AppObjectTypes.Subscription))
                .AddTypeExtension<BookSubscriptions>()
            .AddErrorFilter(_ => new AppErrorFilter())
            .ModifyOptions(o => o.EnableOneOf = true);
    }
```