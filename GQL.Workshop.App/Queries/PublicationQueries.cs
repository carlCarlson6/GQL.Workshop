using GQL.Workshop.App.Data;

namespace GQL.Workshop.App.Queries;

[ExtendObjectType(AppObjectTypes.Query)]
public class PublicationQueries
{
    private readonly BooksDb _booksDb;
    private readonly MagazinesDb _magazinesDb;

    public PublicationQueries(BooksDb booksDb, MagazinesDb magazinesDb) =>
        (_booksDb, _magazinesDb) = (booksDb, magazinesDb);
    
    public Task<IPublication?> GetPublication(Guid publicationId) => throw new NotImplementedException();

    public Task<IEnumerable<IPublication>> GetPublications()
    {
        var books = _booksDb.Get();
        var magazines = _magazinesDb.Get();
        var publications = new List<IPublication>()
            .Concat(books)
            .Concat(magazines);
        return Task.FromResult(publications);
    }
}