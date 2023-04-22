using DSharpPlus.Entities;
using KansasBot.rsc.core;
using KansasBot.rsc.modules.genericmodule.commands.create.embed;
using Microsoft.Extensions.Logging;
using System.Text;

namespace KansasBot.rsc.utils
{
    public sealed class KansasUtilities
    {
        public static UTF8Encoding UTF8 { get; } = new UTF8Encoding(false);

        public static DiscordEmbed GetEmbedFromJson(Embed embed, KansasMain Application)
        {
            DiscordEmbedBuilder eb = new DiscordEmbedBuilder();

            if (!string.IsNullOrWhiteSpace(embed.Color))
            {
                if (ValidateColorHex(embed.Color)) { eb.WithColor(new DiscordColor(embed.Color)); }
                else
                {
                    Application.Client.Logger.LogError(new EventId(777, "Utilities"), "ColorHex Inválida: Não foi possível adicionar 'embed.Color', verifique os arquivos de configuração");
                    eb.WithColor(new DiscordColor("#2B2D31"));
                }
            }
            else { eb.WithColor(new DiscordColor("#2B2D31")); }

            if (!string.IsNullOrWhiteSpace(embed.Description))
            {
                if (embed.Description.Length > 4096)
                {
                    Application.Client.Logger.LogError(new EventId(777, "Utilities"), $"Caracters limite: Não foi possível adicionar 'embed.Description', número de caracteres não pode exceder 4096 verifique os arquivos de configuração");
                }
                else { eb.WithDescription(embed.Description); }
            }

            if (!string.IsNullOrWhiteSpace(embed.Thumbnail))
            {
                if (Uri.TryCreate(embed.Thumbnail, UriKind.Absolute, out var url)) { eb.WithThumbnail(url.OriginalString); }
                else { Application.Client.Logger.LogError(new EventId(777, "Utilities"), "URL Inválida: Não foi possível adicionar 'embed.Thumbnail', verifique os arquivos de configuração"); }
            }

            if (!string.IsNullOrWhiteSpace(embed.Image))
            {
                if (Uri.TryCreate(embed.Image, UriKind.Absolute, out var url)) { eb.WithImageUrl(url.OriginalString); }
                else { Application.Client.Logger.LogError(new EventId(777, "Utilities"), "URL Inválida: Não foi possível adicionar 'embed.Image', verifique os arquivos de configuração"); }
            }

            if (embed.Author != null)
            {
                if (!string.IsNullOrWhiteSpace(embed.Author.Name) || !string.IsNullOrWhiteSpace(embed.Image))
                {
                    if (!string.IsNullOrWhiteSpace(embed.Author.Url) && !string.IsNullOrWhiteSpace(embed.Author.Name))
                    {
                        if (!string.IsNullOrWhiteSpace(embed.Author.Image))
                        {
                            if (Uri.TryCreate(embed.Author.Url, UriKind.Absolute, out var nameUrl) && Uri.TryCreate(embed.Author.Image, UriKind.Absolute, out var iconUrl))
                            {
                                if (embed.Author.Name.Length > 256)
                                {
                                    eb.WithAuthor(null, null, iconUrl.OriginalString);
                                    Application.Client.Logger.LogError(new EventId(777, "Utilities"), $"Caracters limite: Não foi possível adicionar 'embed.Author.Name', número de caracteres não pode exceder 256 verifique os arquivos de configuração");
                                }
                                else { eb.WithAuthor(embed.Author.Name, nameUrl.OriginalString, iconUrl.OriginalString); }
                            }
                            else if (Uri.TryCreate(embed.Author.Url, UriKind.Absolute, out nameUrl))
                            {
                                if (embed.Author.Name.Length > 256)
                                {
                                    Application.Client.Logger.LogError(new EventId(777, "Utilities"), $"Caracters limite: Não foi possível adicionar 'embed.Author.Name', número de caracteres não pode exceder 256 verifique os arquivos de configuração");
                                }
                                else
                                {
                                    Application.Client.Logger.LogError(new EventId(777, "Utilities"), "URL Inválida: Não foi possível adicionar 'embed.Author.Image', verifique os arquivos de configuração");
                                    eb.WithAuthor(embed.Author.Name, nameUrl.OriginalString, null);
                                }
                            }
                            else if (Uri.TryCreate(embed.Author.Image, UriKind.Absolute, out iconUrl))
                            {
                                if (embed.Author.Name.Length > 256)
                                {
                                    Application.Client.Logger.LogError(new EventId(777, "Utilities"), $"Caracters limite: Não foi possível adicionar 'embed.Author.Name', número de caracteres não pode exceder 256 verifique os arquivos de configuração");
                                    eb.WithAuthor(null, null, iconUrl.OriginalString);
                                }
                                else
                                {
                                    Application.Client.Logger.LogError(new EventId(777, "Utilities"), "URL Inválida: Não foi possível adicionar 'embed.Author.Url' ao 'embed.Author.Name', verifique os arquivos de configuração");
                                    eb.WithAuthor(embed.Author.Name, null, iconUrl.OriginalString);
                                }
                            }
                            else
                            {
                                if (embed.Author.Name.Length > 256)
                                {
                                    Application.Client.Logger.LogError(new EventId(777, "Utilities"), $"Caracters limite: Não foi possível adicionar 'embed.Author.Name', número de caracteres não pode exceder 256 verifique os arquivos de configuração");
                                }
                                else
                                {
                                    Application.Client.Logger.LogError(new EventId(777, "Utilities"), "URL Inválida: Não foi possível adicionar 'embed.Author.Url' ao 'embed.Author.Name' e 'Author.Image', verifique os arquivos de configuração");
                                    eb.WithAuthor(embed.Author.Name, null, null);
                                }
                            }
                        }
                        else
                        {
                            if (Uri.TryCreate(embed.Author.Url, UriKind.Absolute, out var nameUrl))
                            {
                                if (embed.Author.Name.Length > 256)
                                {
                                    Application.Client.Logger.LogError(new EventId(777, "Utilities"), $"Caracters limite: Não foi possível adicionar 'embed.Author.Name', número de caracteres não pode exceder 256 verifique os arquivos de configuração");
                                }
                                else { eb.WithAuthor(embed.Author.Name, nameUrl.OriginalString, null); }
                            }
                            else
                            {
                                if (embed.Author.Name.Length > 256)
                                {
                                    Application.Client.Logger.LogError(new EventId(777, "Utilities"), $"Caracters limite: Não foi possível adicionar 'embed.Author.Name', número de caracteres não pode exceder 256 verifique os arquivos de configuração");
                                }
                                else
                                {
                                    Application.Client.Logger.LogError(new EventId(777, "Utilities"), "URL Inválida: Não foi possível adicionar 'embed.Author.Url' ao 'embed.Author.Name', verifique os arquivos de configuração");
                                    eb.WithAuthor(embed.Author.Name, null, null);
                                }
                            }
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(embed.Author.Name))
                    {
                        if (!string.IsNullOrWhiteSpace(embed.Author.Image))
                        {
                            if (Uri.TryCreate(embed.Author.Image, UriKind.Absolute, out var iconUrl))
                            {
                                if (embed.Author.Name.Length > 256)
                                {
                                    Application.Client.Logger.LogError(new EventId(777, "Utilities"), $"Caracters limite: Não foi possível adicionar 'embed.Author.Name', número de caracteres não pode exceder 256 verifique os arquivos de configuração");
                                    eb.WithAuthor(null, null, iconUrl.OriginalString);
                                }
                                else
                                {
                                    eb.WithAuthor(embed.Author.Name, null, iconUrl.OriginalString);
                                }
                            }
                            else
                            {
                                if (embed.Author.Name.Length > 256)
                                {
                                    Application.Client.Logger.LogError(new EventId(777, "Utilities"), $"Caracters limite: Não foi possível adicionar 'embed.Author.Name', número de caracteres não pode exceder 256 verifique os arquivos de configuração");
                                }
                                else
                                {
                                    Application.Client.Logger.LogError(new EventId(777, "Utilities"), "URL Inválida: Não foi possível adicionar 'embed.Author.Image', verifique os arquivos de configuração");
                                    eb.WithAuthor(embed.Author.Name, null, null);
                                }
                            }
                        }
                        else
                        {
                            if (embed.Author.Name.Length > 256)
                            {
                                Application.Client.Logger.LogError(new EventId(777, "Utilities"), $"Caracters limite: Não foi possível adicionar 'embed.Author.Name', número de caracteres não pode exceder 256 verifique os arquivos de configuração");
                            }
                            else
                            {
                                eb.WithAuthor(embed.Author.Name, null, null);
                            }
                        }
                    }
                    else
                    {
                        if (Uri.TryCreate(embed.Author.Image, UriKind.Absolute, out var iconUrl))
                        {
                            eb.WithAuthor(embed.Author.Name, null, iconUrl.OriginalString);
                        }
                        else
                        {
                            Application.Client.Logger.LogError(new EventId(777, "Utilities"), "URL Inválida: Não foi possível adicionar 'embed.Author.Image', verifique os arquivos de configuração");
                        }
                    }
                }
            }

            if (embed.Title != null)
            {
                if (!string.IsNullOrWhiteSpace(embed.Title.Text))
                {
                    if (embed.Title.Text.Length > 256)
                    {
                        Application.Client.Logger.LogError(new EventId(777, "Utilities"), $"Caracters limite: Não foi possível adicionar 'embed.Title.Text', número de caracteres não pode exceder 256 verifique os arquivos de configuração");
                    }
                    else
                    {
                        eb.WithTitle(embed.Title.Text);
                        if (!string.IsNullOrWhiteSpace(embed.Title.Url))
                        {
                            if (Uri.TryCreate(embed.Title.Url, UriKind.Absolute, out var url)) { eb.WithUrl(url.OriginalString); }
                            else { Application.Client.Logger.LogError(new EventId(777, "Utilities"), "URL Inválida: Não foi adicionar 'embed.Title.Url', verifique os arquivos de configuração"); }
                        }
                    }
                }
            }

            if (embed.Footer != null)
            {
                if (!string.IsNullOrWhiteSpace(embed.Footer.Text) || !string.IsNullOrWhiteSpace(embed.Footer.Image))
                {
                    if (!string.IsNullOrWhiteSpace(embed.Footer.Text))
                    {
                        if (!string.IsNullOrWhiteSpace(embed.Footer.Image))
                        {
                            if (embed.Footer.Text.Length > 2048)
                            {
                                Application.Client.Logger.LogError(new EventId(777, "Utilities"), $"Caracters limite: Não foi possível adicionar 'embed.Footer.Text', número de caracteres não pode exceder 2048 verifique os arquivos de configuração");
                                if (Uri.TryCreate(embed.Footer.Image, UriKind.Absolute, out var url))
                                {
                                    eb.WithFooter(null, url.OriginalString);
                                }
                                else { Application.Client.Logger.LogError(new EventId(777, "Utilities"), "URL Inválida: Não foi adicionar 'embed.Footer.Image', verifique os arquivos de configuração"); }
                            }
                            else
                            {
                                if (Uri.TryCreate(embed.Footer.Image, UriKind.Absolute, out var url))
                                {
                                    eb.WithFooter(embed.Footer.Text, url.OriginalString);
                                }
                                else
                                {
                                    eb.WithFooter(embed.Footer.Text, null);
                                    Application.Client.Logger.LogError(new EventId(777, "Utilities"), "URL Inválida: Não foi adicionar 'embed.Footer.Image', verifique os arquivos de configuração");
                                }
                            }
                        }
                    }
                    else
                    {
                        if (Uri.TryCreate(embed.Footer.Image, UriKind.Absolute, out var url)) { eb.WithFooter(null, url.OriginalString); }
                        else { Application.Client.Logger.LogError(new EventId(777, "Utilities"), "URL Inválida: Não foi adicionar 'embed.Footer.Image', verifique os arquivos de configuração"); }
                    }
                }

                if (embed.Footer.Timestamp != null && embed.Footer.Timestamp == true) { eb.WithTimestamp(DateTime.Now.ToUniversalTime()); }
            }

            if (embed.Fields != null)
            {
                foreach (var field in embed.Fields)
                {
                    if (!string.IsNullOrWhiteSpace(field.Title))
                    {
                        if (field.Title.Length > 0)
                        {
                            
                        } else
                        {
                            if (!string.IsNullOrWhiteSpace(field.Content))
                            {
                                if (field.Content.Length > 0)
                                {

                                }
                                else
                                {

                                }
                            }
                        }
                    }
                }
            }

            return eb.Build();
        }

        private static bool ValidateColorHex(string hex)
        {
            if (hex.Length < 6 || hex.Length > 7) { return false; }
            hex = hex.ToUpper();
            char[] hexChars = { '#', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', 'A', 'B', 'C', 'D', 'E', 'F' };
            char[] hexarray = hex.ToArray();

            if (hex.Length == 6)
            {
                if (hexarray[0] == '#') { return false; };
                for (int i = 0; i < hexarray.Length; i++)
                {
                    if (!hexChars.Contains(hexarray[i])) { return false; }
                }
            }
            else
            {
                if (hexarray[0] != '#') { return false; };
                for (int i = 0; i < hexarray.Length; i++)
                {
                    if (!hexChars.Contains(hexarray[i])) { return false; }
                }
            }
            return true;
        }
    }
}
