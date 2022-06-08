namespace GQL.Workshop.App.Data;

public record Book(Guid Id, string Title, DateTime Published, Guid AuthorId);

public class BooksDB
{
    private static IEnumerable<Book> _books = new List<Book> {
        new(new Guid(), "Is graphql better than rest?", DateTime.UtcNow, new Guid()),
        new(new Guid(), "The succss of Colba", DateTime.UtcNow, new Guid()),
    };

    public void Add(Book book) => _books = _books.Append(book);
    
    public IEnumerable<Book> Get() => _books;
    
    public Book? Get(Guid id) => _books.FirstOrDefault(book => book.Id == id);
    
    public void Update(Book book) 
    {
        var books = _books.Where(b => b.Id != book.Id);
        _books = books.Append(book);
    }
    
    public void Delete(Guid id) => _books = _books.Where(book => book.Id != id);
}