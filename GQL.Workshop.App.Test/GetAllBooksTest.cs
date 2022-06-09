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
        // Given
        var client = GivenTestHost().GetGqlClient();

        // When
        var result = await client.AllBooks.ExecuteAsync();

        // Then
        result.Should().MatchSnapshot();
    }
}
