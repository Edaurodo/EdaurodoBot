using Newtonsoft.Json;

namespace EdaurodoBot.rsc.core.config
{
    public sealed class ConfigDiscord
    {
        [JsonProperty("token")]
        public string Token { get; }

        [JsonProperty("message_cache_size")]
        public int MessageCacheSize { get; }

        public ConfigDiscord(string? token, int? messagecachesize)
        {
            Token = string.IsNullOrWhiteSpace(token) ? "insert your bot token here" : token;
            MessageCacheSize = messagecachesize ?? 512;
        }
    }
}
