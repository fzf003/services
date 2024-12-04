/*
  
JsonSerializerOptions jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, WriteIndented = true };

Person person = new Person("fzf003", 89);

Completed completed=new Completed(person.Name,"是");

person.AddProperty("Title", Guid.NewGuid().ToString("N"))
      .AddProperty("ad", completed);

var span = JsonSerializer.Serialize<Person>(person, jsonOptions);

var title=person.GetProperty<string>("Title");

Console.WriteLine(title);

var readcompleted= person.GetProperty<Completed>("ad");

Console.WriteLine(readcompleted.Name+"--"+readcompleted.IsCompleted);

Console.WriteLine(span);

/*
*/


public class Person
{
    public Person():this(string.Empty,0)
    {
        
    }
    public Person(string name, int age)
    {
        this.Name = name;
        this.Age = age;
    }

    public string Name { get; set; }
    public int Age { get; set; }
}

public class Completed
{
    public Completed(string name, string isCompleted)
    {
        Name = name;
        IsCompleted = isCompleted;
    }

    public string Name { get; set; }
    public string IsCompleted { get; set; }
   
}

public static class PersonExtensions
{
    private static readonly Dictionary<object, Dictionary<string, object>> _additionalProperties = new();

    public static T AddProperty<T>(this T obj, string propertyName, object value)
    {
        if (!_additionalProperties.ContainsKey(obj))
        {
            _additionalProperties[obj] = new Dictionary<string, object>();
        }
        _additionalProperties[obj][propertyName] = value;
        return obj;
    }

    public static T GetProperty<T>(this object obj, string propertyName)
    {
        if (_additionalProperties.ContainsKey(obj) && _additionalProperties[obj].ContainsKey(propertyName))
        {
            return (T)_additionalProperties[obj][propertyName];
        }
        return default!;
    }
}
