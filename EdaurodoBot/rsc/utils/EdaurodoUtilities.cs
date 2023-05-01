using DSharpPlus.Entities;
using EdaurodoBot.rsc.core;
using System.Text;
using System.Text.RegularExpressions;

namespace EdaurodoBot.rsc.utils
{
    public sealed class EdaurodoUtilities
    {
        public static UTF8Encoding UTF8 { get; } = new UTF8Encoding(false);

        public static DiscordEmbed GetEmbedFromJson(EdaurodoEmbed embed)
        {
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