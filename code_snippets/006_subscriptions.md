```csharp
using GQL.Workshop.App.Data;

namespace GQL.Workshop.App.Subscriptions;

[ExtendObjectType(AppObjectTypes.Subscription)]
public class BookSubscriptions
{
    [Subscribe]
    //[Topic("ExampleTopic")]
    public Book BookAdded([EventMessage] Book book) => book;
}
```
```csharp
public class BookMutations
{
    private readonly ITopicEventSender _sender;
    private readonly BooksDb _booksDb;
    private readonly AuthorsDb _authorsDb;

    public BookMutations(ITopicEventSender sender, BooksDb booksDb, AuthorsDb authorsDb)
    {
        _sender = sender;
        _booksDb = booksDb;
        _authorsDb = authorsDb;
    }

    [Error(typeof(AuthorNotFound))]
    [Error(typeof(DuplicatedBook))]
    //[UseMutationConvention]
    public async Task<Book> AddBook(AddBookInput input)
    {
        if (_authorsDb.Get(input.AuthorId) is null)
            throw new AuthorNotFound(input.AuthorId);

        if (_booksDb.Query(book => book.Title == input.Title).Any())
            throw new DuplicatedBook(input.Title);
        
        var book = new Book(Guid.NewGuid(), input.Title, DateTime.UtcNow, input.AuthorId);

        _booksDb.Add(book);

        await _sender.SendAsync(nameof(BookSubscriptions.BookAdded), book);
        
        return book;
    }
}
```
```csharp
public class AppObjectTypes
{
    public const string Query = nameof(Query);
    public const string Mutation = nameof(Mutation);
    public const string Subscription = nameof(Subscription);
}
```
```csharp
public class Startup
{    
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _environment;

    public Startup(IConfiguration configuration, IWebHostEnvironment environment) => (_configuration, _environment) = (configuration, environment);

    public void ConfigureServices(IServiceCollection services) 
    {
        services
            .AddSingleton(new BooksDb())
            .AddSingleton(new AuthorsDb())
            .AddInMemorySubscriptions();

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
            .AddSubscriptionType(d => d.Name(AppObjectTypes.Subscription))
                .AddTypeExtension<BookSubscriptions>()
            .AddErrorFilter(_ => new AppErrorFilter());
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env) => app
        .UseRouting()
        .UseWebSockets()
        .UseEndpoints(endpoints =>
        {
            endpoints.MapGraphQL();
            endpoints.MapGraphQLVoyager();
            endpoints.MapGraphQLPlayground();
        });
}
```