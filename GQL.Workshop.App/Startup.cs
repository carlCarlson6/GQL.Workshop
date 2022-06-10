using GQL.Workshop.App.Data;
using GQL.Workshop.App.DataLoaders;
using GQL.Workshop.App.Filters;
using GQL.Workshop.App.Mutations;
using GQL.Workshop.App.Queries;
using GQL.Workshop.App.Subscriptions;

namespace GQL.Workshop.App;

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