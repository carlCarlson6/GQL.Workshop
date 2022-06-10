using GQL.Workshop.App.Data;

namespace GQL.Workshop.App.DataLoaders;

public class BooksByAuthorGroupDataLoader : GroupedDataLoader<Guid, Book>
{
    private readonly BooksDb _booksDb;
    
    public BooksByAuthorGroupDataLoader(IBatchScheduler batchScheduler, BooksDb booksDb, DataLoaderOptions? options = null) 
        : base(batchScheduler, options) => _booksDb = booksDb;
    
    protected override Task<ILookup<Guid, Book>> LoadGroupedBatchAsync(IReadOnlyList<Guid> authorIds, CancellationToken cancellationToken)
    {
        var books = _booksDb.Query(book => authorIds.Contains(book.AuthorId));
        return Task.FromResult(books.ToLookup(b => b.AuthorId));
    }
}