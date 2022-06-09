namespace GQL.Workshop.App.Data;

public record Book(
    Guid Id, 
    string Title, 
    DateTime Published, 
    [property: GraphQLIgnore] 
    Guid AuthorId);

public class BooksDb
{
    private static IEnumerable<Book> _books = new List<Book> 
    {
        new(new Guid("0dacb47e-2e37-459b-bc25-b9ab58bf89ee"), "Is graphql better than rest?", DateTime.UtcNow, new Guid("4f8df196-f699-44fe-9351-4b1d2ca0f0a5")),
        new(new Guid("951c4b62-d33e-4687-bff4-397816c0860d"), "The succss of Colba", DateTime.UtcNow, new Guid("779316ff-1845-4d83-89c9-68720d486bac")),
        new(new Guid("873af69f-2f36-40bf-b49a-dde09a5a7ca4"), "Why Java is not cool anymore", DateTime.UtcNow, new Guid("c1074402-27ef-455f-b099-1bb9123c4a77")),
    };

    public void Add(Book book) => _books = _books.Append(book);
    
    public IEnumerable<Book> Get() => _books;
    
    public Book? Get(Guid id) => _books.FirstOrDefault(book => book.Id == id);
    public IEnumerable<Book> Query(Func<Book, bool> query) => _books.Where(query);

    public void Update(Book book) 
    {
        var books = _books.Where(b => b.Id != book.Id);
        _books = books.Append(book);
    }
    
    public void Delete(Guid id) => _books = _books.Where(book => book.Id != id);
}