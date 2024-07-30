var instanceExpr = Expression.Parameter(typeof(Foo));
 
 ParameterExpression paramB = Expression.Parameter(typeof(string), "name");

var _foo=new Foo();

var methodInfo = typeof(Foo).GetMethod("Bar", BindingFlags.NonPublic | BindingFlags.Instance);

var callExpression=Expression.Call(instanceExpr, methodInfo!,paramB);
 
var _exprAction = Expression.Lambda<Action<Foo,string>>(callExpression, instanceExpr,paramB).Compile();

 methodInfo=typeof(Foo).GetMethod("Handler", BindingFlags.Public | BindingFlags.Instance);
 
var _delegateAction = (Func<Task>)Delegate.CreateDelegate(typeof(Func<Task>), _foo, methodInfo!);

 
 _exprAction.DynamicInvoke(_foo,"fzf003");

  await _delegateAction();
 
 
 

public class Foo
{
	private void Bar(string name) {

		Console.WriteLine($"Bar-{Guid.NewGuid().ToString("N")}-{name}");
	}

    public async Task Handler()
    {
        Console.WriteLine($"Handler-{Guid.NewGuid().ToString("N")}");
    }
}

