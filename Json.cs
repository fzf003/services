var body = JsonContent.Create(person, options: new JsonSerializerOptions
{
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
     Encoder= System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
});

Console.Write(await body.ReadAsStringAsync());
