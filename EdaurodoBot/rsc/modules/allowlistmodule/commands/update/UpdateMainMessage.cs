using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using EdaurodoBot.rsc.modules.allowlistmodule.services;
using EdaurodoBot.rsc.utils;

namespace EdaurodoBot.rsc.modules.allowlistmodule.commands.update
{
    public sealed class UpdateMainMessage
    {
        private InteractionContext _context;
        private AllowlistService _module;
        private EdaurodoEmbed _embed;
        public UpdateMainMessage(InteractionContext context, AllowlistService service)
        {
            _context = context;
            _module = service;
            _embed = _module.Config.Messages.MainMessage.Embed;
        }
        public async Task ExecuteAsync()
        {
            await _context.DeferAsync(true);
            await UpdateMessage();
            await _context.DeleteResponseAsync();
        }
        private async Task UpdateMessage()
        {
            var channel = _context.Guild.GetChannel((ulong)_module.Config.Channels.MainId);
            if (channel != null)
            {
                await channel.SendMessageAsync(new DiscordMessageBuilder().AddEmbed(EdaurodoUtilities.GetEmbedFromJson(_embed)).AddComponents(GetButtons()));
            }
        }
        private IEnumerable<DiscordActionRowComponent> GetButtons()
        {
            List<DiscordComponent> button_line = new List<DiscordComponent>() {
            new DiscordLinkButtonComponent(_module.Config.Messages.MainMessage.ButtonLink, "Leias as Regras", false, null),
            new DiscordButtonComponent(ButtonStyle.Success, "btn_AlStart", "Iniciar Allowlist", false, null)
            };
            List<DiscordActionRowComponent> components_lines = new List<DiscordActionRowComponent>() {
            new DiscordActionRowComponent(button_line)
            };
            return components_lines;
        }
    }
}
