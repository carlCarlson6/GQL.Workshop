namespace GQL.Workshop.App.Data;

public record Author(Guid Id, string Name);

public class AuthorsDB
{
    private static IEnumerable<Author> _authors = new List<Author>();

    public void Add(Author author) => _authors = _authors.Append(author);

    public IEnumerable<Author> Get() => _authors;
    
    public Author? Get(Guid id) => _authors.FirstOrDefault(book => book.Id == id);
    
    public void Update(Author author) 
    {
        var authors = _authors.Where(a => a.Id != author.Id);
        _authors = authors.Append(author);
    }
    
    public void Delete(Guid id) => _authors = _authors.Where(a => a.Id == id);
}