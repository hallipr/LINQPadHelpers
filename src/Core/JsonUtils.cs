using System.IO;
using LINQPad;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace LINQPadHelpers
{
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

        public static void WriteJsonFile(string path, object data, JsonSerializerSettings jsonSettings = null) => File.WriteAllText(path, JsonConvert.SerializeObject(data, jsonSettings ?? DefaultJsonSettings));

        public static T ReadJsonFile<T>(string path, JsonSerializerSettings jsonSettings = null) => JsonConvert.DeserializeObject<T>(File.ReadAllText(path), jsonSettings ?? DefaultJsonSettings);


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