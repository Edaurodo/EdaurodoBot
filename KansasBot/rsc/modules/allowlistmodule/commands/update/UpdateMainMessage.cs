using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using KansasBot.rsc.modules.genericmodule.commands.create.embed;
using KansasBot.rsc.modules.allowlistmodule.services;
using KansasBot.rsc.utils;
using Newtonsoft.Json;

namespace KansasBot.rsc.modules.allowlistmodule.commands.update
{
    public sealed class UpdateMainMessage
    {
        private InteractionContext Context;
        private AllowlistService Module;
        private Embed _embed;
        public UpdateMainMessage(InteractionContext context, AllowlistService service)
        {
            Context = context;
            Module = service;
            _embed = Module.Config.Messages.MainMessage.Embed;
        }
        public async Task ExecuteAsync()
        {
            await Context.DeferAsync(true);
            await UpdateMessage();
            await Context.DeleteResponseAsync();
        }
        private async Task UpdateMessage()
        {
            var channel = Context.Guild.GetChannel((ulong)Module.Config.Channels.MainId);
            if (channel != null)
            {
                await channel.SendMessageAsync(new DiscordMessageBuilder().AddEmbed(KansasUtilities.GetEmbedFromJson(_embed, Module.Bot)).AddComponents(GetButtons()));
            }
        }
        private IEnumerable<DiscordActionRowComponent> GetButtons()
        {
            List<DiscordComponent> button_line = new List<DiscordComponent>() {
            new DiscordLinkButtonComponent(Module.Config.Messages.MainMessage.ButtonLink, "Leias as Regras", false, null),
            new DiscordButtonComponent(ButtonStyle.Success, "btn_AlStart", "Iniciar Allowlist", false, null)
            };

            List<DiscordActionRowComponent> components_lines = new List<DiscordActionRowComponent>() {
            new DiscordActionRowComponent(button_line)
            };

            return components_lines;
        }
    }
}
