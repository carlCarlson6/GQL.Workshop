using GQL.Workshop.App.Data;

namespace GQL.Workshop.App.Queries;

[ExtendObjectType(typeof(Author))]
public class AuthorExtensions
{
    private readonly BooksDb _booksDb;

    public AuthorExtensions(BooksDb booksDb) => _booksDb = booksDb;

    public Task<IEnumerable<Book>> GetBooks([Parent] Author author)
    {
        var books = _booksDb.Query(book => book.AuthorId == author.Id);
        return Task.FromResult(books);
    }
}