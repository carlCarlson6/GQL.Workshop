using GQL.Workshop.App.Data;
using GQL.Workshop.App.DataLoaders;

namespace GQL.Workshop.App.Queries;

[ExtendObjectType(typeof(Author))]
public class AuthorExtensions
{
    private readonly BooksByAuthorGroupDataLoader _dataLoader;

    public AuthorExtensions(BooksByAuthorGroupDataLoader dataLoader) => _dataLoader = dataLoader;

    public async Task<IEnumerable<Book>> GetBooks([Parent] Author author) => await _dataLoader.LoadAsync(author.Id);
}