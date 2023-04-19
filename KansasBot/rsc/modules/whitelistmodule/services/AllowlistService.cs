using DSharpPlus;
using DSharpPlus.EventArgs;
using KansasBot.rsc.core;
using KansasBot.rsc.modules.whitelistmodule.commands;
using KansasBot.rsc.modules.whitelistmodule.config;
using Microsoft.Extensions.Logging;

namespace KansasBot.rsc.modules.whitelistmodule.services
{
    public sealed class AllowlistService
    {
        public KansasMain Bot { get; set; }
        public AllowListConfig? Config { get; private set; }
        public AllowListConfigLoader? ConfigLoader { get; private set; }

        public AllowlistService(KansasMain bot)
        {
            Bot = bot;
            Bot.Client.Ready += Kansas_Ready;
        }

        private Task Component_Interaction_Created(DiscordClient c, ComponentInteractionCreateEventArgs s)
        {
            _ = Task.Run(async () =>
            {
                switch (s.Id)
                {
                    case "btn_startwl":
                        await s.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DSharpPlus.Entities.DiscordInteractionResponseBuilder().WithContent("oi").AsEphemeral(true));
                        break;
                }
            });
            return Task.CompletedTask;
        }
        private async Task Kansas_Ready(DiscordClient sender, ReadyEventArgs args)
        {
            Console.WriteLine("\n DISPAROU O KANSAS_READY \n");
            ConfigLoader = new AllowListConfigLoader(Bot.Client);
            Config = await ConfigLoader.LoadConfigAsync();
            if (!await ConfigLoader.ValidateConfig(Config))
            {
                Bot.Client.Logger.LogWarning(new EventId(700, "AllowListService"), "Não foi possível inicializar a AllowlistModule, verifique os arquivos de configuração.");
            }
            else
            {
                Bot.SlashCommands.RegisterCommands<AllowlistCommands>();
                Bot.SlashCommands.RefreshCommands();
                Bot.Client.ComponentInteractionCreated += Component_Interaction_Created;
            }
        }
    }
}
