namespace GQL.Workshop.App.Data;

public record Author(Guid Id, string Name);

public class AuthorsDb
{
    private static IEnumerable<Author> _authors = new List<Author>
    {
        new(new Guid("4f8df196-f699-44fe-9351-4b1d2ca0f0a5"), "Carl Carlson III"),
        new (new Guid("779316ff-1845-4d83-89c9-68720d486bac"), "Juanjo & Danny")
    };

    public void Add(Author author) => _authors = _authors.Append(author);

    public IEnumerable<Author> Get() => _authors;
    public IEnumerable<Author> Query(Func<Author, bool> query) => _authors.Where(query);
    
    public Author? Get(Guid id) => _authors.FirstOrDefault(book => book.Id == id);
    
    public void Update(Author author) 
    {
        var authors = _authors.Where(a => a.Id != author.Id);
        _authors = authors.Append(author);
    }
    
    public void Delete(Guid id) => _authors = _authors.Where(a => a.Id == id);
}