using DSharpPlus;
using DSharpPlus.Interactivity;
using DSharpPlus.SlashCommands;
using DSharpPlus.Interactivity.Extensions;
using Microsoft.Extensions.DependencyInjection;
using EdaurodoBot.rsc.modules.genericmodule.commands.info;
using EdaurodoBot.rsc.modules.allowlistmodule.services;
using EdaurodoBot.rsc.core.data;
using EdaurodoBot.rsc.modules.genericmodule.commands.create;

namespace EdaurodoBot.rsc.core
{
    public sealed class EdaurodoMain
    {
        public EdaurodoConfig Config { get; }
        public DiscordClient Client { get; }
        public InteractivityExtension? InteractivityExtension { get; private set; }
        public SlashCommandsExtension? SlashCommands { get; set; }
        public IServiceProvider Services { get; private set; }

        public EdaurodoMain(EdaurodoConfig config)
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
                .BuildServiceProvider();

            SlashCommands = Client.UseSlashCommands(new SlashCommandsConfiguration() { Services = Services });

            InteractivityExtension = Client.UseInteractivity(new InteractivityConfiguration()
            {
                Timeout = TimeSpan.FromMinutes(3)
            });

            SlashCommands.RegisterCommands<CreateCommand>(Config.InternalConfig.GuildId);
            SlashCommands.RegisterCommands<InfoCommand>(Config.InternalConfig.GuildId);

        }
        public async Task<Task> StartAsync()
        {
            await Client.ConnectAsync();
            return Task.CompletedTask;
        }
    }
}
