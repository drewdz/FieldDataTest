using Newtonsoft.Json;
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

        public static T Deserialize<T>(string s)
        {
            using (var sr = new StringReader(s))
            {
                using (var jr = new JsonTextReader(sr))
                {
                    var serializer = new JsonSerializer();
                    return serializer.Deserialize<T>(jr);
                }
            }
        }
    }
}
