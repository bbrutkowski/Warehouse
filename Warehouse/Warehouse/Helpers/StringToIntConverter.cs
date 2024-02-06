using Newtonsoft.Json;

namespace Warehouse.Helpers
{
    public class StringToIntConverter : JsonConverter<int>
    {
        public override int ReadJson(JsonReader reader, Type objectType, int existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
            {
                var value = (string)reader.Value;
                if (value == "__empty_line__")
                    return 0; 
                else
                    return int.Parse(value);
            }
            else if (reader.TokenType == JsonToken.Integer)
            {
                return Convert.ToInt32(reader.Value);
            }
            else
            {
                throw new JsonSerializationException($"Unexpected token type: {reader.TokenType}");
            }
        }

        public override void WriteJson(JsonWriter writer, int value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
