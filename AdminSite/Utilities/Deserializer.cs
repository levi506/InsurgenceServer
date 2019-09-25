using Newtonsoft.Json;

namespace AdminSite.Utilities
{
    public static class Deserializer
    {
        private static JsonSerializerSettings _settings = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore
        };
        public static Models.Pokemon DeserializePokemon(string s)
        {
            return JsonConvert.DeserializeObject<Models.Pokemon>(s, _settings);
        }
    }
}