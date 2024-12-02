using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sabatex.Extensions.Converters.Json;

public class TimeOnlyConverter : JsonConverter<TimeOnly>
{
    private const string DateTimeFormat = "0001-01-01THH:mm:ss";
    private const string TimeFormat = "HH:mm:ss.FFFFFFF";
    
    public override TimeOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return TimeOnly.ParseExact(reader.GetString() ?? String.Empty, DateTimeFormat, CultureInfo.InvariantCulture);
    }
    
    public override void Write(Utf8JsonWriter writer, TimeOnly value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(DateTimeFormat, CultureInfo.InvariantCulture));
    }
}