using EdaurodoBot.rsc.modules.musicmodule.config;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace EdaurodoBot.rsc.core.config
{
    public sealed class ConfigEdaurodo
    {
        // 0 - Trace | 1 - Debug | 2 - Information | 3 - Warning | 4 - Error | 5 - Critical | 6 - None
        [JsonProperty("loglevel")]
        public LogLevel MinimumLogLevel { get; }
        [JsonProperty("discord")]
        public ConfigDiscord? Discord { get; }
        [JsonProperty("music")]
        public ConfigMusic? Music { get; }
        
        [JsonIgnore]
        public ConfigSecret SecretConfig { get; }

        public ConfigEdaurodo(LogLevel? logLevel, ConfigDiscord? discord, ConfigMusic? music)
        {
            MinimumLogLevel = logLevel ?? LogLevel.Information;
            Discord = discord ?? new ConfigDiscord(null, null);
            Music = music ?? new ConfigMusic(null, null, null);
            SecretConfig = new ConfigSecret();
        }

        public override string ToString()
        {
            return $"{MinimumLogLevel}\n\n" +
                $"{Discord.Token}\n{Discord.MessageCacheSize}\n\n" +
                $"{Music.Use}\n\n" +
                $"{Music.Lavalink.Hostname}\n{Music.Lavalink.Port}\n{Music.Lavalink.Password}\n\n" +
                $"{Music.YoutubeApi.ApiKey}\n";
        }
    }
}
