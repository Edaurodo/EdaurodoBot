using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus;
using Newtonsoft.Json;
using EdaurodoBot.rsc.utils;
using EdaurodoBot.rsc.exceptions;

namespace EdaurodoBot.rsc.modules.genericmodule.commands.create.embed
{
    public sealed class EmbedCommand
    {
        private InteractionContext _context;
        private DiscordClient _client;
        private DiscordInteraction _interaction;
        private DiscordChannel _channel;
        private DiscordMember _member;
        private EdaurodoEmbed _embed;
        private int _fieldIndex;
        public EmbedCommand(InteractionContext ctx)
        {
            _context = ctx;
            _client = ctx.Client;
            _channel = ctx.Channel;
            _member = ctx.Member;
            _fieldIndex = 0;
            _embed = new EdaurodoEmbed(null, "### > * Lembre-se que seu Embed não pode ser vazio!\nPersonalize abaixo!", null, null, null, null, null, null);
            SubmittedModal();
            CaptureButtons();
            CaptureSelectMenu();
        }
        public async Task ExecuteAsync()
        {
            await _context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(EdaurodoUtilities.DiscordEmbedParse(_embed)).AddComponents(GetMainMenuComponents()).AsEphemeral(true));
        }
        private Task CaptureButtons()
        {
            _client.ComponentInteractionCreated += (c, s) =>
            {
                _ = Task.Run(async () =>
                {
                    if (s.Id.StartsWith(_context.Interaction.Id.ToString()))
                    {
                        if (s.Id.Contains("btn_goback"))
                        {
                            _interaction = s.Interaction;
                            _fieldIndex = -1;
                            await UpdateMessageAsync();
                        }
                        else if (s.Id.Contains("btn_color"))
                        {
                            _interaction = s.Interaction;
                            await UpdateMessagaColorPannel();
                        }
                        else if (s.Id.Contains("btn_author"))
                        {
                            _interaction = s.Interaction;
                            await AuthorModal();
                        }
                        else if (s.Id.Contains("btn_addimage"))
                        {
                            _interaction = s.Interaction;
                            await ImageModal();
                        }
                        else if (s.Id.Contains("btn_title"))
                        {
                            _interaction = s.Interaction;
                            await TitleModal();
                        }
                        else if (s.Id.Contains("btn_description"))
                        {
                            _interaction = s.Interaction;
                            await DescriptionModal();
                        }
                        else if (s.Id.Contains("btn_field"))
                        {
                            _interaction = s.Interaction;
                            _fieldIndex = -1;
                            await UpdateMessagaFieldPannel();
                        }
                        else if (s.Id.Contains("btn_addfield"))
                        {
                            _interaction = s.Interaction;
                            _fieldIndex = -1;
                            await FieldModal();
                        }
                        else if (s.Id.Contains("btn_deletefield"))
                        {
                            _interaction = s.Interaction;
                            await FieldDelete();
                        }
                        else if (s.Id.Contains("btn_editfield"))
                        {
                            _interaction = s.Interaction;
                            await FieldModal();
                        }
                        else if (s.Id.Contains("btn_footer"))
                        {
                            _interaction = s.Interaction;
                            await FooterModal();

                        }
                        else if (s.Id.Contains("btn_jsonup"))
                        {
                            _interaction = s.Interaction;
                            await JsonModal();
                        }
                        else if (s.Id.Contains("btn_jsondown"))
                        {
                            _interaction = s.Interaction;
                            await ExportJson();
                        }
                        else if (s.Id.Contains("btn_sendembed"))
                        {
                            _interaction = s.Interaction;
                            await _channel.SendMessageAsync(new DiscordMessageBuilder().WithEmbed(EdaurodoUtilities.DiscordEmbedParse(_embed)));
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
            _client.ComponentInteractionCreated += (c, s) =>
            {
                _ = Task.Run(async () =>
                {
                    if (s.Id.StartsWith(_context.Interaction.Id.ToString()))
                    {
                        if (s.Id.Contains("select_colors"))
                        {
                            _interaction = s.Interaction;
                            if (s.Values[0] != "select_colorhex") { _embed.Color = s.Values[0]; await UpdateMessagaColorPannel(); }
                            else { await ColorModal(); }
                        }
                        else if (s.Id.Contains("select_field"))
                        {
                            _interaction = s.Interaction;
                            _fieldIndex = int.Parse(s.Values[0]);
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
            _client.ModalSubmitted += (c, s) =>
            {
                _ = Task.Run(async () =>
                {
                    if (s.Interaction.Data.CustomId.StartsWith(_context.Interaction.Id.ToString()))
                    {
                        string customid = s.Interaction.Data.CustomId;
                        if (customid.Contains("modal_color"))
                        {
                            _interaction = s.Interaction;
                            s.Values.TryGetValue("form_color", out string? color);
                            await ColorUpdate(color);
                        }
                        else if (customid.Contains("modal_author"))
                        {
                            _interaction = s.Interaction;
                            s.Values.TryGetValue("form_author", out string? author);
                            s.Values.TryGetValue("form_authorimage", out string? authorimage);
                            s.Values.TryGetValue("form_authorurl", out string? authorurl);
                            await AuthorUpdate(author, authorimage, authorurl);
                        }
                        else if (customid.Contains("modal_title"))
                        {
                            _interaction = s.Interaction;
                            s.Values.TryGetValue("form_title", out string? title);
                            s.Values.TryGetValue("form_titleurl", out string? titleurl);
                            await TitleUpdate(title, titleurl);
                        }
                        else if (customid.Contains("modal_description"))
                        {
                            _interaction = s.Interaction;
                            s.Values.TryGetValue("form_description", out string? description);
                            await DescriptionUpdate(description);
                        }
                        else if (customid.Contains("modal_image"))
                        {
                            _interaction = s.Interaction;
                            s.Values.TryGetValue("form_image", out string? image);
                            s.Values.TryGetValue("form_thumbnail", out string? thumbnail);
                            await ImageUpdate(image, thumbnail);
                        }
                        else if (customid.Contains("modal_footer"))
                        {
                            _interaction = s.Interaction;
                            s.Values.TryGetValue("form_footer", out string? footer);
                            s.Values.TryGetValue("form_footerimage", out string? footerimage);
                            s.Values.TryGetValue("form_footertimestamp", out string? footertimestamp);
                            await FooterUpdate(footer, footerimage, footertimestamp);
                        }
                        else if (customid.Contains("modal_field"))
                        {
                            _interaction = s.Interaction;
                            s.Values.TryGetValue("form_title", out string? fieldtitle);
                            s.Values.TryGetValue("form_content", out string? fieldcontent);
                            s.Values.TryGetValue("form_inline", out string? fieldinline);
                            await FieldUpdate(fieldtitle, fieldcontent, fieldinline);
                        }
                        else if (customid.Contains("modal_json"))
                        {
                            _interaction = s.Interaction;
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
            await _interaction.CreateResponseAsync(InteractionResponseType.Modal, new DiscordInteractionResponseBuilder().
                   WithCustomId($"{_context.Interaction.Id}-modal_json").
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
            if (_fieldIndex < 0)
            {
                await _interaction.CreateResponseAsync(InteractionResponseType.Modal, new DiscordInteractionResponseBuilder().
                        WithCustomId($"{_context.Interaction.Id}-modal_field").
                        WithTitle("Configure seu Field.").
                        AddComponents(new TextInputComponent("Titulo", "form_title", "Titulo do field, pode ser usado formatação", null, true, TextInputStyle.Short, 0, 256)).
                        AddComponents(new TextInputComponent("Conteudo", "form_content", "Conteudo do field, pode ser usado formatação", null, true, TextInputStyle.Paragraph, 0, 1024)).
                        AddComponents(new TextInputComponent("Em linha ?", "form_inline", "Use (SIM/TRUE) OU (NÃO/FALSE)", "false", true, TextInputStyle.Short, 0, 10)));
            }
            else
            {
                await _interaction.CreateResponseAsync(InteractionResponseType.Modal, new DiscordInteractionResponseBuilder().
                        WithCustomId($"{_context.Interaction.Id}-modal_field").
                        WithTitle("Configure seu Field.").
                        AddComponents(new TextInputComponent("Titulo", "form_title", "Titulo do field, pode ser usado formatação", _embed.Fields.ElementAt(_fieldIndex).Title, true, TextInputStyle.Short, 0, 256)).
                        AddComponents(new TextInputComponent("Conteudo", "form_content", "Conteudo do field, pode ser usado formatação", _embed.Fields.ElementAt(_fieldIndex).Value, true, TextInputStyle.Paragraph, 0, 1024)).
                        AddComponents(new TextInputComponent("Conteudo", "form_inline", "Use (SIM/TRUE) OU (NÃO/FALSE)", _embed.Fields.ElementAt(_fieldIndex).Inline.ToString().ToLower(), true, TextInputStyle.Short, 0, 10)));
            }
        }
        private Task FieldUpdate(string? title, string? value, string? inlinestring)
        {
            bool inline = inlinestring.ToLower() == "true" || inlinestring.ToLower() == "sim" ? true : false;
            _ = string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(value) ? throw new EdaurodoEmbedArgumentException("> * **Os campos do seu field não pode conter apenas espaçoes em brancos!**", _interaction) :
                _fieldIndex < 0 ?
                Task.Run(async () =>
                {
                    var temp = _embed.Fields.ToList();
                    temp.Add(new EdaurodoEmbedField(title, value, inline));
                    _embed.Fields = temp;
                    await UpdateMessagaFieldPannel();
                }) :
                Task.Run(async () =>
                {
                    var temp = _embed.Fields.ToList();
                    temp[_fieldIndex].Title = title;
                    temp[_fieldIndex].Value = value;
                    temp[_fieldIndex].Inline = inline;
                    if (_fieldIndex > 0 && inline) { temp[_fieldIndex - 1].Inline = inline; }
                    _embed.Fields = temp;
                    _fieldIndex = -1;
                    await UpdateMessagaFieldPannel();
                });
            return Task.CompletedTask;
        }
        private async Task FieldDelete()
        {
            var temp = _embed.Fields.ToList();
            temp.Remove(temp[_fieldIndex]);
            _embed.Fields = temp;
            _fieldIndex = -1;
            await UpdateMessagaFieldPannel();
        }
        private async Task ColorModal()
        {
            await _interaction.CreateResponseAsync(InteractionResponseType.Modal, new DiscordInteractionResponseBuilder().
                    WithCustomId($"{_context.Interaction.Id}-modal_color").
                    WithTitle("Altere a cor do seu Embed").
                    AddComponents(new TextInputComponent("Cor", "form_color", "Envia a cor em formato HEX: (ex: #000000)", _embed.Color, false, TextInputStyle.Short, 6, 7)));
        }
        private Task ColorUpdate(string? color)
        {
            _ = EdaurodoUtilities.ValidateColorHex(color) ?
                Task.Run(async () =>
                {
                    _embed.Color = color;
                    await UpdateMessagaColorPannel();
                }) : throw new EdaurodoEmbedArgumentException("> * **Seu código HEX é inválido tente novamente**", _interaction);
            return Task.CompletedTask;
        }
        private async Task FooterModal()
        {
            await _interaction.CreateResponseAsync(InteractionResponseType.Modal, new DiscordInteractionResponseBuilder().
                    WithCustomId($"{_context.Interaction.Id}-modal_footer").
                    WithTitle("Personalize o rodapé do seu Embed").
                    AddComponents(new TextInputComponent("Rodapé Texto", "form_footer", "(Opcional) Escreva sua assunatura ou tente @eu ou @bot", _embed.Footer.Value, false, TextInputStyle.Short, 0, 2048)).
                    AddComponents(new TextInputComponent("Rodapé Imagem", "form_footerimage", "(Opcional) cole um link direto da imagem ou tenet @eu ou @bot", _embed.Footer.Image, false, TextInputStyle.Short)).
                    AddComponents(new TextInputComponent("Rodapé Horário", "form_footertimestamp", "Use (SIM/TRUE) OU (NÃO/FALSE)", _embed.Footer.Timestamp.ToString().ToLower(), false, TextInputStyle.Short)));
        }
        private async Task FooterUpdate(string? value, string? image, string? timestamp)
        {
            _embed.Footer.Value = value.ToLower() == "@eu" ? _member.DisplayName : value.ToLower() == "@bot" ? _client.CurrentUser.Username : value;

            _embed.Footer.Timestamp = timestamp.ToLower() == "sim" || timestamp.ToLower() == "true" ? true : false;

            _embed.Footer.Image = string.IsNullOrWhiteSpace(image) ? null :
                image.ToLower() == "@eu" ? _member.AvatarUrl :
                image.ToLower() == "@bot" ? _client.CurrentUser.AvatarUrl :
                Uri.TryCreate(image, UriKind.Absolute, out _) ? image :
                throw new EdaurodoEmbedArgumentException("> * **Não consegui atualizar o rodapé do Embed, verifique se a URL é válida**", _interaction);

            await UpdateMessageAsync();
        }
        private async Task ImageModal()
        {
            await _interaction.CreateResponseAsync(InteractionResponseType.Modal, new DiscordInteractionResponseBuilder().
                                WithCustomId($"{_context.Interaction.Id}-modal_image").
                                WithTitle("Personalize as imagens do seu Embed").
                                AddComponents(new TextInputComponent("Imagem a baixo dos fields", "form_image", "(Opcional) cole o link direto da imagem.", _embed.Image, false, TextInputStyle.Short, 0, 256)).
                                AddComponents(new TextInputComponent("Imagem ao lado da descrição", "form_thumbnail", "(Opcional) cole o link direto da imagem.", _embed.Thumbnail, false, TextInputStyle.Short)));
        }
        private async Task ImageUpdate(string? image, string? thumbnail)
        {
            _embed.Image = string.IsNullOrWhiteSpace(image) ? null :
                Uri.TryCreate(image, UriKind.Absolute, out _) ? image :
                throw new EdaurodoEmbedArgumentException("> * **Não consegui atualizar as imagens do Embed, verifique se as URL's é válida**", _interaction);

            _embed.Thumbnail = string.IsNullOrWhiteSpace(thumbnail) ? null :
                Uri.TryCreate(thumbnail, UriKind.Absolute, out _) ? thumbnail :
                throw new EdaurodoEmbedArgumentException("> * **Não consegui atualizar as imagens do Embed, verifique se as URL's é válida**", _interaction);

            await UpdateMessageAsync();
        }
        private async Task AuthorModal()
        {
            await _interaction.CreateResponseAsync(InteractionResponseType.Modal, new DiscordInteractionResponseBuilder().
                    WithCustomId($"{_context.Interaction.Id}-modal_author").
                    WithTitle("Personalize o autor do seu Embed").
                    AddComponents(new TextInputComponent("Nome do autor", "form_author", "(Opcional) mas lebre-se que seu embed não pode ser vazio. Tente @eu ou @bot", _embed.Author.Name, false, TextInputStyle.Short, 0, 256)).
                    AddComponents(new TextInputComponent("Imagem do autor", "form_authorimage", "(Opcional) cole o link do avatar do author. Tente @eu ou @bot", _embed.Author.Image, false, TextInputStyle.Short)).
                    AddComponents(new TextInputComponent("Url do autor", "form_authorurl", "(Opcional) cole um link para para o redirecionamento.", _embed.Author.Url, false, TextInputStyle.Short)));
        }
        private async Task AuthorUpdate(string? value, string? image, string? url)
        {
            _embed.Author.Name = value.ToLower() == "@eu" ? _member.DisplayName : value.ToLower() == "@bot" ? _client.CurrentUser.Username : value;

            _embed.Author.Url = string.IsNullOrWhiteSpace(url) ? null : Uri.TryCreate(url, UriKind.Absolute, out _) ? url :
                throw new EdaurodoEmbedArgumentException("> * **Não consegui atualizar as informações do author, verifique se as URL's é válida**", _interaction);

            _embed.Author.Image = string.IsNullOrWhiteSpace(image) ? null :
                image.ToLower() == "@eu" ? _member.AvatarUrl :
                image.ToLower() == "@bot" ? _client.CurrentUser.AvatarUrl :
                Uri.TryCreate(image, UriKind.Absolute, out _) ? image :
                throw new EdaurodoEmbedArgumentException("> * **Não consegui atualizar as informações do author, verifique se as URL's é válida**", _interaction);

            await UpdateMessageAsync();
        }
        private async Task TitleModal()
        {
            await _interaction.CreateResponseAsync(InteractionResponseType.Modal, new DiscordInteractionResponseBuilder().
                    WithCustomId($"{_context.Interaction.Id}-modal_title").
                    WithTitle("Personalize o título do seu Embed").
                    AddComponents(new TextInputComponent("Título", "form_title", "(Opcional) mas lebre-se que seu embed não pode ser vazio", _embed.Title.Value, false, TextInputStyle.Short, 0, 2048)).
                    AddComponents(new TextInputComponent("Título URL", "form_titleurl", "(Opcional) cole um link para para o redirecionamento", _embed.Title.Url, false, TextInputStyle.Short)));
        }
        private async Task TitleUpdate(string? title, string? url)
        {
            _embed.Title.Value = title;

            _embed.Title.Url = string.IsNullOrWhiteSpace(url) ? null : Uri.TryCreate(url, UriKind.Absolute, out _) ? url :
                throw new EdaurodoEmbedArgumentException("> * **Não consegui atualizar as informações do seu titulo, verifique se a URL é válida**", _interaction);

            await UpdateMessageAsync();
        }
        private async Task DescriptionModal()
        {
            await _interaction.CreateResponseAsync(InteractionResponseType.Modal, new DiscordInteractionResponseBuilder().
                    WithCustomId($"{_context.Interaction.Id}-modal_description").
                    WithTitle("Personalize a descrição do seu Embed").
                    AddComponents(new TextInputComponent("Descrição", "form_description", "(Opcional) mas lebre-se que seu embed não pode ser vazio", _embed.Description, false, TextInputStyle.Paragraph, 0, 4000)));
        }
        private async Task DescriptionUpdate(string? description)
        {
            _embed.Description = description;
            await UpdateMessageAsync();
        }
        private async Task ExportJson()
        {
            await _interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(new DiscordEmbedBuilder().WithDescription($"```{JsonConvert.SerializeObject(new EdaurodoEmbedSerializable(_embed))}```").WithColor(new DiscordColor("#2B2D31"))).AsEphemeral(true));
        }
        private async Task UpdateMessageAsync()
        {
            await _interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().AddEmbed(EdaurodoUtilities.DiscordEmbedParse(_embed)).AddComponents(GetMainMenuComponents()));
        }
        private async Task UpdateMessagaColorPannel()
        {
            await _interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().AddEmbed(EdaurodoUtilities.DiscordEmbedParse(_embed)).AddComponents(GetColorMenuComponents()));
        }
        private async Task UpdateMessagaFieldPannel()
        {
            await _interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().AddEmbed(EdaurodoUtilities.DiscordEmbedParse(_embed)).AddComponents(GetFieldMenuComponents()));
        }
        private IEnumerable<DiscordActionRowComponent> GetMainMenuComponents()
        {
            List<DiscordActionRowComponent> btn_panel = new List<DiscordActionRowComponent>();
            #region UpLine of buttons
            List<DiscordComponent> btn_upline = new List<DiscordComponent>();
            DiscordComponentEmoji emoji_color = new DiscordComponentEmoji(DiscordEmoji.FromName(_client, ":art:"));
            btn_upline.Add(new DiscordButtonComponent(ButtonStyle.Secondary, $"{_context.Interaction.Id}-btn_color", "Cor", false, emoji_color));
            DiscordComponentEmoji emoji_author = new DiscordComponentEmoji(DiscordEmoji.FromName(_client, ":astronaut:"));
            btn_upline.Add(new DiscordButtonComponent(ButtonStyle.Secondary, $"{_context.Interaction.Id}-btn_author", "Autor", false, emoji_author));
            DiscordComponentEmoji emoji_addimage = new DiscordComponentEmoji(DiscordEmoji.FromName(_client, ":frame_photo:"));
            btn_upline.Add(new DiscordButtonComponent(ButtonStyle.Secondary, $"{_context.Interaction.Id}-btn_addimage", "Imagem e Thumbnail", false, emoji_addimage));
            btn_panel.Add(new DiscordActionRowComponent(btn_upline));
            #endregion
            #region MidLine of buttons
            List<DiscordComponent> btn_midline = new List<DiscordComponent>();
            DiscordComponentEmoji emoji_title = new DiscordComponentEmoji(DiscordEmoji.FromName(_client, ":page_facing_up:"));
            btn_midline.Add(new DiscordButtonComponent(ButtonStyle.Secondary, $"{_context.Interaction.Id}-btn_title", "Título", false, emoji_title));
            DiscordComponentEmoji emoji_description = new DiscordComponentEmoji(DiscordEmoji.FromName(_client, ":pencil:"));
            btn_midline.Add(new DiscordButtonComponent(ButtonStyle.Secondary, $"{_context.Interaction.Id}-btn_description", "Descrição", false, emoji_description));
            DiscordComponentEmoji emoji_field = new DiscordComponentEmoji(DiscordEmoji.FromName(_client, ":pen_fountain:"));
            btn_midline.Add(new DiscordButtonComponent(ButtonStyle.Secondary, $"{_context.Interaction.Id}-btn_field", "Fields", false, emoji_field));
            DiscordComponentEmoji emoji_footer = new DiscordComponentEmoji(DiscordEmoji.FromName(_client, ":triangular_flag_on_post:"));
            btn_midline.Add(new DiscordButtonComponent(ButtonStyle.Secondary, $"{_context.Interaction.Id}-btn_footer", "Footer", false, emoji_footer));
            btn_panel.Add(new DiscordActionRowComponent(btn_midline));
            #endregion
            #region DownLine of buttons
            List<DiscordComponent> btn_downline = new List<DiscordComponent>();
            DiscordComponentEmoji emoji_upload = new DiscordComponentEmoji(DiscordEmoji.FromName(_client, ":arrow_up:"));
            btn_downline.Add(new DiscordButtonComponent(ButtonStyle.Primary, $"{_context.Interaction.Id}-btn_jsonup", "Importar JSON", false, emoji_upload));
            DiscordComponentEmoji emoji_download = new DiscordComponentEmoji(DiscordEmoji.FromName(_client, ":arrow_down:"));
            btn_downline.Add(new DiscordButtonComponent(ButtonStyle.Primary, $"{_context.Interaction.Id}-btn_jsondown", "Exportar JSON", false, emoji_download));
            DiscordComponentEmoji emoji_send = new DiscordComponentEmoji(DiscordEmoji.FromName(_client, ":incoming_envelope:"));
            btn_downline.Add(new DiscordButtonComponent(ButtonStyle.Success, $"{_context.Interaction.Id}-btn_sendembed", "Enviar aqui", false, emoji_send));
            btn_panel.Add(new DiscordActionRowComponent(btn_downline));
            #endregion
            return btn_panel;
        }
        private IEnumerable<DiscordActionRowComponent> GetColorMenuComponents()
        {
            List<DiscordSelectComponentOption> selectOptions = new List<DiscordSelectComponentOption>();
            selectOptions.Clear();
            DiscordComponentEmoji emoji_add = new DiscordComponentEmoji(DiscordEmoji.FromName(_client, ":heavy_plus_sign:"));
            selectOptions.Add(new DiscordSelectComponentOption("Adicione um código HEX", "select_colorhex", null, false, emoji_add));
            foreach (var color in EdaurodoEmbed.Colors)
            {
                selectOptions.Add(new DiscordSelectComponentOption(color.Value, color.Key, null, false, null));
            }
            List<DiscordComponent> select_line = new List<DiscordComponent>() {
                new DiscordSelectComponent($"{_context.Interaction.Id}-select_colors", "Escolha uma cor para seu Embed", selectOptions, false, 1, 1)
            };
            DiscordComponentEmoji emoji_goback = new DiscordComponentEmoji(DiscordEmoji.FromName(_client, ":back:"));
            List<DiscordComponent> button_line = new List<DiscordComponent>() {
            new DiscordButtonComponent(ButtonStyle.Secondary, $"{_context.Interaction.Id}-btn_goback", null, false, emoji_goback),
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
            if (_embed.Fields.Count() > 0)
            {
                DiscordComponentEmoji emoji_edit = new DiscordComponentEmoji(DiscordEmoji.FromName(_client, ":pen_fountain:"));
                DiscordComponentEmoji emoji_delete = new DiscordComponentEmoji(DiscordEmoji.FromName(_client, ":recycle:"));
                List<DiscordComponent> button_upline = new List<DiscordComponent>() {
                    new DiscordButtonComponent(ButtonStyle.Secondary, $"{_context.Interaction.Id}-btn_editfield", "Editar", _fieldIndex < 0, emoji_edit),
                    new DiscordButtonComponent(ButtonStyle.Secondary, $"{_context.Interaction.Id}-btn_deletefield", "Excluir", _fieldIndex < 0, emoji_delete)
                };
                if (_fieldIndex > -1)
                {
                    string fieldTitle = _embed.Fields.ElementAt(_fieldIndex).Title.Length > 25 ? _embed.Fields.ElementAt(_fieldIndex).Title.Substring(0, 25) : _embed.Fields.ElementAt(_fieldIndex).Title;
                    button_upline.Add(new DiscordButtonComponent(ButtonStyle.Secondary, "...", $"\"{fieldTitle}\" Selecionado", true, null));
                }
                else { button_upline.Add(new DiscordButtonComponent(ButtonStyle.Secondary, "...", "Nenhum field selecionado", true, null)); }
                components_lines.Add(new DiscordActionRowComponent(button_upline));
                List<DiscordSelectComponentOption> selectOptions = new List<DiscordSelectComponentOption>();
                selectOptions.Clear();
                int i = 0;
                foreach (var field in _embed.Fields)
                {
                    string title = field.Title.Length > 50 ? field.Title.Substring(0, 50) : field.Title;
                    string content = field.Value.Length > 50 ? field.Value.Substring(0, 50) : field.Value;
                    selectOptions.Add(new DiscordSelectComponentOption($"{i + 1}. {title}", $"{i}", content, false, null));
                    i++;
                }
                List<DiscordComponent> select_line = new List<DiscordComponent>() {
                new DiscordSelectComponent($"{_context.Interaction.Id}-select_field", "Escolha um field para editar", selectOptions, false, 1, 1)
                };
                components_lines.Add(new DiscordActionRowComponent(select_line));
            }
            DiscordComponentEmoji emoji_goback = new DiscordComponentEmoji(DiscordEmoji.FromName(_client, ":back:"));
            DiscordComponentEmoji emoji_add = new DiscordComponentEmoji(DiscordEmoji.FromName(_client, ":heavy_plus_sign:"));
            List<DiscordComponent> button_downline = new List<DiscordComponent>() {
                new DiscordButtonComponent(ButtonStyle.Secondary, $"{_context.Interaction.Id}-btn_goback", null, false, emoji_goback),
                new DiscordButtonComponent(ButtonStyle.Success, $"{_context.Interaction.Id}-btn_addfield", "Novo field", !(_embed.Fields.Count() < 25), emoji_add)
            };
            components_lines.Add(new DiscordActionRowComponent(button_downline));
            return components_lines;
        }
    }
}