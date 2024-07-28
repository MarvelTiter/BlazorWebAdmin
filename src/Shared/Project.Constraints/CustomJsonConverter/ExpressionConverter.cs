using Serialize.Linq.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Project.Constraints.CustomJsonConverter
{
    public class ExpressionConverter<T> : JsonConverter<Expression<Func<T, bool>>>
    {
        ExpressionSerializer serializer = new ExpressionSerializer(new Serialize.Linq.Serializers.JsonSerializer());
        public override Expression<Func<T, bool>>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var b = reader.GetString();
            var content = Encoding.UTF8.GetString(Convert.FromBase64String(b));
            var e = serializer.DeserializeText(content);
            return e as Expression<Func<T, bool>>;
        }

        public override void Write(Utf8JsonWriter writer, Expression<Func<T, bool>> value, JsonSerializerOptions options)
        {
            var json = serializer.SerializeText(value);
            writer.WriteBase64StringValue(Encoding.UTF8.GetBytes(json));
        }
    }
}
