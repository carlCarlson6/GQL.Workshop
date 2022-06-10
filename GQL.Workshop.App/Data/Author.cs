namespace GQL.Workshop.App.Data;

public record Author(Guid Id, string Name);

public class AuthorsDb : Db<Author>
{
    public AuthorsDb() => _entities = new List<Author>
    {
        new(new Guid("4f8df196-f699-44fe-9351-4b1d2ca0f0a5"), "Carl Carlson III"),
        new (new Guid("779316ff-1845-4d83-89c9-68720d486bac"), "Juanjo & Danny")
    }; 
    
    public override IEnumerable<Author> Query(Func<Author, bool> query) => _entities.Where(query);
    
    public override Author? Get(Guid id) => _entities.FirstOrDefault(book => book.Id == id);
    
    public override void Update(Author author) 
    {
        var authors = _entities.Where(a => a.Id != author.Id);
        _entities = authors.Append(author);
    }
    
    public override void Delete(Guid id) => _entities = _entities.Where(a => a.Id == id);
}