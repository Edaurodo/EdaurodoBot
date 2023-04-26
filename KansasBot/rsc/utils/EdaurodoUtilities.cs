﻿using DSharpPlus.Entities;
using EdaurodoBot.rsc.core;
using EdaurodoBot.rsc.modules.genericmodule.commands.create.embed;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.RegularExpressions;

namespace EdaurodoBot.rsc.utils
{
    public sealed class EdaurodoUtilities
    {
        public static UTF8Encoding UTF8 { get; } = new UTF8Encoding(false);

        public static DiscordEmbed GetEmbedFromJson(Embed embed, EdaurodoMain Application)
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
                                    Application.Client.Logger.LogError(new EventId(777, "Utilities"), "URL Inválida: Não foi possível adicionar 'embed.Author.Url' ao 'embed.Author.Name' e 'embed.Author.Image', verifique os arquivos de configuração");
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
                if (embed.Fields.Length > 25)
                {
                    Application.Client.Logger.LogError(new EventId(777, "Utilities"), $"Execedeu limite de Field: Não foi possível adicionar 'embed.Fields', quantidade máxima não pode exceder 25 verifique os arquivos de configuração");
                }
                else
                {
                    int count = 0;
                    foreach (var field in embed.Fields)
                    {
                        if (!string.IsNullOrWhiteSpace(field.Title))
                        {
                            if (field.Title.Length > 256)
                            {
                                Application.Client.Logger.LogError(new EventId(777, "Utilities"), $"Caracters limite: Não foi possível adicionar 'embed.Fields[{count}].Title', número de caracteres não pode exceder 256 verifique os arquivos de configuração");
                            }
                            else
                            {
                                if (!string.IsNullOrWhiteSpace(field.Content))
                                {
                                    if (field.Content.Length > 1024)
                                    {
                                        Application.Client.Logger.LogError(new EventId(777, "Utilities"), $"Caracters limite: Não foi possível adicionar 'embed.Fields[{count}].Content', número de caracteres não pode exceder 1024 verifique os arquivos de configuração");
                                    }
                                    else
                                    {
                                        if (field.Inline != null && field.Inline == true) { eb.AddField(field.Title, field.Content, true); count++; }
                                        else { eb.AddField(field.Title, field.Content, false); count++; }
                                    }
                                }
                                else
                                {
                                    Application.Client.Logger.LogError(new EventId(777, "Utilities"), $"Campo null: Não foi possível adicionar 'embed.Fields[{count}].Content', o campo não pode ser nulo verifique os arquivos de configuração");
                                }
                            }
                        }
                        else
                        {
                            Application.Client.Logger.LogError(new EventId(777, "Utilities"), $"Campo null: Não foi possível adicionar 'embed.Fields[{count}].Title', o campo não pode ser nulo verifique os arquivos de configuração");
                        }
                    }
                }
            }

            return eb.Build();
        }

        private static bool ValidateColorHex(string hex)
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