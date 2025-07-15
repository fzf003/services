var body = JsonContent.Create(person, options: new JsonSerializerOptions
{
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
     Encoder= System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
});

Console.Write(await body.ReadAsStringAsync());



var json = "{ \"response\": \"我是一个由人工智能生成的助手，位于虚拟空间，没有具体的地理位置。我随时可以为你提供帮助！\"}";
//var isvaild = JsonDocument.Parse(json);


var reader = new Utf8JsonReader(System.Text.Encoding.UTF8.GetBytes(json));

if (JsonDocument.TryParseValue(ref reader, out var document))
{
    using (document)
    {
        Console.WriteLine(document.RootElement.GetProperty("response").GetString());

    }
}

