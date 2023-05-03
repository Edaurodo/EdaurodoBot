using Newtonsoft.Json;

namespace EdaurodoBot.rsc.modules.musicmodule.config
{
    public sealed class ConfigLavalink
    {
        [JsonProperty("password")]
        public string Password { get; }

        [JsonProperty("hostname")]
        public string Hostname { get; }

        [JsonProperty("port")]
        public int Port { get; }

        public ConfigLavalink(string? password, string? hostname, int? port)
        {
            Password = string.IsNullOrWhiteSpace(password) ? "your_password_here" : password;
            Hostname = string.IsNullOrWhiteSpace(hostname) ? "localhost" : hostname;
            Port = port ?? 2333;
        }
    }
}
