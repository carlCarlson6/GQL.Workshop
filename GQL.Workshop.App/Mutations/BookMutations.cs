using GQL.Workshop.App.Data;
using GQL.Workshop.App.Subscriptions;
using HotChocolate.Subscriptions;

namespace GQL.Workshop.App.Mutations;

[ExtendObjectType(AppObjectTypes.Mutation)]
public class BookMutations
{
    private readonly ITopicEventSender _sender;
    private readonly BooksDb _booksDb;
    private readonly AuthorsDb _authorsDb;

    public BookMutations(ITopicEventSender sender, BooksDb booksDb, AuthorsDb authorsDb)
    {
        _sender = sender;
        _booksDb = booksDb;
        _authorsDb = authorsDb;
    }

    [Error(typeof(AuthorNotFound))]
    [Error(typeof(DuplicatedBook))]
    //[UseMutationConvention]
    public async Task<Book> AddBook(AddBookInput input)
    {
        if (_authorsDb.Get(input.AuthorId) is null)
            throw new AuthorNotFound(input.AuthorId);

        if (_booksDb.Query(book => book.Title == input.Title).Any())
            throw new DuplicatedBook(input.Title);
        
        var book = new Book(Guid.NewGuid(), input.Title, DateTime.UtcNow, input.AuthorId);

        _booksDb.Add(book);

        await _sender.SendAsync(nameof(BookSubscriptions.BookAdded), book);
        
        return book;
    }
}

public class DuplicatedBook : AppError
{
    public DuplicatedBook(string title) : base(
        $"book with title {title} already in the DB", 
        "DUPLICATED_BOOK") { }
}

public class AuthorNotFound : AppError
{
    public AuthorNotFound(Guid author) : base(
        $"cannot find author with id {author}", 
        "AUTHOR_NOT_FOUND") { }
}

public record AddBookInput(string Title, Guid AuthorId);