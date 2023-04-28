using DSharpPlus;
using DSharpPlus.Entities;
using EdaurodoBot.rsc.modules.allowlistmodule.config;
using EdaurodoBot.rsc.modules.allowlistmodule.data;
using EdaurodoBot.rsc.modules.allowlistmodule.enums;

namespace EdaurodoBot.rsc.modules.allowlistmodule
{
    public sealed class Allowlist
    {
        private DiscordMember Member;
        private DiscordGuild Guild;
        private Allowlist(DiscordMember member, DiscordGuild guild)
        {

            this.Member = member;
            this.Guild = guild;
        }
        public static async Task ExecuteAsync(AllowlistData data, AllowlistConfig config, DiscordGuild guild)
        {
            var allowlist = new Allowlist(guild.GetMemberAsync(data.DiscordUser.Id).GetAwaiter().GetResult(), guild);

            if (data.StartAllowlistTime == null || data.FinishAllowlistTime == null || data.FinishAllowlistTime.Value.Subtract(DateTime.Now.ToUniversalTime()).TotalMinutes < config.ReprovedWaitTime * -1)
            {
                if (data.StartAllowlistTime == null)
                {
                    await data.IncrementCurrentQuestion();
                    await allowlist.CreateAllowlistChannel(data, config, allowlist);

                }
                if (data.FinishAllowlistTime != null)
                {
                    await data.ClearDataBase();
                    await data.IncrementCurrentQuestion();
                    await allowlist.CreateAllowlistChannel(data, config, allowlist);
                }
                if (data.CurrentQuestion < config.Questions.Length) { await allowlist.UpdateMessageQuiz(data, config); }
                else
                {
                    if (await allowlist.QuizApproved(data, config)) { if (data.CurrentForm != Form.None) { await allowlist.SendFormToUser(data.Interaction, config, data.CurrentForm); } else { await allowlist.FinalizeAllowlist(data, config, allowlist, true); } }
                    else { await allowlist.FinalizeAllowlist(data, config, allowlist, false); }
                }
            }
            else
            {
                await data.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                .WithColor(new DiscordColor("#2B2D31"))
                .WithDescription(
                    "### > Você ainda não pode fazer a Allowlist!\n" +
                    $"> Você tentou fazer a Allowlist recentemente e não passou\n" +
                    $"> você pode tentar novamente em **{data.FinishAllowlistTime.Value.AddMinutes((double)config.ReprovedWaitTime).Subtract(DateTime.Now.ToUniversalTime()).ToString("hh\\:mm\\:ss")}**, enquanto isso\n" +
                    $"> use este tempo para ler nossas regras [clicando aqui]({config.Messages.MainMessage.ButtonLink})!"))
                .AsEphemeral(true));
            }
        }
        public static async Task OpenRealInfoModal(AllowlistData data)
        {
            await data.Interaction.CreateResponseAsync(InteractionResponseType.Modal, new DiscordInteractionResponseBuilder()
                .WithCustomId("modal_RealInfoModal")
                .WithTitle("INFORMAÇÕES SOBRE VOCÊ")
                .AddComponents(new TextInputComponent("Qual é o seu nome real ?", "AlRealName", "Seu nome completo", data.UserName, required: true, style: TextInputStyle.Short, max_length: 50))
                .AddComponents(new TextInputComponent("Qual é a sua idade real ?", "AlRealAge", "Somente números (ex: 18)", data.UserAge, required: true, style: TextInputStyle.Short, max_length: 2))
                .AddComponents(new TextInputComponent("Você já jogou Roleplay antes?", "AlExp", "se \"sim\" quanto tempo?", data.UserExp, required: true, style: TextInputStyle.Short, max_length: 20)));
        }
        public static async Task OpenCharInfoModal(AllowlistData data)
        {
            await data.Interaction.CreateResponseAsync(InteractionResponseType.Modal, new DiscordInteractionResponseBuilder()
                .WithCustomId("modal_CharInfoModal")
                .WithTitle("INFORMAÇÕES SOBRE SEU PERSONAGEM")
                .AddComponents(new TextInputComponent("Qual o nome do seu personagem ?", "AlCharName", "Nome do seu personagem", required: true, style: TextInputStyle.Short, max_length: 50))
                .AddComponents(new TextInputComponent("Qual a idade do seu personagem ?", "AlCharAge", "Somente números (Ex: 18)", required: true, style: TextInputStyle.Short, max_length: 3))
                .AddComponents(new TextInputComponent("Conte-nos mais sobre seu personagem.", "AlCharLore", "Conte-nos sobre as características e sobre a história de seu personagem", required: true, style: TextInputStyle.Paragraph, max_length: 4000)));
        }
        public static async Task AllowlistApproved(AllowlistData data, AllowlistConfig config, DiscordGuild guild)
        {
            var allowlist = new Allowlist(guild.GetMemberAsync(data.DiscordUser.Id).GetAwaiter().GetResult(), guild);
            if (await allowlist.TryModifyApprovedAllowlistUserAsync(data, config, allowlist))
            {
                await data.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().AddEmbeds(await allowlist.GetEmbedsWithUserInformations(data)));

                await guild.GetChannel((ulong)config.Channels.ApprovedId)
                    .SendMessageAsync($"> * <@{data.DiscordUser.Id}> Sua Allowlist foi aprovada: Fique atento ao canal <#{config.Channels.InterviewId}> para participar no melhor horário para você!");

                await data.Interaction.Channel.SendMessageAsync(
                    new DiscordEmbedBuilder()
                    .WithColor(new DiscordColor("#2B2D31"))
                    .WithDescription(
                        $"### Você aprovou a Allowlist de <@{data.DiscordUser.Id}>\n\n" +
                        $"Uma mensagem foi enviada para <#{config.Channels.ApprovedId}> avisando-o,\n" +
                        $"assim que possível envie uma mensagem em <#{config.Channels.InterviewId}>!\n" +
                        $"atualizando o horário de entrevista.\n\n" +
                        $"**Este canal será excluido em 30 segundos**")
                    .Build());

                await allowlist.DeleteChannelAsync(data.Interaction.Channel);
            }
            else
            {
                await data.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().AddEmbeds(await allowlist.GetEmbedsWithUserInformations(data)));

                await data.Interaction.Channel.SendMessageAsync(
                    new DiscordEmbedBuilder()
                    .WithColor(new DiscordColor("#2B2D31"))
                    .WithDescription(
                        $"### Não foi possível aprovar a Allowlist." +
                        $"O usuário <@{data.DiscordUser.Id}> não faz mais parte do discord!\n\n" +
                        $"**Este canal será excluido em 30 segundos**")
                    .Build());

                await allowlist.DeleteChannelAsync(data.Interaction.Channel);
            }
        }
        public static async Task AllowlistReproved(AllowlistData data, AllowlistConfig config, DiscordGuild guild, string reason)
        {
            var allowlist = new Allowlist(guild.GetMemberAsync(data.DiscordUser.Id).GetAwaiter().GetResult(), guild);

            if (await allowlist.TryModifyReprovedAllowlistUserAsync(data, config, allowlist))
            {
                await data.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().AddEmbeds(await allowlist.GetEmbedsWithUserInformations(data)));

                await data.Interaction.Channel.SendMessageAsync(
                    new DiscordEmbedBuilder()
                    .WithColor(new DiscordColor("#2B2D31"))
                    .WithDescription(
                        $"### Você reprovou a Allowlist de <@{data.DiscordUser.Id}>\n\n" +
                        $"Uma mensagem foi enviada para <#{config.Channels.ReprovedId}> avisando-o,\n" +
                        $"com os seguintes motivos..." +
                        $"```\n{reason}\n```\n\n" +
                        $"**Este canal será excluido em 30 segundos**")
                    .Build());

                await allowlist.Guild.GetChannel((ulong)config.Channels.ReprovedId)
                    .SendMessageAsync(new DiscordMessageBuilder()
                    .WithContent($"> * <@{data.DiscordUser.Id}> Sua Allowlist foi reprovada pelos seguintes motivos:")
                    .WithEmbed(new DiscordEmbedBuilder()
                    .WithColor(new DiscordColor("#2B2D31"))
                    .WithDescription($"```\n{reason}\n```\n**Releia as regras** [clicando aqui]({config.Messages.MainMessage.ButtonLink})!")
                    .Build()));
                await data.ReprovedClearDataBase();
                await allowlist.DeleteChannelAsync(data.Interaction.Channel);
            }
            else
            {
                await data.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().AddEmbeds(await allowlist.GetEmbedsWithUserInformations(data)));

                await data.Interaction.Channel.SendMessageAsync(
                    new DiscordEmbedBuilder()
                    .WithColor(new DiscordColor("#2B2D31"))
                    .WithDescription(
                        $"### Não foi possível reprovar a Allowlist." +
                        $"O usuário <@{data.DiscordUser.Id}> não faz mais parte do discord!\n\n" +
                        $"**Este canal será excluido em 30 segundos**")
                    .Build());

                await allowlist.DeleteChannelAsync(data.Interaction.Channel);
            }
        }
        public static async Task AllowlistReprovedModal(AllowlistData data)
        {
            await data.Interaction.CreateResponseAsync(InteractionResponseType.Modal, new DiscordInteractionResponseBuilder()
               .WithCustomId("modal_Reproved")
               .WithTitle("MOTIVOS PELO QUAL O REPROVOU")
               .AddComponents(new TextInputComponent("Seja objetivo!", "AlReasons", "- motivo um\n- motivo dois\n- motivo três", required: true, style: TextInputStyle.Paragraph, max_length: 4000)));
        }
        private async Task FinalizeAllowlist(AllowlistData data, AllowlistConfig config, Allowlist allowlist, bool approved)
        {
            if (approved)
            {
                await SendFormToReader(data, allowlist.Member);
                await data.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                    .WithColor(new DiscordColor("#2B2D31"))
                    .WithDescription(
                        "### > Parabêns você finalizou sua aplicação para a Allowlist!\n" +
                        $"Seu formulário foi enviado para os nossos <@&{config.Roles.ReaderId}>,\n" +
                        "agora basta esperar que eles leiam sua lore, e em breve você ira\n" +
                        $"receber uma notificação em <#{config.Channels.ApprovedId}> se aprovado.\n\n" +
                        "> * **Seu canal será deletado em 30 segundos.**")
                    .WithFooter("📄 Sistema de Allowlist - Kansas Roleplay")
                    .Build()));

                await data.SubmitFinishAllowlistTime();
                await SetRoleSentAllowlist(data, config, allowlist);

                _ = Task.Run(async () =>
                {
                    Task.Delay(30000);
                    await DeleteChannelAsync(data.AllowlistUserChannel);
                });
            }
            else
            {
                await data.SubmitFinishAllowlistTime();

                await allowlist.Guild.GetChannel((ulong)config.Channels.ReprovedId).SendMessageAsync($"> * <@{data.DiscordUser.Id}> Sua Allowlist foi reprovada... Releia as regras do servidor [clicando aqui]({config.Messages.MainMessage.ButtonLink}), e tente novamente assim que se sentir confiante.");

                await data.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                    .WithColor(new DiscordColor("#2B2D31"))
                    .WithDescription(
                        "### > Infelizmente você não passou em nosso questionário\n" +
                        $"Você não conseguiu passar pelo questionário da **Allowlist**,\n" +
                        "mas não tem problema você pode tentar fazer novamente em\n" +
                        $"**{config.ReprovedWaitTime} minutos**, enquanto isto você pode rever nossas regras\n" +
                        $"[clicando aqui]({config.Messages.MainMessage.ButtonLink}) ou no botão a baixo **\"Leia as Regras\"**\n" +
                        $"você pode verificar em <#{config.Channels.ReprovedId}> o motivo pelo qual\n" +
                        $"você foi reprovado na **Allowlist**!\n\n" +
                        "> * **Seu canal será deletado em 30 segundos.**")
                    .WithFooter("📄 Sistema de Allowlist - Kansas Roleplay")
                    .Build())
                    .AddComponents(await GetButtonRules(config)));
                await ShowAllowlistChannel(data, config, allowlist);

                await DeleteChannelAsync(data.AllowlistUserChannel);
            }
        }
        private async Task SendFormToUser(DiscordInteraction interaction, AllowlistConfig config, Form form)
        {
            await interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().AddEmbed(await GetEmbedToStartForm(config)).AddComponents(await GetButtonForm(form)));
        }
        private async Task SendFormToReader(AllowlistData data, DiscordMember member)
        {
            var readerCategories = member.Guild.GetChannelsAsync().GetAwaiter().GetResult().ToList().FindAll(_ => _.IsCategory == true && _.Name.StartsWith("reader-"));
            var category = readerCategories.First();
            foreach (var tempReaderCategory in readerCategories)
            {
                if (tempReaderCategory.Children.Count < category.Children.Count) { category = tempReaderCategory; }
            }
            await member.Guild.CreateChannelAsync(
                name: $"allowlist-{member.Id}",
                type: ChannelType.Text,
                parent: category).GetAwaiter().GetResult()
                .SendMessageAsync(new DiscordMessageBuilder()
                .AddEmbeds(await GetEmbedsWithUserInformations(data))
                .AddComponents(new List<DiscordComponent>(){
                    new DiscordButtonComponent(ButtonStyle.Danger, "btn_AlReproved", "Reprovar Allowlist", false),
                    new DiscordButtonComponent(ButtonStyle.Success, "btn_AlApproved", "Aprovar Allowlist", false)}));
        }
        private async Task CreateAllowlistChannel(AllowlistData data, AllowlistConfig config, Allowlist allowlist)
        {
            await data.SubmitStartAllowlistTime();

            await data.SetAllowlistChannel(allowlist.Guild.CreateChannelAsync(
                name: $"allowlist-{data.DiscordUser.Id}",
                type: ChannelType.Text,
                parent: allowlist.Guild.GetChannel((ulong)config.Channels.CategoryId),
                overwrites: new List<DiscordOverwriteBuilder>() {
                    new DiscordOverwriteBuilder()
                    .For(allowlist.Member)
                    .Allow(Permissions.ReadMessageHistory)
                    .Allow(Permissions.AccessChannels)
                    .Deny(Permissions.SendMessages),
                    new DiscordOverwriteBuilder()
                    .For(allowlist.Guild.EveryoneRole)
                    .Deny(Permissions.AccessChannels)}).GetAwaiter().GetResult()
                    .SendMessageAsync(
                new DiscordMessageBuilder()
                .AddEmbed(await GetEmbedCurrentQuestion(data, config))
                .AddComponents(await GetSelectOptionsCurrentQuestion(data, config))).GetAwaiter().GetResult().Channel);


            await data.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                .WithColor(new DiscordColor("#2B2D31"))
                .WithDescription(
                    $"> O canal <#{data.AllowlistUserChannel.Id}> foi criado para você fazer sua **Allowlist**,\n" +
                    "> a partir deste ponto você tem **15 minutos** para enviar sua **Allowlist**,\n" +
                    "> após este tempo seu canal será **excluido** e você terá que refazer\n" +
                    "> sua **Allowlist**, a equipe do Kansas agradece e lhe deseja boa sorte!")).AsEphemeral(true));

            await HideAllowlistChannel(data, config, allowlist);
        }
        private async Task UpdateMessageQuiz(AllowlistData data, AllowlistConfig config)
        {
            await data.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().AddEmbed(await GetEmbedCurrentQuestion(data, config)).AddComponents(await GetSelectOptionsCurrentQuestion(data, config)));
        }
        private async Task<bool> QuizApproved(AllowlistData data, AllowlistConfig config)
        {
            for (int i = 0; i < config.Questions.Length; i++)
            {
                if (data.Responses[i] != config.Questions[i].CorrectAnswer) { return false; }
            }
            return true;
        }
        private async Task SetRoleSentAllowlist(AllowlistData data, AllowlistConfig config, Allowlist allowlist)
        {
            List<DiscordRole> roles = allowlist.Member.Roles.ToList();
            roles.Add(allowlist.Guild.GetRole((ulong)config.Roles.AllowlistSentId));
            await allowlist.Member.ModifyAsync(_ => { _.Roles = roles; });
        }
        private async Task<bool> TryModifyApprovedAllowlistUserAsync(AllowlistData data, AllowlistConfig config, Allowlist allowlist)
        {
            if (allowlist.Member != null)
            {
                List<DiscordRole> roles = allowlist.Member.Roles.ToList();
                if (roles.Contains(allowlist.Guild.GetRole((ulong)config.Roles.AllowlistSentId)))
                {
                    roles.Remove(allowlist.Guild.GetRole((ulong)config.Roles.AllowlistSentId));
                }
                roles.Add(allowlist.Guild.GetRole((ulong)config.Roles.WaitingInterviewId));
                await allowlist.Member.ModifyAsync(_ => _.Roles = roles);
                return true;
            }
            return false;
        }
        private async Task<bool> TryModifyReprovedAllowlistUserAsync(AllowlistData data, AllowlistConfig config, Allowlist allowlist)
        {
            if (allowlist.Member != null)
            {
                List<DiscordRole> roles = allowlist.Member.Roles.ToList();
                if (roles.Contains(allowlist.Guild.GetRole((ulong)config.Roles.AllowlistSentId)))
                {
                    roles.Remove(allowlist.Guild.GetRole((ulong)config.Roles.AllowlistSentId));
                }
                await allowlist.Member.ModifyAsync(_ => _.Roles = roles);
                await ShowAllowlistChannel(data, config, allowlist);
                return true;
            }
            return false;
        }
        private async Task HideAllowlistChannel(AllowlistData data, AllowlistConfig config, Allowlist allowlist)
        {
            await allowlist.Guild.GetChannel((ulong)config.Channels.MainId).AddOverwriteAsync(allowlist.Member, Permissions.None, Permissions.AccessChannels);
        }
        private async Task ShowAllowlistChannel(AllowlistData data, AllowlistConfig config, Allowlist allowlist)
        {
            await allowlist.Guild.GetChannel((ulong)config.Channels.MainId).DeleteOverwriteAsync(allowlist.Member);
        }
        private Task DeleteChannelAsync(DiscordChannel channel)
        {
            _ = Task.Run(async () =>
            {
                await Task.Delay(30000);
                await channel.DeleteAsync();
            });
            return Task.CompletedTask;
        }
        private Task<List<DiscordEmbed>> GetEmbedsWithUserInformations(AllowlistData data)
        {
            return Task.FromResult(new List<DiscordEmbed>(){
                new DiscordEmbedBuilder()
                .WithColor(new DiscordColor("#2B2D31"))
                .WithDescription(
                    "### > `userinfo [ignore]`\n" +
                    $"user: <@{data.DiscordUser.Id}>\n" +
                    $"user_id: {data.DiscordUser.Id}\n")
                .Build(),
                new DiscordEmbedBuilder()
                .WithColor(new DiscordColor("#2B2D31"))
                .WithDescription(
                    $"### > Você esta lendo a Allowlist de {data.DiscordUser.Username}\nㅤ\n" +
                    "> **Leia com todo cuidado e seja detalhista**\nㅤ\n" +
                    "* Você é o filtro para fazer o servidor um local imersivo e" +
                    " seguro para todos os jogadores, leia sem pressa e se precisar" +
                    " tome notas de dúvidas que você teve sobre a história do personagem," +
                    " na hora da entrevista seja atencioso(a) e explique para os player sobre" +
                    " o servidor e sobre o roleplay, pois muitos será a primeira vez jogando.")
                .Build(),
                new DiscordEmbedBuilder()
                .WithColor(new DiscordColor("#2B2D31"))
                .AddField("ㅤ", "> **INFORMÇÕES PESSOAIS**", false)
                .AddField("> NOME REAL", $"**Nome:**\n`{data.UserName}`", true)
                .AddField("> IDADE REAL", $"**Idade:**\n`{data.UserAge}`", true)
                .AddField("> EXPERIÊNCIA", $"**Experiência:**\n`{data.UserExp}`", true)
                .AddField( "ㅤ", "**INFORMAÇÕES SOBRE O PERSONAGEM**", false)
                .AddField("> NOME DO PERSONAGEM", $"**Nome:**\n`{data.CharName}`",true)
                .AddField("> IDADE DO PERSONAGEM", $"**Idade:**\n`{data.CharAge}`", true)
                .Build(),
                new DiscordEmbedBuilder()
                .WithColor(new DiscordColor("#2B2D31"))
                .WithTitle("> LORE DO PERSONAGEM")
                .WithDescription($"```\n{data.CharLore}\n```")
                .Build()
            });
        }
        private Task<DiscordEmbed> GetEmbedToStartForm(AllowlistConfig config)
        {
            return Task.FromResult(new DiscordEmbedBuilder()
                .WithColor(new DiscordColor("#2B2D31"))
                .WithDescription(
                "### > Parabéns você passou para a segunda etapa da Allowlist\n\n" +
                "Nesta etapa eu vou pedir para que você conte um pouco mais\n" +
                "sobre você e seu personagem, para isso seria bom que você\n" +
                "já tenha consigo em mãos a história de seu personagem pronta\n\n" +
                "> * Tente focar em traços fortes dele como suas característica;\n" +
                "> * Lembre-se é claro no contexto em que o universo se passa;\n" +
                "> * Evite histórias genéricas como \"minha familía morreu por isso sou assim\";\n\n" +
                $"Esses dados serão enviados para nossos <@&{config.Roles.ReaderId}> para que\n" +
                $"avaliem sua história e se condiz com o contexto em que se passa o roleplay,\n" +
                "após esta etapa se você passar iremos marcar a entrevista com você.")
                .WithFooter("📄 Sistema de Allowlist - Kansas Roleplay")
                .Build());
        }
        private Task<DiscordButtonComponent> GetButtonForm(Form form)
        {
            switch (form)
            {
                case Form.User:
                    return Task.FromResult(new DiscordButtonComponent(ButtonStyle.Success, "btn_openRealInfoModal", "Abrir formulário sobre você", false));
                case Form.Character:
                    return Task.FromResult(new DiscordButtonComponent(ButtonStyle.Success, "btn_openCharInfoModal", "Abrir formulário do personagem", false));
                default:
                    throw new Exception();
            }
        }
        private Task<DiscordLinkButtonComponent> GetButtonRules(AllowlistConfig config)
        {
            return Task.FromResult(new DiscordLinkButtonComponent(config.Messages.MainMessage.ButtonLink, "Leia as Regras", false, null));
        }
        private Task<DiscordEmbed> GetEmbedCurrentQuestion(AllowlistData data, AllowlistConfig config)
        {
            string embedContent =
                $"### > Você esta fazendo a Allowlist para o Kansas Roleplay: {data.CurrentQuestion + 1}/{config.Questions.Length}\n" +
                "-------------------------------------------------------------------------------------\n" +
                $"**{config.Questions[(int)data.CurrentQuestion].Question}**";

            char a = 'A';
            foreach (string alt in config.Questions[(int)data.CurrentQuestion].Alternatives)
            {
                embedContent += $"\n\n**{a} -** `{alt}`";
                a++;
            }

            DiscordEmbedBuilder eb = new DiscordEmbedBuilder()
               .WithColor(new DiscordColor("#2B2D31"))
               .WithDescription(embedContent);
            return Task.FromResult(eb.Build());
        }
        private Task<DiscordSelectComponent> GetSelectOptionsCurrentQuestion(AllowlistData data, AllowlistConfig config)
        {
            char a = 'A';
            int i = 1;
            List<DiscordSelectComponentOption> selectOptions = new List<DiscordSelectComponentOption>();
            selectOptions.Clear();
            foreach (string alt in config.Questions[(int)data.CurrentQuestion].Alternatives)
            {
                selectOptions.Add(new DiscordSelectComponentOption($"{a} - {alt}", $"{i}", null, false, null));
                i++;
                a++;
            }
            return Task.FromResult(new DiscordSelectComponent($"select_AlAlternativesResponse", "Escolha a alternativa correta!", selectOptions, false, 1, 1));
        }
    }
}