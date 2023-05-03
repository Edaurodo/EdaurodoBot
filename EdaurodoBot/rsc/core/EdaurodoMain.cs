using DSharpPlus;
using DSharpPlus.Interactivity;
using DSharpPlus.SlashCommands;
using DSharpPlus.Interactivity.Extensions;
using Microsoft.Extensions.DependencyInjection;
using EdaurodoBot.rsc.modules.genericmodule.commands.info;
using EdaurodoBot.rsc.modules.allowlistmodule.services;
using EdaurodoBot.rsc.modules.genericmodule.commands.create;
using EdaurodoBot.rsc.modules.musicmodule.services;
using EdaurodoBot.rsc.core.config;

namespace EdaurodoBot.rsc.core
{
    public sealed class EdaurodoMain
    {
        public ConfigEdaurodo Config { get; }
        public DiscordClient Client { get; private set; }
        public InteractivityExtension? InteractivityExtension { get; private set; }
        public SlashCommandsExtension? SlashCommands { get; set; }
        public IServiceProvider Services { get; private set; }

        public EdaurodoMain(ConfigEdaurodo config)
        {
            Config = config;

            Client = new DiscordClient(new DiscordConfiguration()
            {
                Token = Config.Discord.Token,
                MessageCacheSize = Config.Discord.MessageCacheSize,
                LogTimestampFormat = "dd/MM/yyyy - HH:mm:ss",
                MinimumLogLevel = Config.MinimumLogLevel,
                Intents = DiscordIntents.All,
            });

            Services = new ServiceCollection()
                .AddSingleton(new AllowlistService(this))
                .AddSingleton(new LavalinkService(this.Client, Config.Music.Lavalink))
                .AddSingleton(new YoutubeSearchProvider(Config.Music.YoutubeApi.ApiKey))
                .BuildServiceProvider();


            SlashCommands = Client.UseSlashCommands(new SlashCommandsConfiguration() { Services = Services });

            InteractivityExtension = Client.UseInteractivity(new InteractivityConfiguration()
            {
                Timeout = TimeSpan.FromMinutes(3)
            });

            SlashCommands.RegisterCommands<CreateCommand>(Config.SecretConfig.GuildId);
            SlashCommands.RegisterCommands<InfoCommand>(Config.SecretConfig.GuildId);

        }
        public async Task<Task> StartAsync()
        {
            await Client.ConnectAsync();
            return Task.CompletedTask;
        }
    }
}
