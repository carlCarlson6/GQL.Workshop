add strawberry shake cli tools
```
dotnet new tool-manifest
```
```
dotnet tool install StrawberryShake.Tools --local
```

install required packages
```
dotnet add GQL.Workshop.App.Test package StrawberryShake.Transport.Http
```
```
dotnet add GQL.Workshop.App.Test package StrawberryShake.CodeGeneration.CSharp.Analyzers
```
```
dotnet add GQL.Workshop.App.Test package Microsoft.Extensions.DependencyInjection
```
```
dotnet add GQL.Workshop.App.Test package Microsoft.Extensions.Http
```

add a gql client to your project using the cli tools
```
dotnet graphql init https://localhost:7131/graphql/ -n GqlClient -p ./GQL.Workshop.App.Test
```

create your first queries
```graphql
query allBooks {
    books {
        title
    }
}
```
build the project

BaseApiTest.cs
```csharp
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
```

GetAllBooksTest.cs
```csharp
using System.Threading.Tasks;
using Snapshooter.Xunit;
using FluentAssertions;
using Xunit;

namespace GQL.Workshop.App.Test;

public class GetAllBooksTest : BaseApiTest
{
    [Fact]
    public async Task GetAllBooks()
    {
        var client = GivenTestHost().GetGqlClient();
        var result = await client.AllBooks.ExecuteAsync();
        result.Should().MatchSnapshot();
    }
}
```