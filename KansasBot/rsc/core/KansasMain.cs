using DSharpPlus;
using DSharpPlus.Interactivity;
using DSharpPlus.SlashCommands;
using DSharpPlus.Interactivity.Extensions;
using KansasBot.rsc.core.data;
using KansasBot.rsc.modules.genericmodule.commands.info;
using KansasBot.rsc.modules.genericmodule.commands.create;

namespace KansasBot.rsc.core
{
    public sealed class KansasMain
    {
        public KansasConfig Config { get; }
        public DiscordClient Client { get; }
        public InteractivityExtension? InteractivityExtension { get; internal set; }
        public SlashCommandsExtension? SlashCommands { get; internal set; }

        //public IServiceProvider Services { get; }
        //public CommandsNextExtension CommandsNext { get; internal set; 
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

            //if (Config.Discord.UseCommandNext) { InitCommandsNext(); }
            if (Config.Discord.UseInteractivity) { InitInteractivity(); }
        }

        /*
        private void InitCommandsNext()
        {
            CommandsNext = Client.UseCommandsNext(new CommandsNextConfiguration()
            {
                CaseSensitive = true,
                EnableDms = false,
                DmHelp = false,
                EnableMentionPrefix = Config.Discord.EnableMentionPrefix,
                StringPrefixes = Config.Discord.DefaultPrefixes,
                Services = Services
            });
        }
        */

        private void InitInteractivity()
        {
            SlashCommands = Client.UseSlashCommands(new SlashCommandsConfiguration());
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
