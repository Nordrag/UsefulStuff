using Microsoft.Maui.Graphics;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using System.Xml.Serialization;
#pragma warning disable

//binary formatter is obsolate
public static class Serializer
{

    static string path;

    public static void Serialize<T>(T obj)
    {
        IFormatter formatter = new BinaryFormatter();
        Stream stream = new FileStream(path, FileMode.OpenOrCreate);
        formatter.Serialize(stream, obj);
        stream.Close();
    }

    public static T Deserialize<T>()
    {
        IFormatter formatter = new BinaryFormatter();
        Stream stream = new FileStream(path, FileMode.Open);
        var res = formatter.Deserialize(stream);
        stream.Close();
        return (T)res;
    }

    public static void SerializeXml<T>(T obj)
    {
        var serializer = new XmlSerializer(typeof(T));
        using (var writer = new StreamWriter(path, true))
        {
            serializer.Serialize(writer, obj);
        }
    }

    public static T DeserializeXml<T>()
    {
        var serializer = new XmlSerializer(typeof(T));     
        using (Stream reader = new FileStream(path, FileMode.OpenOrCreate))
        {
            try
            {
                return (T)serializer.Deserialize(reader);       
            }
            catch 
            {
                return default(T);  
            }
        }
    }

    public static void SetPath(string p)
    {
        path = p;
    }
}
