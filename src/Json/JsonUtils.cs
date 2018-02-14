using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace LINQPadHelpers.Json
{
    public static class JsonUtils
    {
        public static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Converters = { new StringEnumConverter() },
            Formatting = Formatting.Indented
        };

        public static void WriteJsonFile(string path, object data) => File.WriteAllText(path, JsonConvert.SerializeObject(data, JsonSettings));

        public static T ReadJsonFile<T>(string path) => JsonConvert.DeserializeObject<T>(File.ReadAllText(path), JsonSettings);

    }
}