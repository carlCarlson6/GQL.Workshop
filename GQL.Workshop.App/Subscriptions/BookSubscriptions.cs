using GQL.Workshop.App.Data;

namespace GQL.Workshop.App.Subscriptions;

[ExtendObjectType(AppObjectTypes.Subscription)]
public class BookSubscriptions
{
    [Subscribe]
    //[Topic("ExampleTopic")]
    public Book BookAdded([EventMessage] Book book) => book;
}