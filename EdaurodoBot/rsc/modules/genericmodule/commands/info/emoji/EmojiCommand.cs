using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus;

namespace EdaurodoBot.rsc.modules.genericmodule.commands.info.emoji
{
    public sealed class EmojiCommand
    {
        private InteractionContext Context;
        private DiscordEmoji? Emoji;
        private ulong EmojiId;
        private string EmojiString;
        public EmojiCommand(InteractionContext ctx, string emoji)
        {
            Context = ctx;
            EmojiString = emoji;
            Emoji = null;
        }
        public async Task ExecuteAsync()
        {
            if (ulong.TryParse(EmojiString.Substring(EmojiString.LastIndexOf(':') + 1).Replace('>', ' '), out EmojiId))
            {
                if (DiscordEmoji.TryFromGuildEmote(Context.Client, EmojiId, out Emoji))
                {
                    await GuildEmojiInfo();
                }
                else
                {
                    await Context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent(
                    "> Me desculpe eu não consegui encontrar este emoji.\n" +
                    "> Provavelmente eu não estou no servidor que contem este emoji.").AsEphemeral(true));
                }
            }
            else
            {
                await Context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent(
                "> Este comando só obtêm informações de emojis personalizado de Guildas.\n" +
                "> Tente novamente com um emoji válido!").AsEphemeral(true));
            }
        }
        private async Task GuildEmojiInfo()
        {
            DiscordEmbedBuilder eb = new DiscordEmbedBuilder();
            eb.WithColor(new DiscordColor("#2B2D31"));
            eb.WithThumbnail(Emoji.Url);
            eb.WithTitle(Emoji.Name);
            eb.AddField("> <:identifier:1096379975235555368> ulong ID", $"```{Emoji.Id}```", false);
            eb.AddField("> <:mention:1096400745655451780> Menção", $"```{Emoji}```", false);
            if (Emoji.IsAnimated) { eb.AddField("> <:gif:1096400744124514434> Tipo", "Animado", true); }
            else { eb.AddField("> <:gif:1096400744124514434> Tipo", "Estático", true); }
            eb.AddField("> <:calendar:1096400748079742999> Criação", $"{Emoji.CreationTimestamp.ToString("dd")} de {Emoji.CreationTimestamp.ToString("MMMM")} de {Emoji.CreationTimestamp.ToString("yyyy")} às {Emoji.CreationTimestamp.ToString("hh:mm")}", true);
            await Context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(eb.Build()).AddComponents(new[] { new DiscordLinkButtonComponent(Emoji.Url, "Abrir no navegador") }).AsEphemeral(true));
        }
    }
}
