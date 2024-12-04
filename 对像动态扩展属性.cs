/*
  
JsonSerializerOptions jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, WriteIndented = true };

Person person = new Person("fzf003", 89);

Completed completed=new Completed(person.Name,"是");

person.AddProperty<Completed>("ad",completed);

var span = JsonSerializer.Serialize<Person>(person, jsonOptions);

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
    private static readonly Dictionary<Person, Dictionary<string, object>> _additionalProperties = new();

    public static Person AddProperty<T>(this Person person, string propertyName, object value)
    {
        if (!_additionalProperties.ContainsKey(person))
        {
            _additionalProperties[person] = new Dictionary<string, object>();
        }
        _additionalProperties[person][propertyName] = value;
        return person;
    }

    public static T GetProperty<T>(this Person person, string propertyName)
    {
        if (_additionalProperties.ContainsKey(person) && _additionalProperties[person].ContainsKey(propertyName))
        {
            return (T)_additionalProperties[person][propertyName];
        }
       return default!;
    }
}
