
public interface IProjection
{
    Type[] Handles { get; }
    void Handle(object @event);
}

public abstract class Projection : IProjection
{
    private readonly ConcurrentDictionary<Type, Action<object>> handlers = new();

    public Type[] Handles => handlers.Keys.ToArray();

    protected void Projects<TEvent>(Action<TEvent> action)
    {
        handlers.GetOrAdd(typeof(TEvent), @event => action((TEvent)@event));
    }

    public void Handle(object @event)
    {
        handlers[@event.GetType()](@event);
    }
}

public record UserCreated(string Name, int TaskId)
{

}

public class MyProjection : Projection
{
    public MyProjection()
    {
        Projects<UserCreated>(Apply);
    }

    void Apply(UserCreated created)
    {
        Console.WriteLine(created.Name + "-" + created.TaskId);
    }
}


public class ViewManager
{
    public static void Run()
    {
        var MyProjection = Activator.CreateInstance(typeof(MyProjection), true)! as MyProjection;

        MyProjection.Handle(new UserCreated("fzf003", 99839));
    }
}
