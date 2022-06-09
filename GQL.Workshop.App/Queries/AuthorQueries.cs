using GQL.Workshop.App.Data;

namespace GQL.Workshop.App.Queries;

[ExtendObjectType(AppObjectTypes.Query)]
public class AuthorQueries
{
    private readonly AuthorsDb _authorsDb;
    
    public AuthorQueries(AuthorsDb authorsDb) => _authorsDb = authorsDb;
    
    public Task<IEnumerable<Author>> GetAuthors() => Task.FromResult(_authorsDb.Get());
}