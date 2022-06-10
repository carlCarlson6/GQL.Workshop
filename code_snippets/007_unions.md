```csharp
namespace GQL.Workshop.App.Data;

public abstract class Db<T>
{
    protected static IEnumerable<T> _entities = new List<T>();
    
    public void Add(T entity) => _entities = _entities.Append(entity);
    public IEnumerable<T> Get() => _entities;
    
    public abstract T? Get(Guid id);
    public abstract IEnumerable<T> Query(Func<T, bool> query);
    public abstract void Update(T entity);
    public abstract void Delete(Guid id);
}
```

```csharp
namespace GQL.Workshop.App.Data;

[UnionType("Publication")]
public interface IPublication { }
```
```csharp
public record Book(
    Guid Id, 
    string Title, 
    DateTime Published,
    [property: GraphQLIgnore] 
    Guid AuthorId) : IPublication;
```

```csharp
namespace GQL.Workshop.App.Data;

public record Magazine(Guid Id, string Title, DateTime Published, IEnumerable<Guid> AuthorIds) : IPublication;

public class MagazinesDb : Db<Magazine> 
{
    public override Magazine? Get(Guid id) => throw new NotImplementedException();
    public override IEnumerable<Magazine> Query(Func<Magazine, bool> query) => throw new NotImplementedException();
    public override void Update(Magazine entity) => throw new NotImplementedException();
    public override void Delete(Guid id) => throw new NotImplementedException();
}
```

```csharp
using GQL.Workshop.App.Data;

namespace GQL.Workshop.App.Queries;

[ExtendObjectType(AppObjectTypes.Query)]
public class PublicationQueries
{
    private readonly BooksDb _booksDb;
    private readonly MagazinesDb _magazinesDb;

    public PublicationQueries(BooksDb booksDb, MagazinesDb magazinesDb) =>
        (_booksDb, _magazinesDb) = (booksDb, magazinesDb);
    
    public Task<IPublication?> GetPublication(Guid publicationId) => throw new NotImplementedException();

    public Task<IEnumerable<IPublication>> GetPublications()
    {
        var books = _booksDb.Get();
        var magazines = _magazinesDb.Get();
        var publications = new List<IPublication>()
            .Concat(books)
            .Concat(magazines);
        return Task.FromResult(publications);
    }
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
            .AddMutationConventions(applyToAllMutations: true)
            .AddType<Magazine>()
            .AddType<Book>()
            .AddQueryType(d => d.Name(AppObjectTypes.Query))
                .AddTypeExtension<BookQueries>()
                .AddTypeExtension<BookExtensions>()
                .AddTypeExtension<AuthorQueries>()
                .AddTypeExtension<AuthorExtensions>()
                .AddTypeExtension<PublicationQueries>()
            .AddMutationType(d => d.Name(AppObjectTypes.Mutation))
                .AddTypeExtension<BookMutations>()
            .AddSubscriptionType(d => d.Name(AppObjectTypes.Subscription))
                .AddTypeExtension<BookSubscriptions>()
            .AddErrorFilter(_ => new AppErrorFilter())
            .ModifyOptions(o => o.EnableOneOf = true);
    }

```

---

```csharp
using GQL.Workshop.App.Data;

namespace GQL.Workshop.App.Mutations;

[ExtendObjectType(AppObjectTypes.Mutation)]
public class PublicationMutation
{
    private readonly BooksDb _booksDb;
    private readonly MagazinesDb _magazinesDb;

    public PublicationMutation(BooksDb booksDb, MagazinesDb magazinesDb) =>
        (_booksDb, _magazinesDb) = (booksDb, magazinesDb);

    public Task<IPublication> AddPublication(AddPublicationInput input)
    {
        var publication = input switch
        {
            { BookInput: { } } => new Book(Guid.NewGuid(), input.BookInput.Title, DateTime.UtcNow,
                input.BookInput.AuthorId),
            { MagazineInput: { } } => (IPublication)new Magazine(Guid.NewGuid(), input.MagazineInput.Title,
                DateTime.UtcNow, input.MagazineInput.AuthorIds),
            _ => throw new ArgumentOutOfRangeException(nameof(input), input, null)
        } ?? throw new Exception();
        

        Action savePublication = publication switch
        {
            Book book => () => _booksDb.Add(book),
            Magazine magazine => () => _magazinesDb.Add(magazine),
            _ => throw new ArgumentOutOfRangeException()
        };
        savePublication();

        return Task.FromResult(publication);
    }
}

[OneOf] 
public record AddPublicationInput(AddBookInput? BookInput, AddMagazineInput? MagazineInput);

public record AddMagazineInput(string Title, IEnumerable<Guid> AuthorIds);
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
            .AddMutationConventions(applyToAllMutations: true)
            .AddType<Magazine>()
            .AddType<Book>()
            .AddQueryType(d => d.Name(AppObjectTypes.Query))
                .AddTypeExtension<BookQueries>()
                .AddTypeExtension<BookExtensions>()
                .AddTypeExtension<AuthorQueries>()
                .AddTypeExtension<AuthorExtensions>()
                .AddTypeExtension<PublicationQueries>()
            .AddMutationType(d => d.Name(AppObjectTypes.Mutation))
                .AddTypeExtension<BookMutations>()
            .AddTypeExtension<PublicationMutation>()
            .AddSubscriptionType(d => d.Name(AppObjectTypes.Subscription))
                .AddTypeExtension<BookSubscriptions>()
            .AddErrorFilter(_ => new AppErrorFilter())
            .ModifyOptions(o => o.EnableOneOf = true);
    }
```