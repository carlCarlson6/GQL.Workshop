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