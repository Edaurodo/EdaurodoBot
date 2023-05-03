using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.Lavalink;
using DSharpPlus.Net;
using EdaurodoBot.rsc.modules.musicmodule.config;

namespace EdaurodoBot.rsc.modules.musicmodule.services
{
    public sealed class LavalinkService
    {
        private DiscordClient _client;
        private ConfigLavalink _config;
        private LavalinkNodeConnection _connection;

        public LavalinkService(DiscordClient client, ConfigLavalink config)
        {
            _client = client;
            _config = config;
            _client.Ready += Lavalink_Ready;
        }

        private Task Lavalink_Ready(DiscordClient sender, ReadyEventArgs args)
        {
            _ = Task.Run(async () =>
            {
                var lava = _client.UseLavalink();
                _connection = await lava.ConnectAsync(new LavalinkConfiguration()
                {
                    Password = _config.Password,
                    RestEndpoint = new ConnectionEndpoint(_config.Hostname, _config.Port),
                    SocketEndpoint = new ConnectionEndpoint(_config.Hostname, _config.Port)
                });
            });

            return Task.CompletedTask;
        }
    }
}
