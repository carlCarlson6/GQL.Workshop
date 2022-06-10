using GQL.Workshop.App.Data;

namespace GQL.Workshop.App.DataLoaders;

public class AuthorBatchDataLoader : BatchDataLoader<Guid, Author>
{
    private readonly AuthorsDb _authorsDb;

    public AuthorBatchDataLoader(IBatchScheduler batchScheduler, AuthorsDb authorsDb, DataLoaderOptions? options = null)
        : base(batchScheduler, options) => _authorsDb = authorsDb;

    protected override Task<IReadOnlyDictionary<Guid, Author>> LoadBatchAsync(IReadOnlyList<Guid> authorIds, CancellationToken cancellationToken)
    {
        var authors = _authorsDb
            .Query(author => authorIds.Contains(author.Id))
            .ToDictionary(a => a.Id);
        return Task.FromResult((IReadOnlyDictionary<Guid, Author>)authors);
    }
}