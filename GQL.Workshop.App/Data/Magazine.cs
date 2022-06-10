namespace GQL.Workshop.App.Data;

public record Magazine(
    Guid Id, 
    string Title, 
    DateTime Published, 
    [property: GraphQLIgnore]
    IEnumerable<Guid> AuthorIds) : IPublication;

public class MagazinesDb : Db<Magazine> 
{
    public override Magazine? Get(Guid id) => throw new NotImplementedException();
    public override IEnumerable<Magazine> Query(Func<Magazine, bool> query) => throw new NotImplementedException();
    public override void Update(Magazine entity) => throw new NotImplementedException();
    public override void Delete(Guid id) => throw new NotImplementedException();
}