using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Terminal.Application.Helpers;

public static class XmlHelper
{
    public static T DeserializeXml<T>(string xmlContent)
    {
        var serializer = new XmlSerializer(typeof(T));
        using var reader = new StringReader(xmlContent);
        return (T)serializer.Deserialize(reader)!;
    }
    
    public static string SerializeXml<T>(T obj, bool indent = true)
    {
        var serializer = new XmlSerializer(typeof(T));
    
        var settings = new XmlWriterSettings
        {
            Indent = indent,
            OmitXmlDeclaration = false,
            Encoding = Encoding.UTF8
        };
    
        using var stringWriter = new StringWriter();
        using var xmlWriter = XmlWriter.Create(stringWriter, settings);
    
        var ns = new XmlSerializerNamespaces();
        ns.Add("", "");
    
        serializer.Serialize(xmlWriter, obj, ns);
        return stringWriter.ToString();
    }

    public static T DeserializeXmlFromFile<T>(string filePath)
    {
        var serializer = new XmlSerializer(typeof(T));
        using var reader = new StreamReader(filePath);
        return (T)serializer.Deserialize(reader)!;
    }
    
    public static void SerializeXmlToFile<T>(T obj, string filePath, bool indent = true)
    {
        var xml = SerializeXml(obj, indent);
        File.WriteAllText(filePath, xml, Encoding.UTF8);
    }
}