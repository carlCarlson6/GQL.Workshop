using GQL.Workshop.App.Data;

namespace GQL.Workshop.App;

public class Queries
{
    private readonly BooksDB _booksDb;
    private readonly AuthorsDB _authorsDb;

    public Queries(BooksDB booksDb, AuthorsDB authorsDb)
    {
        _booksDb = booksDb;
        _authorsDb = authorsDb;
    }

    public Task<IEnumerable<Book>> GetBooks() => Task.FromResult(_booksDb.Get());
    public Task<IEnumerable<Author>> GetAuthors() => Task.FromResult(_authorsDb.Get());
}