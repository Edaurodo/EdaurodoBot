using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus;
using Newtonsoft.Json;
using EdaurodoBot.rsc.utils;

namespace EdaurodoBot.rsc.modules.genericmodule.commands.create.embed
{
    public sealed class EmbedCommand
    {

        private InteractionContext Context;
        private DiscordClient Client;
        private DiscordInteraction Interaction;
        private DiscordChannel Channel;
        private DiscordMember Member;

        private EdaurodoEmbed _embed;
        private int FieldIndex;
        private EdaurodoEmbedField? FieldSelect;
        private List<EdaurodoEmbedField>? _fieldList;

        public EmbedCommand(InteractionContext ctx)
        {
            Context = ctx;
            Client = ctx.Client;
            Channel = ctx.Channel;
            Member = ctx.Member;

            _embed = new EdaurodoEmbed(null, "### > * Lembre-se que seu Embed não pode ser vazio!\nPersonalize abaixo!", null, null, null, null, null, null);

            FieldIndex = 0;
            FieldSelect = null;
            _fieldList = new List<EdaurodoEmbedField>();
            SubmittedModal();
            CaptureButtons();
            CaptureSelectMenu();
        }
        public async Task ExecuteAsync()
        {
            await Context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(GetEmbed()).AddComponents(GetMainMenuComponents()).AsEphemeral(true));
        }
        private Task CaptureButtons()
        {
            Client.ComponentInteractionCreated += (c, s) =>
            {
                _ = Task.Run(async () =>
                {
                    if (s.Id.StartsWith(Context.Interaction.Id.ToString()))
                    {
                        if (s.Id.Contains("btn_goback"))
                        {
                            Interaction = s.Interaction;
                            FieldIndex = 0;
                            FieldSelect = null;
                            await UpdateMessageAsync();
                        }
                        else if (s.Id.Contains("btn_color"))
                        {
                            Interaction = s.Interaction;
                            await UpdateMessagaColorPannel();
                        }
                        else if (s.Id.Contains("btn_author"))
                        {
                            Interaction = s.Interaction;
                            await AuthorModal();
                        }
                        else if (s.Id.Contains("btn_addimage"))
                        {
                            Interaction = s.Interaction;
                            await ImageModal();
                        }
                        else if (s.Id.Contains("btn_title"))
                        {
                            Interaction = s.Interaction;
                            await TitleModal();
                        }
                        else if (s.Id.Contains("btn_description"))
                        {
                            Interaction = s.Interaction;
                            await DescriptionModal();
                        }
                        else if (s.Id.Contains("btn_field"))
                        {
                            Interaction = s.Interaction;
                            FieldIndex = 0;
                            FieldSelect = null;
                            await UpdateMessagaFieldPannel();
                        }
                        else if (s.Id.Contains("btn_addfield"))
                        {
                            Interaction = s.Interaction;
                            FieldIndex = 0;
                            FieldSelect = null;
                            await FieldModal();
                        }
                        else if (s.Id.Contains("btn_deletefield"))
                        {
                            Interaction = s.Interaction;
                            await FieldDelete();
                        }
                        else if (s.Id.Contains("btn_editfield"))
                        {
                            Interaction = s.Interaction;
                            await FieldModal();
                        }
                        else if (s.Id.Contains("btn_footer"))
                        {
                            Interaction = s.Interaction;
                            await FooterModal();

                        }
                        else if (s.Id.Contains("btn_jsonup"))
                        {
                            Interaction = s.Interaction;
                            await JsonModal();
                        }
                        else if (s.Id.Contains("btn_jsondown"))
                        {
                            Interaction = s.Interaction;
                            await ExportJson();
                        }
                        else if (s.Id.Contains("btn_sendembed"))
                        {
                            Interaction = s.Interaction;
                            await Channel.SendMessageAsync(new DiscordMessageBuilder().WithEmbed(GetEmbed()));
                            await UpdateMessageAsync();
                        }
                    }
                });
                return Task.CompletedTask;
            };
            return Task.CompletedTask;
        }
        private Task CaptureSelectMenu()
        {
            Client.ComponentInteractionCreated += (c, s) =>
            {
                _ = Task.Run(async () =>
                {
                    if (s.Id.StartsWith(Context.Interaction.Id.ToString()))
                    {
                        if (s.Id.Contains("select_colors"))
                        {
                            Interaction = s.Interaction;
                            if (s.Values[0] != "select_colorhex") { _embed.Color = s.Values[0]; await UpdateMessagaColorPannel(); }
                            else { await ColorModal(); }
                        }
                        else if (s.Id.Contains("select_field"))
                        {
                            Interaction = s.Interaction;
                            FieldIndex = int.Parse(s.Values[0]);
                            FieldSelect = _fieldList[FieldIndex];
                            await UpdateMessagaFieldPannel();
                        }
                    }
                });
                return Task.CompletedTask;
            };
            return Task.CompletedTask;
        }
        private Task SubmittedModal()
        {
            Client.ModalSubmitted += (c, s) =>
            {
                _ = Task.Run(async () =>
                {

                    if (s.Interaction.Data.CustomId.StartsWith(Context.Interaction.Id.ToString()))
                    {
                        string customid = s.Interaction.Data.CustomId;
                        if (customid.Contains("modal_color"))
                        {
                            Interaction = s.Interaction;
                            s.Values.TryGetValue("form_color", out string? color);
                            await ColorUpdate(color);
                        }
                        else if (customid.Contains("modal_author"))
                        {
                            Interaction = s.Interaction;
                            s.Values.TryGetValue("form_author", out string? author);
                            s.Values.TryGetValue("form_authorimage", out string? authorimage);
                            s.Values.TryGetValue("form_authorurl", out string? authorurl);
                            await AuthorUpdate(author, authorimage, authorurl);
                        }
                        else if (customid.Contains("modal_title"))
                        {
                            Interaction = s.Interaction;
                            s.Values.TryGetValue("form_title", out string? title);
                            s.Values.TryGetValue("form_titleurl", out string? titleurl);
                            await TitleUpdate(title, titleurl);
                        }
                        else if (customid.Contains("modal_description"))
                        {
                            Interaction = s.Interaction;
                            s.Values.TryGetValue("form_description", out string? description);
                            await DescriptionUpdate(description);
                        }
                        else if (customid.Contains("modal_image"))
                        {
                            Interaction = s.Interaction;
                            s.Values.TryGetValue("form_image", out string? image);
                            s.Values.TryGetValue("form_thumbnail", out string? thumbnail);
                            await ImageUpdate(image, thumbnail);
                        }
                        else if (customid.Contains("modal_footer"))
                        {
                            Interaction = s.Interaction;
                            s.Values.TryGetValue("form_footer", out string? footer);
                            s.Values.TryGetValue("form_footerimage", out string? footerimage);
                            s.Values.TryGetValue("form_footertimestamp", out string? footertimestamp);
                            await FooterUpdate(footer, footerimage, footertimestamp);
                        }
                        else if (customid.Contains("modal_field"))
                        {
                            Interaction = s.Interaction;
                            s.Values.TryGetValue("form_title", out string? fieldtitle);
                            s.Values.TryGetValue("form_content", out string? fieldcontent);
                            s.Values.TryGetValue("form_inline", out string? fieldinline);
                            await FieldUpdate(fieldtitle, fieldcontent, fieldinline);
                        }
                        else if (customid.Contains("modal_json"))
                        {
                            Interaction = s.Interaction;
                            s.Values.TryGetValue("form_json", out string? json);
                            await JsonUpdate(json);
                        }
                    }
                });
                return Task.CompletedTask;
            };
            return Task.CompletedTask;
        }
        private async Task JsonModal()
        {
            await Interaction.CreateResponseAsync(InteractionResponseType.Modal, new DiscordInteractionResponseBuilder().
                   WithCustomId($"{Context.Interaction.Id}-modal_json").
                   WithTitle("Importe um Embed a partir de um json").
                   AddComponents(new TextInputComponent("JSON", "form_json", "Cole aqui o JSON de seu Embed", null, false, TextInputStyle.Paragraph, 0, 4000)));
        }
        private async Task JsonUpdate(string? json)
        {
            _embed = JsonConvert.DeserializeObject<EdaurodoEmbed>(json);

            await UpdateMessageAsync();
        }
        private async Task FieldModal()
        {
            if (FieldSelect == null)
            {
                await Interaction.CreateResponseAsync(InteractionResponseType.Modal, new DiscordInteractionResponseBuilder().
                        WithCustomId($"{Context.Interaction.Id}-modal_field").
                        WithTitle("Configure seu Field.").
                        AddComponents(new TextInputComponent("Titulo", "form_title", "Titulo do field, pode ser usado formatação", null, true, TextInputStyle.Short, 0, 256)).
                        AddComponents(new TextInputComponent("Conteudo", "form_content", "Conteudo do field, pode ser usado formatação", null, true, TextInputStyle.Paragraph, 0, 1024)).
                        AddComponents(new TextInputComponent("Em linha ?", "form_inline", "Use (SIM/TRUE) OU (NÃO/FALSE)", "false", true, TextInputStyle.Short, 0, 10)));
            }
            else
            {
                await Interaction.CreateResponseAsync(InteractionResponseType.Modal, new DiscordInteractionResponseBuilder().
                        WithCustomId($"{Context.Interaction.Id}-modal_field").
                        WithTitle("Configure seu Field.").
                        AddComponents(new TextInputComponent("Titulo", "form_title", "Titulo do field, pode ser usado formatação", FieldSelect.Title, true, TextInputStyle.Short, 0, 256)).
                        AddComponents(new TextInputComponent("Conteudo", "form_content", "Conteudo do field, pode ser usado formatação", FieldSelect.Value, true, TextInputStyle.Paragraph, 0, 1024)).
                        AddComponents(new TextInputComponent("Conteudo", "form_inline", "Use (SIM/TRUE) OU (NÃO/FALSE)", FieldSelect.Inline.ToString().ToLower(), true, TextInputStyle.Short, 0, 10))); ;
            }
        }
        private async Task FieldUpdate(string? fieldtitle, string? fieldcontent, string? fieldinline)
        {
            if (FieldSelect == null)
            {
                if (!string.IsNullOrEmpty(fieldtitle) && !string.IsNullOrEmpty(fieldcontent) && !string.IsNullOrEmpty(fieldinline))
                {
                    string x = fieldinline.ToLower();
                    bool inline;
                    if (x == "sim" || x == "true") { inline = true; }
                    else { inline = false; }

                    if (inline) { if (_fieldList.Count > 1) { _fieldList[_fieldList.Count - 1].Inline = inline; } }

                    _fieldList.Add(new EdaurodoEmbedField(fieldtitle, fieldcontent, inline));
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(fieldtitle) && !string.IsNullOrEmpty(fieldcontent) && !string.IsNullOrEmpty(fieldinline))
                {
                    string x = fieldinline.ToLower();
                    bool inline;
                    if (x == "sim" || x == "true") { inline = true; }
                    else { inline = false; }

                    _fieldList[FieldIndex].Title = fieldtitle;
                    _fieldList[FieldIndex].Value = fieldcontent;

                    if (FieldIndex > 0)
                    {
                        if (inline)
                        {
                            _fieldList[FieldIndex].Inline = inline;
                            _fieldList[FieldIndex - 1].Inline = inline;
                        }
                        else { _fieldList[FieldIndex].Inline = inline; }
                    }
                    else { _fieldList[FieldIndex].Inline = inline; }
                }
                FieldSelect = null;
            }
            await UpdateMessagaFieldPannel();
        }
        private async Task FieldDelete()
        {
            _fieldList.RemoveAt(FieldIndex);
            FieldSelect = null;
            FieldIndex = 0;
            await UpdateMessagaFieldPannel();
        }
        private async Task ColorModal()
        {
            await Interaction.CreateResponseAsync(InteractionResponseType.Modal, new DiscordInteractionResponseBuilder().
                    WithCustomId($"{Context.Interaction.Id}-modal_color").
                    WithTitle("Altere a cor do seu Embed").
                    AddComponents(new TextInputComponent("Cor", "form_color", "Envia a cor em formato HEX: (ex: #000000)", _embed.Color, false, TextInputStyle.Short, 6, 7)));
        }
        private async Task ColorUpdate(string? color)
        {
            if (EdaurodoUtilities.ValidateColorHex(color))
            {
                _embed.Color = color;
                await UpdateMessagaColorPannel();
            }
            else
            {
                await Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Seu código HEX é invalido tente novamente").AsEphemeral(true));
            }
        }
        private async Task FooterModal()
        {
            await Interaction.CreateResponseAsync(InteractionResponseType.Modal, new DiscordInteractionResponseBuilder().
                    WithCustomId($"{Context.Interaction.Id}-modal_footer").
                    WithTitle("Personalize o rodapé do seu Embed").
                    AddComponents(new TextInputComponent("Rodapé Texto", "form_footer", "(Opcional) Escreva sua assunatura ou tente @eu ou @bot", _embed.Footer.Value, false, TextInputStyle.Short, 0, 2048)).
                    AddComponents(new TextInputComponent("Rodapé Imagem", "form_footerimage", "(Opcional) cole um link direto da imagem ou tenet @eu ou @bot", _embed.Footer.Image, false, TextInputStyle.Short)).
                    AddComponents(new TextInputComponent("Rodapé Horário", "form_footertimestamp", "Use (SIM/TRUE) OU (NÃO/FALSE)", _embed.Footer.Timestamp.ToString(), false, TextInputStyle.Short)));
        }
        private async Task FooterUpdate(string? footer, string? footerimage, string? footertimestamp)
        {
            if (!string.IsNullOrEmpty(footer))
            {
                if (footer.ToLower() == "@eu") { _embed.Footer.Value = Member.DisplayName; }
                else if (footer.ToLower() == "@bot") { _embed.Footer.Value = Client.CurrentUser.Username; }
                else { _embed.Footer.Value = footer; }
            }
            else { _embed.Footer.Value = null; }
            if (!string.IsNullOrEmpty(footerimage))
            {
                if (footerimage.ToLower() == "@eu") { _embed.Footer.Image = Member.AvatarUrl; }
                else if (footerimage.ToLower() == "@bot") { _embed.Footer.Image = Client.CurrentUser.AvatarUrl; }
                else
                {
                    if (Uri.TryCreate(footerimage, UriKind.Absolute, out var uri)) { _embed.Footer.Image = uri.OriginalString; }
                    else { await Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Não consegui atualizar o rodapé do Embed, verifique se a URL é válida").AsEphemeral(true)); throw new Exception(); }
                }
            }
            else { _embed.Footer.Image = null; }

            if (!string.IsNullOrEmpty(footertimestamp))
            {
                string x = footertimestamp.ToLower();
                if (x == "sim" || x == "true") { _embed.Footer.Timestamp = true; }
                else { _embed.Footer.Timestamp = false; }
            }
            else { _embed.Footer.Timestamp = false; }
            await UpdateMessageAsync();
        }
        private async Task ImageModal()
        {
            await Interaction.CreateResponseAsync(InteractionResponseType.Modal, new DiscordInteractionResponseBuilder().
                                WithCustomId($"{Context.Interaction.Id}-modal_image").
                                WithTitle("Personalize as imagens do seu Embed").
                                AddComponents(new TextInputComponent("Imagem a baixo dos fields", "form_image", "(Opcional) cole o link direto da imagem.", _embed.Image, false, TextInputStyle.Short, 0, 256)).
                                AddComponents(new TextInputComponent("Imagem ao lado da descrição", "form_thumbnail", "(Opcional) cole o link direto da imagem.", _embed.Thumbnail, false, TextInputStyle.Short)));
        }
        private async Task ImageUpdate(string? image, string? thumbnail)
        {
            if (!string.IsNullOrEmpty(image))
            {
                if (Uri.TryCreate(image, UriKind.Absolute, out Uri? uri)) { _embed.Image = uri.OriginalString; }
                else { await Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Não consegui atualizar as imagens do Embed, verifique se as URL's é válida").AsEphemeral(true)); throw new Exception(); }
            }
            else { _embed.Image = null; }
            if (!string.IsNullOrEmpty(thumbnail))
            {
                if (Uri.TryCreate(thumbnail, UriKind.Absolute, out Uri? uri)) { _embed.Thumbnail = uri.OriginalString; }
                else { await Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Não consegui atualizar as imagens do Embed, verifique se as URL's é válida").AsEphemeral(true)); }
            }
            else { _embed.Thumbnail = null; }
            await UpdateMessageAsync();
        }
        private async Task AuthorModal()
        {
            await Interaction.CreateResponseAsync(InteractionResponseType.Modal, new DiscordInteractionResponseBuilder().
                    WithCustomId($"{Context.Interaction.Id}-modal_author").
                    WithTitle("Personalize o autor do seu Embed").
                    AddComponents(new TextInputComponent("Nome do autor", "form_author", "(Opcional) mas lebre-se que seu embed não pode ser vazio. Tente @eu ou @bot", _embed.Author.Name, false, TextInputStyle.Short, 0, 256)).
                    AddComponents(new TextInputComponent("Imagem do autor", "form_authorimage", "(Opcional) cole o link do avatar do author. Tente @eu ou @bot", _embed.Author.Image, false, TextInputStyle.Short)).
                    AddComponents(new TextInputComponent("Url do autor", "form_authorurl", "(Opcional) cole um link para para o redirecionamento.", _embed.Author.Url, false, TextInputStyle.Short)));
        }
        private async Task AuthorUpdate(string? author, string? authorimage, string? authorurl)
        {
            if (!string.IsNullOrEmpty(author))
            {
                if (author.ToLower() == "@eu") { _embed.Author.Name = Member.DisplayName; }
                else if (author.ToLower() == "@bot") { _embed.Author.Name = Client.CurrentUser.Username; }
                else { _embed.Author.Name = author; }
            }
            else { _embed.Author.Name = null; }

            if (!string.IsNullOrEmpty(authorimage))
            {
                if (authorimage.ToLower() == "@eu") { _embed.Author.Image = Member.AvatarUrl; }
                else if (authorimage.ToLower() == "@bot") { _embed.Author.Image = Client.CurrentUser.AvatarUrl; }
                else
                {
                    if (Uri.TryCreate(authorimage, UriKind.Absolute, out var uri)) { _embed.Author.Image = uri.OriginalString; }
                    else { await Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Não consegui atualizar as informações do author, verifique se as URL's é válida").AsEphemeral(true)); throw new Exception(); }
                }
            }
            else { _embed.Author.Image = null; }

            if (!string.IsNullOrEmpty(authorurl))
            {
                if (Uri.TryCreate(authorurl, UriKind.Absolute, out var uri)) { _embed.Author.Url = uri.OriginalString; }
                else { await Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Não consegui atualizar as informações do author, verifique se as URL's é válida").AsEphemeral(true)); throw new Exception(); }
            }
            else { _embed.Author.Url = null; }
            await UpdateMessageAsync();
        }
        private async Task TitleModal()
        {
            await Interaction.CreateResponseAsync(InteractionResponseType.Modal, new DiscordInteractionResponseBuilder().
                    WithCustomId($"{Context.Interaction.Id}-modal_title").
                    WithTitle("Personalize o título do seu Embed").
                    AddComponents(new TextInputComponent("Título", "form_title", "(Opcional) mas lebre-se que seu embed não pode ser vazio", _embed.Title.Value, false, TextInputStyle.Short, 0, 2048)).
                    AddComponents(new TextInputComponent("Título URL", "form_titleurl", "(Opcional) cole um link para para o redirecionamento", _embed.Title.Url, false, TextInputStyle.Short)));
        }
        private async Task TitleUpdate(string? title, string? url)
        {
            if (!string.IsNullOrEmpty(title)) { _embed.Title.Value = title; }
            else { _embed.Title.Value = null; }

            if (!string.IsNullOrEmpty(url))
            {
                if (Uri.TryCreate(url, UriKind.Absolute, out var uri)) { _embed.Title.Url = uri.OriginalString; }
                else { await Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Não consegui atualizar as informações do seu titulo, verifique se a URL é válida").AsEphemeral(true)); throw new Exception(); }
            }
            else { _embed.Title.Url = null; }
            await UpdateMessageAsync();
        }
        private async Task DescriptionModal()
        {
            await Interaction.CreateResponseAsync(InteractionResponseType.Modal, new DiscordInteractionResponseBuilder().
                    WithCustomId($"{Context.Interaction.Id}-modal_description").
                    WithTitle("Personalize a descrição do seu Embed").
                    AddComponents(new TextInputComponent("Descrição", "form_description", "(Opcional) mas lebre-se que seu embed não pode ser vazio", _embed.Description, false, TextInputStyle.Paragraph, 0, 4000)));
        }
        private async Task DescriptionUpdate(string? description)
        {
            if (!string.IsNullOrEmpty(description)) { _embed.Description = description; }
            else { _embed.Description = null; }
            await UpdateMessageAsync();
        }

        private async Task ExportJson()
        {
            await Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(new DiscordEmbedBuilder().WithDescription($"```{JsonConvert.SerializeObject(new EdaurodoEmbedSerializable(_embed))}```").WithColor(new DiscordColor("#2B2D31"))).AsEphemeral(true));
        }
        private async Task UpdateMessageAsync()
        {
            await Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().AddEmbed(GetEmbed()).AddComponents(GetMainMenuComponents()));
        }
        private async Task UpdateMessagaColorPannel()
        {
            await Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().AddEmbed(GetEmbed()).AddComponents(GetColorMenuComponents()));
        }
        private async Task UpdateMessagaFieldPannel()
        {
            await Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().AddEmbed(GetEmbed()).AddComponents(GetFieldMenuComponents()));
        }
        private DiscordEmbed GetEmbed()
        {
            DiscordEmbedBuilder eb = new DiscordEmbedBuilder();

            eb.WithColor(new DiscordColor(_embed.Color));

            if (_embed.Image != null) { eb.WithImageUrl(_embed.Image); }

            if (_embed.Thumbnail != null) { eb.WithThumbnail(_embed.Thumbnail); }

            if (_embed.Author.Name != null || _embed.Author.Image != null) { eb.WithAuthor(_embed.Author.Name, _embed.Author.Url, _embed.Author.Image); }

            if (_embed.Title.Value != null) { eb.WithTitle(_embed.Title.Value); }

            if (_embed.Title.Value != null && _embed.Title.Url != null) { eb.WithUrl(_embed.Title.Url); }

            if (_embed.Description != null) { eb.WithDescription(_embed.Description); }

            if (_embed.Footer.Value != null || _embed.Footer.Image != null) { eb.WithFooter(_embed.Footer.Value, _embed.Footer.Image); }

            if (_embed.Footer.Timestamp ?? false) { eb.WithTimestamp(DateTime.Now.ToLocalTime()); }

            if (_fieldList.Count > 0)
            {
                foreach (var field in _fieldList)
                {
                    eb.AddField(field.Title, field.Value, field.Inline ?? false);
                }
            }
            return eb.Build();
        }
        private IEnumerable<DiscordActionRowComponent> GetMainMenuComponents()
        {
            List<DiscordActionRowComponent> btn_panel = new List<DiscordActionRowComponent>();

            #region UpLine of buttons
            List<DiscordComponent> btn_upline = new List<DiscordComponent>();
            DiscordComponentEmoji emoji_color = new DiscordComponentEmoji(DiscordEmoji.FromName(Client, ":art:"));
            btn_upline.Add(new DiscordButtonComponent(ButtonStyle.Secondary, $"{Context.Interaction.Id}-btn_color", "Cor", false, emoji_color));
            DiscordComponentEmoji emoji_author = new DiscordComponentEmoji(DiscordEmoji.FromName(Client, ":astronaut:"));
            btn_upline.Add(new DiscordButtonComponent(ButtonStyle.Secondary, $"{Context.Interaction.Id}-btn_author", "Autor", false, emoji_author));
            DiscordComponentEmoji emoji_addimage = new DiscordComponentEmoji(DiscordEmoji.FromName(Client, ":frame_photo:"));
            btn_upline.Add(new DiscordButtonComponent(ButtonStyle.Secondary, $"{Context.Interaction.Id}-btn_addimage", "Imagem e Thumbnail", false, emoji_addimage));
            btn_panel.Add(new DiscordActionRowComponent(btn_upline));
            #endregion

            #region MidLine of buttons
            List<DiscordComponent> btn_midline = new List<DiscordComponent>();
            DiscordComponentEmoji emoji_title = new DiscordComponentEmoji(DiscordEmoji.FromName(Client, ":page_facing_up:"));
            btn_midline.Add(new DiscordButtonComponent(ButtonStyle.Secondary, $"{Context.Interaction.Id}-btn_title", "Título", false, emoji_title));
            DiscordComponentEmoji emoji_description = new DiscordComponentEmoji(DiscordEmoji.FromName(Client, ":pencil:"));
            btn_midline.Add(new DiscordButtonComponent(ButtonStyle.Secondary, $"{Context.Interaction.Id}-btn_description", "Descrição", false, emoji_description));
            DiscordComponentEmoji emoji_field = new DiscordComponentEmoji(DiscordEmoji.FromName(Client, ":pen_fountain:"));
            btn_midline.Add(new DiscordButtonComponent(ButtonStyle.Secondary, $"{Context.Interaction.Id}-btn_field", "Fields", false, emoji_field));
            DiscordComponentEmoji emoji_footer = new DiscordComponentEmoji(DiscordEmoji.FromName(Client, ":triangular_flag_on_post:"));
            btn_midline.Add(new DiscordButtonComponent(ButtonStyle.Secondary, $"{Context.Interaction.Id}-btn_footer", "Footer", false, emoji_footer));
            btn_panel.Add(new DiscordActionRowComponent(btn_midline));
            #endregion

            #region DownLine of buttons
            List<DiscordComponent> btn_downline = new List<DiscordComponent>();
            DiscordComponentEmoji emoji_download = new DiscordComponentEmoji(DiscordEmoji.FromName(Client, ":arrow_down:"));
            DiscordComponentEmoji emoji_upload = new DiscordComponentEmoji(DiscordEmoji.FromName(Client, ":arrow_up:"));
            DiscordComponentEmoji emoji_send = new DiscordComponentEmoji(DiscordEmoji.FromName(Client, ":incoming_envelope:"));
            btn_downline.Add(new DiscordButtonComponent(ButtonStyle.Primary, $"{Context.Interaction.Id}-btn_jsonup", "Importar JSON", false, emoji_upload));
            btn_downline.Add(new DiscordButtonComponent(ButtonStyle.Primary, $"{Context.Interaction.Id}-btn_jsondown", "Exportar JSON", false, emoji_download));
            btn_downline.Add(new DiscordButtonComponent(ButtonStyle.Success, $"{Context.Interaction.Id}-btn_sendembed", "Enviar aqui", false, emoji_send));
            btn_panel.Add(new DiscordActionRowComponent(btn_downline));
            #endregion

            return btn_panel;
        }
        private IEnumerable<DiscordActionRowComponent> GetColorMenuComponents()
        {
            List<DiscordSelectComponentOption> selectOptions = new List<DiscordSelectComponentOption>();
            selectOptions.Clear();
            DiscordComponentEmoji emoji_add = new DiscordComponentEmoji(DiscordEmoji.FromName(Client, ":heavy_plus_sign:"));
            selectOptions.Add(new DiscordSelectComponentOption("Adicione um código HEX", "select_colorhex", null, false, emoji_add));
            foreach (var color in EdaurodoEmbed.Colors)
            {
                selectOptions.Add(new DiscordSelectComponentOption(color.Value, color.Key, null, false, null));
            }

            List<DiscordComponent> select_line = new List<DiscordComponent>() {
                new DiscordSelectComponent($"{Context.Interaction.Id}-select_colors", "Escolha uma cor para seu Embed", selectOptions, false, 1, 1)
            };

            DiscordComponentEmoji emoji_goback = new DiscordComponentEmoji(DiscordEmoji.FromName(Client, ":back:"));
            List<DiscordComponent> button_line = new List<DiscordComponent>() {
            new DiscordButtonComponent(ButtonStyle.Secondary, $"{Context.Interaction.Id}-btn_goback", null, false, emoji_goback),
            new DiscordLinkButtonComponent("https://htmlcolorcodes.com/", "Encontre novas cores", false, null)
            };

            List<DiscordActionRowComponent> components_lines = new List<DiscordActionRowComponent>() {
            new DiscordActionRowComponent(select_line),
            new DiscordActionRowComponent(button_line)
            };

            return components_lines;
        }
        private IEnumerable<DiscordActionRowComponent> GetFieldMenuComponents()
        {
            List<DiscordActionRowComponent> components_lines = new List<DiscordActionRowComponent>();

            if (_fieldList.Count > 0)
            {
                DiscordComponentEmoji emoji_edit = new DiscordComponentEmoji(DiscordEmoji.FromName(Client, ":pen_fountain:"));
                DiscordComponentEmoji emoji_delete = new DiscordComponentEmoji(DiscordEmoji.FromName(Client, ":recycle:"));
                List<DiscordComponent> button_upline = new List<DiscordComponent>() {

                    new DiscordButtonComponent(ButtonStyle.Secondary, $"{Context.Interaction.Id}-btn_editfield", "Editar", FieldSelect == null, emoji_edit),
                    new DiscordButtonComponent(ButtonStyle.Secondary, $"{Context.Interaction.Id}-btn_deletefield", "Excluir", FieldSelect == null, emoji_delete)
                };
                if (FieldSelect != null)
                {
                    string fieldTitle = FieldSelect.Title; ;
                    if (fieldTitle.Length > 25) { fieldTitle = fieldTitle.Substring(0, 25); }
                    button_upline.Add(new DiscordButtonComponent(ButtonStyle.Secondary, "...", $"\"{fieldTitle}\" Selecionado", true, null));
                }
                else { button_upline.Add(new DiscordButtonComponent(ButtonStyle.Secondary, "...", "Nenhum field selecionado", true, null)); }
                components_lines.Add(new DiscordActionRowComponent(button_upline));

                List<DiscordSelectComponentOption> selectOptions = new List<DiscordSelectComponentOption>();
                selectOptions.Clear();

                int i = 1;
                foreach (var field in _fieldList)
                {
                    string title = field.Title;
                    string content = field.Value;
                    if (title.Length > 50) { title = title.Substring(0, 50); }
                    if (content.Length > 50) { content = content.Substring(0, 50); }
                    selectOptions.Add(new DiscordSelectComponentOption($"{i}. {title}", $"{i - 1}", content, false, null));
                    i++;
                }

                List<DiscordComponent> select_line = new List<DiscordComponent>() {
                new DiscordSelectComponent($"{Context.Interaction.Id}-select_field", "Escolha um field para editar", selectOptions, false, 1, 1)
                };
                components_lines.Add(new DiscordActionRowComponent(select_line));
            }

            DiscordComponentEmoji emoji_goback = new DiscordComponentEmoji(DiscordEmoji.FromName(Client, ":back:"));
            DiscordComponentEmoji emoji_add = new DiscordComponentEmoji(DiscordEmoji.FromName(Client, ":heavy_plus_sign:"));
            List<DiscordComponent> button_downline = new List<DiscordComponent>() {
                new DiscordButtonComponent(ButtonStyle.Secondary, $"{Context.Interaction.Id}-btn_goback", null, false, emoji_goback),
                new DiscordButtonComponent(ButtonStyle.Success, $"{Context.Interaction.Id}-btn_addfield", "Novo field", !(_fieldList.Count < 25), emoji_add)
            };
            components_lines.Add(new DiscordActionRowComponent(button_downline));

            return components_lines;
        }
    }
}