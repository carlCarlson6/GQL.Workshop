# initial setup

packages
```
HotChocolate.AspNetCore 12.10.0
GraphQL.Server.Ui.Voyager 6.1.0
GraphQL.Server.Ui.Playground 6.1.0
```

Program.cs
```
using GQL.Workshop.App;

Host.CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(webBuilder => 
        webBuilder.UseStartup<Startup>())
    .Build()
    .Run();
```

Statup.cs
```
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
```
namespace GQL.Workshop.App.Data;

public record Book(Guid Id, string Title, DateTime Published, Guid AuthorId);

public class BooksDB
{
    private static IEnumerable<Book> _books = new List<Book> {
        new(new Guid(), "Is graphql better than rest?", DateTime.UtcNow, new Guid()),
        new(new Guid(), "The succss of Colba", DateTime.UtcNow, new Guid()),
    };

    public void Add(Book book) => _books = _books.Append(book);
    
    public IEnumerable<Book> Get() => _books;
    
    public Book? Get(Guid id) => _books.FirstOrDefault(book => book.Id == id);
    
    public void Update(Book book) 
    {
        var books = _books.Where(b => b.Id != book.Id);
        _books = books.Append(book);
    }
    
    public void Delete(Guid id) => _books = _books.Where(book => book.Id != id);
}
```

Data/Author.cs
```
namespace GQL.Workshop.App.Data;

public record Author(Guid Id, string Name);

public class AuthorsDB
{
    private static IEnumerable<Author> _authors = new List<Author>();

    public void Add(Author author) => _authors = _authors.Append(author);

    public IEnumerable<Author> Get() => _authors;
    
    public Author? Get(Guid id) => _authors.FirstOrDefault(book => book.Id == id);
    
    public void Update(Author author) 
    {
        var authors = _authors.Where(a => a.Id != author.Id);
        _authors = authors.Append(author);
    }
    
    public void Delete(Guid id) => _authors = _authors.Where(a => a.Id == id);
}
```