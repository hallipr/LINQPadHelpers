using System.IO;
using LINQPad;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace LINQPadHelpers
{
    using System.Linq;
    using System.Threading.Tasks;
    using Newtonsoft.Json.Linq;

    public static class JsonUtils
    {
        private static readonly JsonSerializerSettings DefaultJsonSettings = BuildDefaultJsonSettings(null);

        private static JsonSerializerSettings BuildDefaultJsonSettings(int? maxDepth)
        {
            return new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Converters = { new StringEnumConverter() },
                Formatting = Formatting.Indented,
                MaxDepth = maxDepth
            };
        }
        
        public static Task<T> ReadJsonFileAsync<T>(string path) => ReadJsonFileAsync<T>(path, DefaultJsonSettings);

        public static async Task<T> ReadJsonFileAsync<T>(string path, JsonSerializerSettings jsonSettings)
        {
            using (var textReader = File.OpenText(path))
            using (var jsonReader = new JsonTextReader(textReader))
            {
                var jsonSerializer = JsonSerializer.Create(jsonSettings);
                var token = await JToken.LoadAsync(jsonReader);
                return token.ToObject<T>(jsonSerializer);
            }
        }

        public static Task WriteJsonFileAsync(string path, object data) => WriteJsonFileAsync(path, data, DefaultJsonSettings);

        public static async Task WriteJsonFileAsync(string path, object data, JsonSerializerSettings jsonSettings)
        {
            using (var stream = File.OpenWrite(path))
            using (var textWriter = new StreamWriter(stream))
            using (var jsonWriter = new JsonTextWriter(textWriter))
            {
                var jsonSerializer = JsonSerializer.Create(jsonSettings);
                await JToken.FromObject(data, jsonSerializer).WriteToAsync(jsonWriter, jsonSerializer.Converters.ToArray());
            }
        }

        public static T ReadJsonFile<T>(string path) => ReadJsonFile<T>(path, DefaultJsonSettings);

        public static T ReadJsonFile<T>(string path, JsonSerializerSettings jsonSettings) => JsonConvert.DeserializeObject<T>(File.ReadAllText(path), jsonSettings);

        public static void WriteJsonFile(string path, object data) => WriteJsonFile(path, data, DefaultJsonSettings);

        public static void WriteJsonFile(string path, object data, JsonSerializerSettings jsonSettings) => File.WriteAllText(path, JsonConvert.SerializeObject(data, jsonSettings));
        
        public static T DumpJson<T>(this T o)
        {
            return o.DumpJson(null, null);
        }

        public static T DumpJson<T>(this T o, string description)
        {
            return o.DumpJson(description, null);
        }

        public static T DumpJson<T>(this T o, int depth)
        {
            return o.DumpJson(null, depth);
        }

        public static T DumpJson<T>(this T o, string description, int? depth)
        {
            var settings = BuildDefaultJsonSettings(depth);

            JsonConvert.SerializeObject(o, settings).Dump(description);

            return o;
        }
    }
}