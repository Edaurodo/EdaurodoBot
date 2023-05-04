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
        private ConfigMusic _config;
        public LavalinkNodeConnection Node { get; private set; }

        public LavalinkService(DiscordClient client, ConfigMusic config)
        {
            _client = client;
            _config = config;
            _client.Ready += Lavalink_Ready;
        }



        private Task Lavalink_Ready(DiscordClient sender, ReadyEventArgs args)
        {
            if (_config.Use)
            {
                _ = Task.Run(async () =>
                {
                    var lava = _client.UseLavalink();
                    Node = await lava.ConnectAsync(new LavalinkConfiguration()
                    {
                        Password = _config.Lavalink.Password,
                        RestEndpoint = new ConnectionEndpoint(_config.Lavalink.Hostname, _config.Lavalink.Port),
                        SocketEndpoint = new ConnectionEndpoint(_config.Lavalink.Hostname, _config.Lavalink.Port)
                    });
                });
            }
            return Task.CompletedTask;
        }
    }
}
