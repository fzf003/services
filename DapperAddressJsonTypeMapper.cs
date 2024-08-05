using Dapper;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace SqlServerTools
{
    [Table("Car")]
    public class Car
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }

        public string Home { get; set; }

        public int State { get; set; }

        public List<Address> Metadata { get; set; } = new List<Address>();
    }

    public record Address
    {
        public string Tel { get; set; }
        public string City { get; set; }
    }
    /// <summary>
    ///  SqlMapper.AddTypeHandler(new AddressJsonTypeMapper());
    /// </summary>
    public class AddressJsonTypeMapper : SqlMapper.TypeHandler<List<Address>>
    {
        readonly static JsonSerializerOptions jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };

        public override List<Address> Parse(object value)
        {
            if (value is null) return new List<Address>();

            var json = value.ToString();

            if (string.IsNullOrWhiteSpace(json)) return new List<Address>();

            var res = JsonSerializer.Deserialize<List<Address>>(json, jsonOptions);
 
            if (res is null) return new List<Address>();

            return res;
        }

        public override void SetValue(IDbDataParameter parameter, List<Address> value)
        {
            parameter.Value = value is null ? null : JsonSerializer.Serialize(value, jsonOptions);
        }
    }

    public class InstallmentSpanTypeMapper: SqlMapper.TypeHandler<List<Address>>
    {
        readonly static JsonSerializerOptions jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };

        public override List<Address> Parse(object value)
        {
            if (value is not byte[] bytes)
            {
                return new List<Address>();
            }

            var span = bytes.AsSpan();


            // var structSpan = MemoryMarshal.Cast<byte, Address>(span);

            //JsonSerializer.SerializeToUtf8Bytes<Address>()
 
           // using var stream= new MemoryStream(bytes);
            return JsonSerializer.Deserialize<List<Address>>(bytes, jsonOptions);


           // return structSpan.ToArray().ToList();
        }

        public override void SetValue(IDbDataParameter parameter,List<Address> value)
        {
            //var s = CollectionsMarshal.AsSpan(value);

           // Span<byte> span = MemoryMarshal.AsBytes(s);

            var span=JsonSerializer.SerializeToUtf8Bytes<List<Address>>(value, jsonOptions);

            parameter.Value = span;
        }
    }

}
