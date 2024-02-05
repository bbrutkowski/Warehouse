using Newtonsoft.Json;

namespace Warehouse.Helpers
{
    public class BooleanConverter : JsonConverter<bool>
    {
        public override bool ReadJson(JsonReader reader, Type objectType, bool existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.Value is string stringValue) return stringValue == "1";
         
            return Convert.ToBoolean(reader.Value);
        }

        public override void WriteJson(JsonWriter writer, bool value, JsonSerializer serializer)
        {
            writer.WriteValue(value ? "1" : "0");
        }
    }
}
