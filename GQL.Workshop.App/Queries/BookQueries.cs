using GQL.Workshop.App.Data;

namespace GQL.Workshop.App.Queries;

[ExtendObjectType(AppObjectTypes.Query)]
public class BookQueries
{
    private readonly BooksDb _booksDb;

    public BookQueries(BooksDb booksDb) => _booksDb = booksDb;

    public Task<Book?> GetBook(Guid bookId) => Task.FromResult(_booksDb.Get(bookId));

    public Task<IEnumerable<Book>> GetBooks()
    {
        var books = _booksDb.Get();
        return Task.FromResult(books);
    }
}