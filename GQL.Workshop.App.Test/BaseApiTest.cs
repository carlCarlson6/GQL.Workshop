using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using System.Net.Http;

namespace GQL.Workshop.App.Test;

public abstract class BaseApiTest 
{
    protected IWebHost GivenTestHost() => 
        WebHost
            .CreateDefaultBuilder()
            .UseStartup<Startup>()
            .UseTestServer()
            .UseDefaultServiceProvider((_, options) =>
            {
                // makes sure DI lifetimes and scopes don't have common issues
                options.ValidateScopes = true;
                options.ValidateOnBuild = true;
            })
            .Start();
}

public static class TestingExtensions 
{
    public static GqlClient GetGqlClient(this IWebHost host) 
    {
        var httpClient = host.GetTestClient();
        httpClient.BaseAddress = new Uri("http://localhost/graphql/"); // proper base address needs to be set
        
        var services = new ServiceCollection()
            .AddScoped(sp =>
            {
                var httpClientFactory = Substitute.For<IHttpClientFactory>();
                httpClientFactory.CreateClient().Returns(httpClient);
                httpClientFactory.CreateClient(Arg.Any<string>()).Returns(httpClient); // important otherwise HttpConnection can't resolve
                return httpClientFactory;
            });

        services.AddGqlClient();

        return services.BuildServiceProvider()
                .GetRequiredService<GqlClient>();
    }    
}