using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Terminal.Application.Helpers;

public static class XmlHelper
{
    private const string Format = "yyyy-MM-dd HH:mm:ss";
    private const string Empty = "0000-00-00 00:00:00";
    
    public static T DeserializeXml<T>(string xmlContent)
    {
        var serializer = new XmlSerializer(typeof(T));
        using var reader = new StringReader(xmlContent);
        return (T)serializer.Deserialize(reader)!;
    }
    
    /// <summary>
    /// Сериализует объект в XML-строку с указанной кодировкой
    /// </summary>
    /// <typeparam name="T">Тип сериализуемого объекта</typeparam>
    /// <param name="obj">Объект для сериализации</param>
    /// <param name="indent">Форматировать ли XML с отступами</param>
    /// <param name="encoding">Кодировка XML-документа. По умолчанию windows-1251</param>
    /// <returns>XML-строка с корректным объявлением кодировки</returns>
    public static string SerializeXml<T>(T obj, bool indent = true, Encoding? encoding = null)
    {
        encoding ??= Encoding.GetEncoding("windows-1251");
    
        var settings = new XmlWriterSettings
        {
            Indent = indent,
            OmitXmlDeclaration = false,
            Encoding = encoding,
            IndentChars = "  ",
            NewLineChars = Environment.NewLine
        };

        using var memoryStream = new MemoryStream();
        using var xmlWriter = XmlWriter.Create(memoryStream, settings);

        var ns = new XmlSerializerNamespaces();
        ns.Add("", "");

        var serializer = new XmlSerializer(typeof(T));
        serializer.Serialize(xmlWriter, obj, ns);

        xmlWriter.Flush();
        memoryStream.Position = 0;
    
        using var reader = new StreamReader(memoryStream, encoding);
        return reader.ReadToEnd();
    }
    
    /// <summary>
    /// DateTime -> XML строка
    /// </summary>
    public static string DateTimeToXml(DateTime date) =>
        date <= DateTime.MinValue ? Empty : date.ToString(Format, CultureInfo.InvariantCulture);

    /// <summary>
    /// XML строка -> DateTime
    /// </summary>
    public static DateTime DateTimeFromXml(string? value) =>
        string.IsNullOrEmpty(value) || value == Empty
            ? DateTime.MinValue
            : DateTime.ParseExact(value, Format, CultureInfo.InvariantCulture);

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