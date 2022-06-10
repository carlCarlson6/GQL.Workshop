using GQL.Workshop.App.Data;

namespace GQL.Workshop.App.Mutations;

[ExtendObjectType(AppObjectTypes.Mutation)]
public class PublicationMutation
{
    private readonly BooksDb _booksDb;
    private readonly MagazinesDb _magazinesDb;

    public PublicationMutation(BooksDb booksDb, MagazinesDb magazinesDb) =>
        (_booksDb, _magazinesDb) = (booksDb, magazinesDb);

    public Task<IPublication> AddPublication(AddPublicationInput input)
    {
        var publication = input switch
        {
            { BookInput: { } } => new Book(Guid.NewGuid(), input.BookInput.Title, DateTime.UtcNow,
                input.BookInput.AuthorId),
            { MagazineInput: { } } => (IPublication)new Magazine(Guid.NewGuid(), input.MagazineInput.Title,
                DateTime.UtcNow, input.MagazineInput.AuthorIds),
            _ => throw new ArgumentOutOfRangeException(nameof(input), input, null)
        } ?? throw new Exception();
        

        Action savePublication = publication switch
        {
            Book book => () => _booksDb.Add(book),
            Magazine magazine => () => _magazinesDb.Add(magazine),
            _ => throw new ArgumentOutOfRangeException()
        };
        savePublication();

        return Task.FromResult(publication);
    }
}

[OneOf] 
public record AddPublicationInput(AddBookInput? BookInput, AddMagazineInput? MagazineInput);

public record AddMagazineInput(string Title, IEnumerable<Guid> AuthorIds);