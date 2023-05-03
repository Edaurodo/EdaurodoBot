using Newtonsoft.Json;

namespace EdaurodoBot.rsc.modules.musicmodule.config
{
    public sealed class ConfigMusic
    {
        [JsonProperty("use")]
        public bool Use { get; }

        [JsonProperty("lavalink")]
        public ConfigLavalink Lavalink { get; }

        [JsonProperty("youtube")]
        public ConfigYouTubeApi YoutubeApi { get; }

        public ConfigMusic(bool? use, ConfigLavalink? lavalink, ConfigYouTubeApi? youtube)
        {
            this.Use = use ?? false;
            this.Lavalink = lavalink ?? new ConfigLavalink(null, null, null);
            this.YoutubeApi = youtube ?? new ConfigYouTubeApi(null);
        }
    }
}