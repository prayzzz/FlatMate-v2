using System.IO;
using System.Xml.Serialization;

namespace FlatMate.Module.Common
{
    public static class XmlConvert
    {
        public static T Deserialize<T>(string objectData)
        {
            using (TextReader reader = new StringReader(objectData))
            {
                return (T)new XmlSerializer(typeof(T)).Deserialize(reader);
            }
        }

        public static string Serialize<T>(T obj)
        {
            using (TextWriter writer = new StringWriter())
            {
                new XmlSerializer(typeof(T)).Serialize(writer, obj);
                return writer.ToString();
            }
        }
    }
}
