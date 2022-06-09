using GQL.Workshop.App.Data;
using GQL.Workshop.App.Filters;
using GQL.Workshop.App.Mutations;
using GQL.Workshop.App.Queries;

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
            .AddSingleton(new AuthorsDb());

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

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env) => app
        .UseRouting()
        .UseEndpoints(endpoints =>
        {
            endpoints.MapGraphQL();
            endpoints.MapGraphQLVoyager();
            endpoints.MapGraphQLPlayground();
        });
}