using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
public class DecimalFormatConverter : JsonConverter
{
  public override bool CanConvert(Type objectType)
  {
    return (objectType == typeof(decimal) || (objectType == typeof(decimal?)));
  }

  public override void WriteJson(JsonWriter writer, object? value,
                                 JsonSerializer serializer)
  {
    writer.WriteRawValue($"{value:0.00}");
  }

  public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
  {
    JToken token = JToken.Load(reader);
    decimal value = Decimal.Parse(token.ToString());
    return Decimal.Round(value, 2);
  }
}