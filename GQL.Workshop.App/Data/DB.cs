namespace GQL.Workshop.App.Data;

public abstract class Db<T>
{
    protected static IEnumerable<T> _entities = new List<T>();
    
    public void Add(T entity) => _entities = _entities.Append(entity);
    public IEnumerable<T> Get() => _entities;
    
    public abstract T? Get(Guid id);
    public abstract IEnumerable<T> Query(Func<T, bool> query);
    public abstract void Update(T entity);
    public abstract void Delete(Guid id);
}