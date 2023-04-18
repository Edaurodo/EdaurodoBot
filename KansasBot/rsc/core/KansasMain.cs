using DSharpPlus;
using DSharpPlus.Interactivity;
using DSharpPlus.SlashCommands;
using DSharpPlus.Interactivity.Extensions;
using KansasBot.rsc.core.data;
using KansasBot.rsc.modules.genericmodule.commands.info;
using KansasBot.rsc.modules.genericmodule.commands.create;
using KansasBot.rsc.modules.whitelistmodule;

namespace KansasBot.rsc.core
{
    public sealed class KansasMain
    {
        public KansasConfig Config { get; }
        public DiscordClient Client { get; }
        public InteractivityExtension? InteractivityExtension { get; internal set; }
        public SlashCommandsExtension? SlashCommands { get; internal set; }
        //public CommandsNextExtension CommandsNext { get; internal set;

        public AllowlistModule Allowlist { get; private set; }
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
            if (Config.Discord.UseInteractivity) { InitInteractivity(); }

            InitKansasModules();
        }

        private void InitKansasModules()
        {
            Allowlist = new AllowlistModule(this);
        }
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
