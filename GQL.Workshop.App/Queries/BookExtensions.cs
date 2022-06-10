using GQL.Workshop.App.Data;
using GQL.Workshop.App.DataLoaders;

namespace GQL.Workshop.App.Queries;

[ExtendObjectType(typeof(Book))]
public class BookExtensions
{
    private readonly AuthorBatchDataLoader _authorBatchDataLoader;

    public BookExtensions(AuthorBatchDataLoader authorBatchDataLoader) => _authorBatchDataLoader = authorBatchDataLoader;

    public Task<Author> GetAuthor([Parent] Book book) => _authorBatchDataLoader.LoadAsync(book.AuthorId);
}