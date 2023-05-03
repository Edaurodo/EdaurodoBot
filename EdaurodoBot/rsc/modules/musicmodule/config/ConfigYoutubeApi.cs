using Newtonsoft.Json;

namespace EdaurodoBot.rsc.modules.musicmodule.config
{
    public sealed class ConfigYouTubeApi
    {
        [JsonProperty("apikey")]
        public string ApiKey { get; }

        public ConfigYouTubeApi(string? apikey)
        {
            ApiKey = string.IsNullOrWhiteSpace(apikey) ? "insert your apikey here" : apikey;
        }
    }
}
