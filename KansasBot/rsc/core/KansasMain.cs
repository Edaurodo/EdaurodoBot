using DSharpPlus;
using DSharpPlus.Interactivity;
using DSharpPlus.SlashCommands;
using DSharpPlus.Interactivity.Extensions;
using KansasBot.rsc.core.data;
using KansasBot.rsc.modules.genericmodule.commands.info;
using KansasBot.rsc.modules.genericmodule.commands.create;
using Microsoft.Extensions.DependencyInjection;
using KansasBot.rsc.modules.whitelistmodule.commands;
using KansasBot.rsc.modules.whitelistmodule.services;

namespace KansasBot.rsc.core
{
    public sealed class KansasMain
    {
        public KansasConfig Config { get; }
        public DiscordClient Client { get; }
        public InteractivityExtension? InteractivityExtension { get; private set; }
        public SlashCommandsExtension? SlashCommands { get;  set; }
        public IServiceProvider Services { get; private set; }

        //public CommandsNextExtension CommandsNext { get; internal set;

        public AllowlistService Allowlist { get; private set; }
        public KansasMain(KansasConfig config)
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

            SlashCommands.RegisterCommands<CreateCommand>();
            SlashCommands.RegisterCommands<InfoCommand>();

        }
        public async Task<Task> StartAsync()
        {
            await Client.ConnectAsync();
            return Task.CompletedTask;
        }
    }
}
