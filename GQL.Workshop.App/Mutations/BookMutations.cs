using GQL.Workshop.App.Data;

namespace GQL.Workshop.App.Mutations;

[ExtendObjectType(AppObjectTypes.Mutation)]
public class BookMutations
{
    [Error(typeof(AuthorNotFound))]
    [Error(typeof(DuplicatedBook))]
    //[UseMutationConvention]
    public Task<Book> AddBook([Service] BooksDb db, [Service] AuthorsDb authorsDb,  AddBookInput input)
    {
        if (authorsDb.Get(input.AuthorId) is null)
            throw new AuthorNotFound(input.AuthorId);

        if (db.Query(book => book.Title == input.Title).Any())
            throw new DuplicatedBook(input.Title);
        
        var book = new Book(Guid.NewGuid(), input.Title, DateTime.UtcNow, input.AuthorId);

        db.Add(book);

        return Task.FromResult(book);
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