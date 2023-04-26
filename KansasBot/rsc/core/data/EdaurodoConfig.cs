using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace EdaurodoBot.rsc.core.data
{
    public sealed class EdaurodoConfig
    {
        // 0 - Trace | 1 - Debug | 2 - Information | 3 - Warning | 4 - Error | 5 - Critical | 6 - None
        [JsonProperty("loglevel")]
        public LogLevel MinimumLogLevel { get; private set; }

        [JsonProperty("discord")]
        public EdaurodoConfigDiscord Discord { get; private set; }

        public EdaurodoConfig() { 
        
            this.MinimumLogLevel = LogLevel.Information;
            this.Discord = new EdaurodoConfigDiscord();
        }
    }

    public sealed class EdaurodoConfigDiscord
    {
        [JsonProperty("token")]
        public string Token { get; private set; }

        [JsonProperty("message_cache_size")]
        public int MessageCacheSize { get; private set; }

        [JsonProperty("use_interactivity")]
        public bool UseInteractivity { get; private set; }

        public EdaurodoConfigDiscord()
        {
            this.Token = "insert your bot token here";
            this.MessageCacheSize = 512;
            this.UseInteractivity = true;
        }
    }
}
