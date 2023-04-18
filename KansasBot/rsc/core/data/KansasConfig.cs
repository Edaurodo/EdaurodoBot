using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Immutable;

namespace KansasBot.rsc.core.data
{
    public sealed class KansasConfig
    {
        [JsonProperty("loglevel")]
        public LogLevel MinimumLogLevel { get; private set; } = LogLevel.Debug;

        [JsonProperty("discord")]
        public KansasConfigDiscord Discord { get; private set; } = new KansasConfigDiscord();
    }

    public sealed class KansasConfigDiscord
    {
        [JsonProperty("token")]
        public string Token { get; private set; } = "insert your bot token here";

        [JsonProperty("prefixes")]
        public ImmutableArray<string> DefaultPrefixes { get; private set; } = new[] { "!", ">" }.ToImmutableArray();

        [JsonProperty("message_cache_size")]
        public int MessageCacheSize { get; private set; } = 512;

        [JsonProperty("shards")]
        public int ShardCount { get; private set; } = 1;

        [JsonProperty("use_interactivity")]
        public bool UseInteractivity { get; private set; } = true;

        [JsonProperty("use_commandnext")]
        public bool UseCommandNext { get; private set; } = false;

        [JsonProperty("mention_prefix")]
        public bool EnableMentionPrefix { get; private set; } = false;
    }
}
