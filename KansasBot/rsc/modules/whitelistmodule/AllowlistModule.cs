using DSharpPlus;
using DSharpPlus.EventArgs;
using KansasBot.rsc.core;
using KansasBot.rsc.modules.whitelistmodule.config;
using Microsoft.Extensions.Logging;

namespace KansasBot.rsc.modules.whitelistmodule
{
    public sealed class AllowlistModule
    {
        private KansasMain Bot;
        private AllowListConfig? Config;
        private AllowListConfigLoader? ConfigLoader;
        public AllowlistModule(KansasMain bot) {

            Bot = bot;
            Bot.Client.Ready += Kansas_Ready;
        }

        private Task Component_Interaction_Created(DiscordClient client, ComponentInteractionCreateEventArgs sender)
        {
            _ = Task.Run(async () =>
            {
                
            });
            return Task.CompletedTask;
        }
        private Task Kansas_Ready(DiscordClient sender, ReadyEventArgs args)
        {
            _ = Task.Run(async () => { 
                ConfigLoader = new AllowListConfigLoader();
                Config = await ConfigLoader.LoadConfigAsync();

                if (!await ConfigLoader.ValidateConfig(Config)) { Bot.Client.Logger.LogWarning("Não foi possível inicializar a AllowListModule, verifique os arquivos de configuração."); }
                else { Bot.Client.ComponentInteractionCreated += Component_Interaction_Created; }
            });
            return Task.CompletedTask;
        }
    }
}
