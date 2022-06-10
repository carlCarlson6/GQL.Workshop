using GQL.Workshop.App.Data;

namespace GQL.Workshop.App.Queries;

[ExtendObjectType(typeof(Magazine))]
public class MagazineExtensions
{
    private readonly AuthorsDb _authorsDb;

    public MagazineExtensions(AuthorsDb authorsDb) => _authorsDb = authorsDb;

    public Task<IEnumerable<Author>> GetAuthors([Parent] Magazine magazine)
    {
        var authors = _authorsDb.Query(author => magazine.AuthorIds.Contains(author.Id));
        if (!authors.Any())
            throw new Exception($"authors not for given magazine: {magazine}");

        return Task.FromResult(authors);
    }
}