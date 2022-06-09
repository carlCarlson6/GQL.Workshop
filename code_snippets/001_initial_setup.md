# initial setup

packages
```csharp
HotChocolate.AspNetCore 12.10.0
GraphQL.Server.Ui.Voyager 6.1.0
GraphQL.Server.Ui.Playground 6.1.0
```

Program.cs
```csharp
using GQL.Workshop.App;

Host.CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(webBuilder => 
        webBuilder.UseStartup<Startup>())
    .Build()
    .Run();
```

Statup.cs
```csharp
using GQL.Workshop.App.Data;

namespace GQL.Workshop.App;

public class Startup
{    
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _environment;

    public Startup(IConfiguration configuration, IWebHostEnvironment environment) => (_configuration, _environment) = (configuration, environment);

    public void ConfigureServices(IServiceCollection services) 
    {
        services
            .AddSingleton(new BooksDB())
            .AddSingleton(new AuthorsDB());

        services
            .AddGraphQLServer()
            .AddQueryType<Queries>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env) => app
        .UseRouting()
        .UseEndpoints(endpoints =>
        {
            endpoints.MapGraphQL();
            endpoints.MapGraphQLVoyager();
            endpoints.MapGraphQLPlayground();
        });
}
```

Data/Book.cs
```csharp
namespace GQL.Workshop.App.Data;

public record Book(Guid Id, string Title, DateTime Published, Guid AuthorId);

public class BooksDb
{
    private static IEnumerable<Book> _books = new List<Book> 
    {
        new(new Guid("0dacb47e-2e37-459b-bc25-b9ab58bf89ee"), "Is graphql better than rest?", DateTime.UtcNow, new Guid("4f8df196-f699-44fe-9351-4b1d2ca0f0a5")),
        new(new Guid("951c4b62-d33e-4687-bff4-397816c0860d"), "The succss of Colba", DateTime.UtcNow, new Guid("779316ff-1845-4d83-89c9-68720d486bac")),
    };

    public void Add(Book book) => _books = _books.Append(book);
    
    public IEnumerable<Book> Get() => _books;
    
    public Book? Get(Guid id) => _books.FirstOrDefault(book => book.Id == id);
    public IEnumerable<Book> Query(Func<Book, bool> query) => _books.Where(query);

    public void Update(Book book) 
    {
        var books = _books.Where(b => b.Id != book.Id);
        _books = books.Append(book);
    }
    
    public void Delete(Guid id) => _books = _books.Where(book => book.Id != id);
}
```

Data/Author.cs
```csharp
namespace GQL.Workshop.App.Data;

public record Author(Guid Id, string Name);

public class AuthorsDb
{
    private static IEnumerable<Author> _authors = new List<Author>
    {
        new(new Guid("4f8df196-f699-44fe-9351-4b1d2ca0f0a5"), "Carl Carlson III"),
        new (new Guid("779316ff-1845-4d83-89c9-68720d486bac"), "Juanjo & Danny")
    };

    public void Add(Author author) => _authors = _authors.Append(author);

    public IEnumerable<Author> Get() => _authors;
    public IEnumerable<Author> Query(Func<Author, bool> query) => _authors.Where(query);
    
    public Author? Get(Guid id) => _authors.FirstOrDefault(book => book.Id == id);
    
    public void Update(Author author) 
    {
        var authors = _authors.Where(a => a.Id != author.Id);
        _authors = authors.Append(author);
    }
    
    public void Delete(Guid id) => _authors = _authors.Where(a => a.Id == id);
}
```

Queries/Queries.cs
```csharp
using GQL.Workshop.App.Data;

namespace GQL.Workshop.App.Queries;

public class Queries
{
    private readonly BooksDb _booksDb;
    private readonly AuthorsDb _authorsDb;

    public Queries(BooksDb booksDb, AuthorsDb authorsDb)
    {
        _booksDb = booksDb;
        _authorsDb = authorsDb;
    }

    public Task<IEnumerable<Book>> GetBooks()
    {
        var books = _booksDb.Get();
        return Task.FromResult(books);
    }

    public Task<IEnumerable<Author>> GetAuthors() => Task.FromResult(_authorsDb.Get());
}
```