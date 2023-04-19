using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using KansasBot.rsc.modules.genericmodule.commands.create.embed;
using KansasBot.rsc.modules.whitelistmodule.services;
using Newtonsoft.Json;

namespace KansasBot.rsc.modules.whitelistmodule.commands.commandclass
{
    public sealed class UpdateWlMessage
    {
        private InteractionContext Context;
        private AllowlistService Module;
        private Embed _embed;
        public UpdateWlMessage(InteractionContext context, AllowlistService service)
        {
            Context = context;
            Module = service;
            _embed = new Embed()
            {
                Title = new EmbedTitle(),
                Author = new EmbedAuthor(),
                Footer = new EmbedFooter(),
                Fields = new[] { new EmbedField() }
            };
        }
        public async Task ExecuteAsync()
        {
            await Context.DeferAsync(true);
            await UpdateMessage();
            await Context.DeleteResponseAsync();
        }
        private async Task UpdateMessage()
        {
            var channel = Context.Guild.GetChannel((ulong)Module.Config.ChannelConfig.MainChannelId);
            if (channel != null)
            {
                string json = JsonConvert.SerializeObject(Module.Config.Messages.MainMessage.EmbedJson);
                UpdateEmbed(json);
                await channel.SendMessageAsync(new DiscordMessageBuilder().AddEmbed(GetEmbed()).AddComponents(GetButtons()));
            }
        }
        private async Task UpdateEmbed(string? json)
        {
            Embed tempEmbed = JsonConvert.DeserializeObject<Embed>(json);
            if (tempEmbed != null)
            {
                if (!string.IsNullOrEmpty(tempEmbed.Color)) { _embed.Color = tempEmbed.Color; } else { _embed.Color = "#2B2D31"; }
                if (tempEmbed.Author != null)
                {
                    if (!string.IsNullOrEmpty(tempEmbed.Author.Name)) { _embed.Author.Name = tempEmbed.Author.Name; } else { _embed.Author.Name = null; }
                    if (!string.IsNullOrEmpty(tempEmbed.Author.Url)) { _embed.Author.Url = tempEmbed.Author.Url; } else { _embed.Author.Url = null; }
                    if (!string.IsNullOrEmpty(tempEmbed.Author.Image)) { _embed.Author.Image = tempEmbed.Author.Image; } else { _embed.Author.Image = null; }
                }
                else { _embed.Author = new EmbedAuthor() { Name = null, Url = null, Image = null }; }
                if (tempEmbed.Title != null)
                {
                    if (!string.IsNullOrEmpty(tempEmbed.Title.Text)) { _embed.Title.Text = tempEmbed.Title.Text; } else { _embed.Title.Text = null; }
                    if (!string.IsNullOrEmpty(tempEmbed.Title.Url)) { _embed.Title.Url = tempEmbed.Title.Url; } else { _embed.Title.Url = null; }
                }
                if (!string.IsNullOrEmpty(tempEmbed.Description)) { _embed.Description = tempEmbed.Description; } else { _embed.Description = null; }
                if (!string.IsNullOrEmpty(tempEmbed.Thumbnail)) { _embed.Thumbnail = tempEmbed.Thumbnail; } else { _embed.Thumbnail = null; }
                if (!string.IsNullOrEmpty(tempEmbed.Image)) { _embed.Image = tempEmbed.Image; } else { _embed.Image = null; }

                if (tempEmbed.Footer != null)
                {
                    if (!string.IsNullOrEmpty(tempEmbed.Footer.Text) != null) { _embed.Footer.Text = tempEmbed.Footer.Text; } else { _embed.Footer.Text = null; }
                    if (!string.IsNullOrEmpty(tempEmbed.Footer.Image) != null) { _embed.Footer.Image = tempEmbed.Footer.Image; } else { _embed.Footer.Image = null; }
                    if (tempEmbed.Footer.Timestamp != null) { _embed.Footer.Timestamp = tempEmbed.Footer.Timestamp; }
                }
                else { _embed.Footer = new EmbedFooter() { Text = null, Image = null, Timestamp = false }; }

                if (tempEmbed.Fields != null) { _embed.Fields = tempEmbed.Fields; }
                else { _embed.Fields = null; }
            }
        }
        private IEnumerable<DiscordActionRowComponent> GetButtons()
        {
            List<DiscordComponent> button_line = new List<DiscordComponent>() {
            new DiscordLinkButtonComponent(Module.Config.Messages.MainMessage.RuleButtonLink, "REGRAS", false, null),
            new DiscordButtonComponent(ButtonStyle.Success, "btn_startwl", "INICIAR", false, null)
            };

            List<DiscordActionRowComponent> components_lines = new List<DiscordActionRowComponent>() {
            new DiscordActionRowComponent(button_line)
            };

            return components_lines;
        }
        private DiscordEmbed GetEmbed()
        {
            DiscordEmbedBuilder eb = new DiscordEmbedBuilder();
            eb.WithColor(new DiscordColor(_embed.Color));
            if (_embed.Image != null) { eb.WithImageUrl(_embed.Image); }
            if (_embed.Thumbnail != null) { eb.WithThumbnail(_embed.Thumbnail); }
            if (_embed.Author.Name != null || _embed.Author.Image != null) { eb.WithAuthor(_embed.Author.Name, _embed.Author.Url, _embed.Author.Image); }
            if (_embed.Title.Text != null) { eb.WithTitle(_embed.Title.Text); }
            if (_embed.Title.Text != null && _embed.Title.Url != null) { eb.WithUrl(_embed.Title.Url); }
            if (_embed.Description != null) { eb.WithDescription(_embed.Description); }
            if (_embed.Footer.Text != null || _embed.Footer.Image != null) { eb.WithFooter(_embed.Footer.Text, _embed.Footer.Image); }
            if (_embed.Footer.Timestamp ?? false) { eb.WithTimestamp(DateTime.Now.ToLocalTime()); }

            if (_embed.Fields.Count() > 0 && _embed.Fields != null)
            {
                foreach (var field in _embed.Fields)
                {
                    eb.AddField(field.Title, field.Content, field.Inline ?? false);
                }
            }
            return eb.Build();
        }
    }
}
