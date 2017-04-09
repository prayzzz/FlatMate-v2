using System;
using Newtonsoft.Json;

namespace FlatMate.Web.Mvc.Json
{
    public class DateTimeConverter : JsonConverter
    {
        private static readonly Type DateTimeType;
        private static readonly Type NullableDateTimeType;

        static DateTimeConverter()
        {
            DateTimeType = typeof(DateTime);
            NullableDateTimeType = typeof(DateTime?);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == DateTimeType || objectType == NullableDateTimeType;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var value = reader.Value.ToString();

            return DateTime.TryParse(value, out DateTime dateTime) ? dateTime : DateTime.MinValue;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var dateTime = value as DateTime?;

            if (dateTime == null)
            {
                writer.WriteValue("");
                return;
            }

            writer.WriteValue(dateTime.Value.ToString("s"));
        }
    }
}