using DSharpPlus.Entities;
using System.Text;
using System.Text.RegularExpressions;

namespace EdaurodoBot.rsc.utils
{
    public sealed class EdaurodoUtilities
    {
        public static UTF8Encoding UTF8 { get; } = new UTF8Encoding(false);

        public static DiscordEmbed DiscordEmbedParse(EdaurodoEmbed embed)
        {
            /*
            DiscordEmbedBuilder eb = new DiscordEmbedBuilder();

            eb.WithColor(new DiscordColor(embed.Color));
            eb.WithDescription(embed.Description);
            eb.WithThumbnail(embed.Thumbnail);
            eb.WithImageUrl(embed.Image);
            eb.WithAuthor(embed.Author.Name, embed.Author.Url, embed.Author.Image);
            eb.WithTitle(embed.Title.Value);
            eb.WithUrl(embed.Title.Url);
            eb.WithFooter(embed.Footer.Value, embed.Footer.Image);
            if (embed.Footer.Timestamp ?? false) { eb.WithTimestamp(DateTime.Now.ToLocalTime()); }
            if (embed.Fields.Count() > 0)
            {
                foreach (var field in embed.Fields)
                {
                    eb.AddField(field.Title, field.Value, field.Inline ?? false);
                }
            }
            return eb.Build();
            */

            DiscordEmbedBuilder eb = new DiscordEmbedBuilder();

            eb.WithColor(new DiscordColor(embed.Color));
            _ = embed.Author.Name is null && embed.Author.Image is null ? null : eb.WithAuthor(embed.Author.Name, embed.Author.Url, embed.Author.Image);
            _ = embed.Title.Value is null ? null : eb.WithTitle(embed.Title.Value);
            _ = embed.Title.Url is null ? null : eb.WithUrl(embed.Title.Url);
            _ = embed.Description is null ? null : eb.WithDescription(embed.Description);
            _ = embed.Thumbnail is null ? null : eb.WithThumbnail(embed.Thumbnail);
            _ = embed.Image is null ? null : eb.WithImageUrl(embed.Image);
            _ = embed.Footer.Value is null && embed.Footer.Image is null ? null : eb.WithFooter(embed.Footer.Value, embed.Footer.Image);
            _ = embed.Footer.Timestamp is null || embed.Footer.Timestamp is false ? null : eb.WithTimestamp(DateTime.Now.ToLocalTime());

            if (embed.Fields.Count() > 0)
            {
                foreach (var field in embed.Fields)
                {
                    eb.AddField(field.Title, field.Value, field.Inline ?? false);
                }
            }
            return eb.Build();
        }
        public static bool ValidateColorHex(string hex)
        {
            if (hex.Length > 5 && hex.Length < 8)
            {
                if (Regex.IsMatch(hex, "[#][a-fA-F0-9]{6}|[a-fA-F0-9]{6}")) { return true; }
                else { return false; }
            }
            else { return false; }
        }
    }
}