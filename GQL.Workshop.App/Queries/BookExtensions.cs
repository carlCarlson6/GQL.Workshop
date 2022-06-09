using GQL.Workshop.App.Data;

namespace GQL.Workshop.App.Queries;

[ExtendObjectType(typeof(Book))]
public class BookExtensions
{
    private readonly AuthorsDb _authorsDb;

    public BookExtensions(AuthorsDb authorsDb) => _authorsDb = authorsDb;

    public Task<Author> GetAuthor([Parent] Book book)
    {
        var author = _authorsDb.Get(book.AuthorId);
        if (author is null)
            throw new Exception($"author not found for given book: {book}");
        
        return Task.FromResult(author);
    }
}