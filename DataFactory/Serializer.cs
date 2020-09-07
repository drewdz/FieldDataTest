using Newtonsoft.Json;
using System;
using System.IO;

namespace DataFactory
{
    public static class Serializer
    {
        public static T DeserializeFile<T>(string path)
        {
            using (var sr = new StreamReader(path))
            {
                return Deserialize<T>(sr.ReadToEnd());
            }
        }

        public static string Serialize<TEntity>(TEntity entity)
        {
            var serializer = new JsonSerializer();
            using (StringWriter sr = new StringWriter())
            {
                serializer.Serialize(sr, entity);
                sr.Flush();
                return sr.ToString();
            }
        }

        public static void Serialize<TEntity>(TEntity entity, Stream stream)
        {
            var serializer = new JsonSerializer();
            using (var writer = new StreamWriter(stream))
            {
                writer.Write(Serialize(entity));
                writer.Flush();
            }
        }

        public static TEntity Deserialize<TEntity>(string json)
        {
            if (json.Contains("DOCTYPE html"))
            {
                System.Diagnostics.Debug.WriteLine($"*** Serializer.Deserialize - Cannot deserialize HTML\r\n\t{json}");
                throw new ArgumentException("Cannot deserialize HTML");
            }

            var serializer = new JsonSerializer();
            try
            {
                using (var sr = new StringReader(json))
                {
                    using (var reader = new JsonTextReader(sr))
                    {
                        return serializer.Deserialize<TEntity>(reader);
                    }
                }
            }
            catch (JsonSerializationException jsx)
            {
                System.Diagnostics.Debug.WriteLine($"*** Serializer.Deserialize - Unable to deserialize json: {json}");
                throw jsx;
            }
        }

        public static TEntity Deserialize<TEntity>(Stream stream)
        {
            var serializer = new JsonSerializer();
            try
            {
                using (var sr = new StreamReader(stream))
                {
                    using (var reader = new JsonTextReader(sr))
                    {
                        return serializer.Deserialize<TEntity>(reader);
                    }
                }
            }
            catch (JsonSerializationException jsx)
            {
                System.Diagnostics.Debug.WriteLine($"*** Serializer.Deserialize - Unable to deserialize json from stream");
                throw jsx;
            }
        }

        public static object Deserialize(System.Type entityType, string json)
        {
            if (json.Contains("DOCTYPE html"))
            {
                System.Diagnostics.Debug.WriteLine($"*** Serializer.Deserialize - Cannot deserialize HTML\r\n\t{json}");
                throw new ArgumentException("Cannot deserialize HTML");
            }

            var serializer = new JsonSerializer()
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            try
            {
                using (var sr = new StringReader(json))
                {
                    using (var reader = new JsonTextReader(sr))
                    {
                        return serializer.Deserialize(reader, entityType);
                    }
                }
            }
            catch (JsonSerializationException jsx)
            {
                System.Diagnostics.Debug.WriteLine($"*** Serializer.Deserialize - Unable to deserialize json: {json}");
                throw jsx;
            }
        }
    }
}
