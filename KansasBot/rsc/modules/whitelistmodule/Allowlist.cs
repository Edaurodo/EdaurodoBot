using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using KansasBot.rsc.modules.whitelistmodule.services;

namespace KansasBot.rsc.modules.whitelistmodule
{
    public sealed class Allowlist
    {
        private AllowlistService Service;
        private InteractionCreateEventArgs Sender;
        private DiscordGuild Guild;
        private DiscordInteraction Interaction;
        private DiscordUser User;
        private DiscordMember? Member;
        private int Form;

        public Allowlist(AllowlistService service, InteractionCreateEventArgs sender)
        {
            Service = service;
            Sender = sender;
            Interaction = Sender.Interaction;
            User = Interaction.User;
            Guild = Interaction.Guild;
            Member = null;
            Form = 0;
        }

        public Task UpdateInteraction(DiscordInteraction interaction)
        {
            Interaction = interaction;
            return Task.CompletedTask;
        }
        public Task NextForm()
        {
            Form++;
            return Task.CompletedTask;
        }
        public async Task OpenRealInfoModal()
        {
            await Interaction.CreateResponseAsync(InteractionResponseType.Modal, new DiscordInteractionResponseBuilder()
                .WithCustomId("modal_RealInfoModal")
                .WithTitle("INFORMAÇÕES SOBRE VOCÊ")
                .AddComponents(new TextInputComponent("Qual é o seu nome real ?", "AlRealName", "Seu nome completo", required: true, style: TextInputStyle.Short, max_length: 50))
                .AddComponents(new TextInputComponent("Qual é a sua idade real ?", "AlRealAge", "Somente números (ex: 18)", required: true, style: TextInputStyle.Short, max_length: 2))
                .AddComponents(new TextInputComponent("Você já jogou Roleplay antes?", "AlExp", "se \"sim\" quanto tempo?", required: true, style: TextInputStyle.Short, max_length: 20)));
        }
        public async Task OpenCharInfoModal()
        {
            await Interaction.CreateResponseAsync(InteractionResponseType.Modal, new DiscordInteractionResponseBuilder()
                .WithCustomId("modal_CharInfoModal")
                .WithTitle("INFORMAÇÕES SOBRE SEU PERSONAGEM")
                .AddComponents(new TextInputComponent("Qual o nome do seu personagem ?", "AlCharName", "Nome do seu personagem", required: true, style: TextInputStyle.Short, max_length: 50))
                .AddComponents(new TextInputComponent("Qual a idade do seu personagem ?", "AlCharAge", "Somente números (Ex: 18)", required: true, style: TextInputStyle.Short, max_length: 3))
                .AddComponents(new TextInputComponent("Conte-nos mais sobre seu personagem.", "AlCharLore", "Conte-nos sobre as características e sobre a história de seu personagem", required: true, style: TextInputStyle.Paragraph, max_length: 4000)));
        }
        public async Task ExecuteAsync()
        {
            if (Service.Data[User.Id].StartAllowlistTime == null || Service.Data[User.Id].FinishAllowlistTime == null || Service.Data[User.Id].FinishAllowlistTime.Value.Subtract(DateTime.Now.ToUniversalTime()).TotalMinutes < Service.Config.ReprovedWaitTime * (-1))
            {
                if (Service.Data[User.Id].StartAllowlistTime == null)
                {
                    await Service.Data[User.Id].IncrementCurrentQuestion();
                    await CreateAllowlistChannel();

                }
                if (Service.Data[User.Id].FinishAllowlistTime != null)
                {
                    await Service.Data[User.Id].ClearDataBase();
                    await Service.Data[User.Id].IncrementCurrentQuestion();
                    await CreateAllowlistChannel();
                }
                if (Service.Data[User.Id].CurrentQuestion < Service.Config.Questions.Length) { await UpdateMessageQuiz(); }
                else
                {
                    if (await QuizApproved()) { if (Form < 2) { await SendFormToUser(); } else { await FinalizeAllowlist(true); } }
                    else { await FinalizeAllowlist(false); }
                }
            }
            else
            {
                await Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                .WithColor(new DiscordColor("#2B2D31"))
                .WithDescription(
                    "### > Você ainda não pode fazer a Allowlist!\n" +
                    $"> Você tentou fazer a Allowlist recentemente e não passou\n" +
                    $"> você pode tentar novamente em **{Service.Data[User.Id].FinishAllowlistTime.Value.AddMinutes((double)Service.Config.ReprovedWaitTime).Subtract(DateTime.Now.ToUniversalTime()).ToString("hh\\:mm\\:ss")}**, enquanto isso\n" +
                    $"> use este tempo para ler nossas regras [clicando aqui]({Service.Config.Messages.MainMessage.ButtonLink})!"))
                .AsEphemeral(true));
            }
        }
        public async Task AllowlistApproved()
        {
            if (await TryModifyApprovedAllowlistUserAsync())
            {
                await Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().AddEmbeds(await GetEmbedsWithUserInformations()));

                await Guild.GetChannel((ulong)Service.Config.Channels.ApprovedId)
                    .SendMessageAsync($"> * <@{User.Id}> Sua Allowlist foi aprovada: Fique atento ao canal <#{Service.Config.Channels.InterviewId}> para participar no melhor horário para você!");

                await Interaction.Channel.SendMessageAsync(
                    new DiscordEmbedBuilder()
                    .WithColor(new DiscordColor("#2B2D31"))
                    .WithDescription(
                        $"### Você aprovou a Allowlist de <@{User.Id}>\n\n" +
                        $"Uma mensagem foi enviada para <#{Service.Config.Channels.ApprovedId}> avisando-o,\n" +
                        $"assim que possível envie uma mensagem em <#{Service.Config.Channels.InterviewId}>!\n" +
                        $"atualizando o horário de entrevista.\n\n" +
                        $"**Este canal será excluido em 30 segundos**")
                    .Build());
                _ = Task.Run(async () =>
                {
                    await Task.Delay(30000);
                    await Interaction.Channel.DeleteAsync();
                });
            }
            else
            {
                await Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().AddEmbeds(await GetEmbedsWithUserInformations()));

                await Interaction.Channel.SendMessageAsync(
                    new DiscordEmbedBuilder()
                    .WithColor(new DiscordColor("#2B2D31"))
                    .WithDescription(
                        $"### Não foi possível aprovar a Allowlist." +
                        $"O usuário <@{User.Id}> não faz mais parte do discord!\n\n" +
                        $"**Este canal será excluido em 30 segundos**")
                    .Build());
                _ = Task.Run(async () =>
                {
                    await Task.Delay(30000);
                    await Interaction.Channel.DeleteAsync();
                });
            }
        }
        public async Task AllowlistReproved(string reason)
        {
            if (await TryModifyReprovedAllowlistUserAsync())
            {
                await Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().AddEmbeds(await GetEmbedsWithUserInformations()));

                await Interaction.Channel.SendMessageAsync(
                    new DiscordEmbedBuilder()
                    .WithColor(new DiscordColor("#2B2D31"))
                    .WithDescription(
                        $"### Você reprovou a Allowlist de <@{User.Id}>\n\n" +
                        $"Uma mensagem foi enviada para <#{Service.Config.Channels.ReprovedId}> avisando-o,\n" +
                        $"com os seguintes motivos..." +
                        $"```\n{reason}\n```\n\n" +
                        $"**Este canal será excluido em 30 segundos**")
                    .Build());

                await Guild.GetChannel((ulong)Service.Config.Channels.ReprovedId)
                    .SendMessageAsync(new DiscordMessageBuilder()
                    .WithContent($"> * <@{User.Id}> Sua Allowlist foi reprovada pelos seguintes motivos:")
                    .WithEmbed(new DiscordEmbedBuilder()
                    .WithColor(new DiscordColor("#2B2D31"))
                    .WithDescription($"```\n{reason}\n```\n**Releia as regras** [clicando aqui]({Service.Config.Messages.MainMessage.ButtonLink})!")
                    .Build()));

                _ = Task.Run(async () =>
                {
                    await Task.Delay(30000);
                    await Interaction.Channel.DeleteAsync();
                });
            }
            else
            {
                await Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().AddEmbeds(await GetEmbedsWithUserInformations()));

                await Interaction.Channel.SendMessageAsync(
                    new DiscordEmbedBuilder()
                    .WithColor(new DiscordColor("#2B2D31"))
                    .WithDescription(
                        $"### Não foi possível reprovar a Allowlist." +
                        $"O usuário <@{User.Id}> não faz mais parte do discord!\n\n" +
                        $"**Este canal será excluido em 30 segundos**")
                    .Build());
                _ = Task.Run(async () =>
                {
                    await Task.Delay(30000);
                    await Interaction.Channel.DeleteAsync();
                });
            }
        }
        public async Task AllowlistReprovedModal()
        {
            await Interaction.CreateResponseAsync(InteractionResponseType.Modal, new DiscordInteractionResponseBuilder()
               .WithCustomId("modal_Reproved")
               .WithTitle("MOTIVOS PELO QUAL O REPROVOU")
               .AddComponents(new TextInputComponent("Seja objetivo!", "AlReasons", "- motivo um\n- motivo dois\n- motivo três", required: true, style: TextInputStyle.Paragraph, max_length: 4000)));
        }
        private async Task FinalizeAllowlist(bool approved)
        {
            if (approved)
            {
                await SendFormToReader();
                await Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                    .WithColor(new DiscordColor("#2B2D31"))
                    .WithDescription(
                        "### > Parabêns você finalizou sua aplicação para a Allowlist!\n" +
                        $"Seu formulário foi enviado para os nossos <@&{Service.Config.Roles.ReaderId}>,\n" +
                        "agora basta esperar eles leiam sua lore, e em breve você ira\n" +
                        $"receber uma notificação em <#{Service.Config.Channels.ApprovedId}> se aprovado.\n\n" +
                        "> * **Seu canal será deletado em 30 segundos.**")
                    .WithFooter("📄 Sistema de Allowlist - Kansas Roleplay")
                    .Build()));

                await Service.Data[User.Id].SubmitFinishAllowlistTime();
                await SetRoleSentAllowlist();

                _ = Task.Run(async () =>
                {
                    await Task.Delay(30000);
                    await Service.Data[User.Id].AllowListChannel.DeleteAsync();
                });
            }
            else
            {
                await Service.Data[User.Id].SubmitFinishAllowlistTime();

                await Guild.GetChannel((ulong)Service.Config.Channels.ReprovedId).SendMessageAsync($"> * <@{User.Id}> Sua Allowlist foi reprovada... Releia as regras do servidor [clicando aqui]({Service.Config.Messages.MainMessage.ButtonLink}), e tente novamente assim que se sentir confiante.");

                await Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                    .WithColor(new DiscordColor("#2B2D31"))
                    .WithDescription(
                        "### > Infelizmente você não passou em nosso questionário\n" +
                        $"Você não conseguiu passar pelo questionário da **Allowlist**,\n" +
                        "mas não tem problema você pode tentar fazer novamente em\n" +
                        $"**{Service.Config.ReprovedWaitTime} minutos**, enquanto isto você pode rever nossas regras\n" +
                        $"[clicando aqui]({Service.Config.Messages.MainMessage.ButtonLink}) ou no botão a baixo **\"Leia as Regras\"**\n" +
                        $"você pode verificar em <#{Service.Config.Channels.ReprovedId}> o motivo pelo qual\n" +
                        $"você foi reprovado na **Allowlist**!\n\n" +
                        "> * **Seu canal será deletado em 30 segundos.**")
                    .WithFooter("📄 Sistema de Allowlist - Kansas Roleplay")
                    .Build())
                    .AddComponents(await GetButtonRules()));
                await ShowAllowlistChannel();

                _ = Task.Run(async () =>
                {
                    await Task.Delay(30000);
                    await Service.Data[User.Id].AllowListChannel.DeleteAsync();
                });
            }
        }
        private async Task SendFormToUser()
        {
            await Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().AddEmbed(await GetEmbedToStartForm()).AddComponents(await GetButtonForm()));
        }
        private async Task SendFormToReader()
        {
            var readerCategories = Guild.GetChannelsAsync().GetAwaiter().GetResult().ToList().FindAll(_ => _.IsCategory == true && _.Name.StartsWith("reader-"));
            var category = readerCategories.First();
            foreach (var tempReaderCategory in readerCategories)
            {
                if (tempReaderCategory.Children.Count < category.Children.Count) { category = tempReaderCategory; }
            }
            await Guild.CreateChannelAsync(
                name: $"allowlist-{User.Id}",
                type: ChannelType.Text,
                parent: category).GetAwaiter().GetResult()
                .SendMessageAsync(new DiscordMessageBuilder()
                .AddEmbeds(await GetEmbedsWithUserInformations())
                .AddComponents(new List<DiscordComponent>(){
                    new DiscordButtonComponent(ButtonStyle.Danger, "btn_AlReproved", "Reprovar Allowlist", false),
                    new DiscordButtonComponent(ButtonStyle.Success, "btn_AlApproved", "Aprovar Allowlist", false)}));
        }
        private async Task CreateAllowlistChannel()
        {
            Member = await Guild.GetMemberAsync(User.Id);
            await Service.Data[User.Id].SubmitStartAllowlistTime();

            await Service.Data[User.Id].SetChannel(Guild.CreateChannelAsync(
                name: $"allowlist-{Member.DisplayName}",
                type: ChannelType.Text,
                parent: Guild.GetChannel((ulong)Service.Config.Channels.CategoryId),
                overwrites: new List<DiscordOverwriteBuilder>() {
                    new DiscordOverwriteBuilder()
                    .For(Member)
                    .Allow(Permissions.ReadMessageHistory)
                    .Allow(Permissions.AccessChannels)
                    .Deny(Permissions.SendMessages),
                    new DiscordOverwriteBuilder()
                    .For(Guild.EveryoneRole)
                    .Deny(Permissions.AccessChannels)}).GetAwaiter().GetResult()
                    .SendMessageAsync(
                new DiscordMessageBuilder()
                .AddEmbed(await GetEmbedCurrentQuestion())
                .AddComponents(await GetSelectOptionsCurrentQuestion())).GetAwaiter().GetResult().Channel);


            await Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                .WithColor(new DiscordColor("#2B2D31"))
                .WithDescription(
                    $"> O canal <#{Service.Data[User.Id].AllowListChannel.Id}> foi criado para você fazer sua **Allowlist**,\n" +
                    "> a partir deste ponto você tem **15 minutos** para enviar sua **Allowlist**,\n" +
                    "> após este tempo seu canal será **excluido** e você terá que refazer\n" +
                    "> sua **Allowlist**, a equipe do Kansas agradece e lhe deseja boa sorte!")).AsEphemeral(true));

            await HideAllowlistChannel();
        }
        private async Task UpdateMessageQuiz()
        {
            await Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().AddEmbed(await GetEmbedCurrentQuestion()).AddComponents(await GetSelectOptionsCurrentQuestion()));
        }
        private async Task<bool> QuizApproved()
        {
            for (int i = 0; i < Service.Config.Questions.Length; i++)
            {
                if (Service.Data[User.Id].Response[i] != Service.Config.Questions[i].CorrectAnswer) { return false; }
            }
            return true;
        }
        private async Task SetRoleSentAllowlist()
        {
            List<DiscordRole> roles = Member.Roles.ToList();
            roles.Add(Guild.GetRole((ulong)Service.Config.Roles.AllowlistSentId));
            await Member.ModifyAsync(_ => { _.Roles = roles; });
        }
        private async Task<bool> TryModifyApprovedAllowlistUserAsync()
        {
            if (await Guild.GetMemberAsync(User.Id) != null)
            {
                List<DiscordRole> roles = Member.Roles.ToList();
                if (roles.Contains(Guild.GetRole((ulong)Service.Config.Roles.AllowlistSentId)))
                {
                    roles.Remove(Guild.GetRole((ulong)Service.Config.Roles.AllowlistSentId));
                }
                roles.Add(Guild.GetRole((ulong)Service.Config.Roles.WaitingInterviewId));
                await Member.ModifyAsync(_ => _.Roles = roles);
                return true;
            }
            return false;
        }
        private async Task<bool> TryModifyReprovedAllowlistUserAsync()
        {
            if (await Guild.GetMemberAsync(User.Id) != null)
            {
                List<DiscordRole> roles = Member.Roles.ToList();
                if (roles.Contains(Guild.GetRole((ulong)Service.Config.Roles.AllowlistSentId)))
                {
                    roles.Remove(Guild.GetRole((ulong)Service.Config.Roles.AllowlistSentId));
                }
                await Member.ModifyAsync(_ => _.Roles = roles);
                await ShowAllowlistChannel();
                return true;
            }
            return false;
        }
        private async Task HideAllowlistChannel()
        {
            await Guild.GetChannel((ulong)Service.Config.Channels.MainId).AddOverwriteAsync(Member, Permissions.None, Permissions.AccessChannels);
        }
        private async Task ShowAllowlistChannel()
        {
            await Guild.GetChannel((ulong)Service.Config.Channels.MainId).DeleteOverwriteAsync(Member);
        }
        private Task<List<DiscordEmbed>> GetEmbedsWithUserInformations()
        {
            return Task.FromResult(new List<DiscordEmbed>(){
                new DiscordEmbedBuilder()
                .WithColor(new DiscordColor("#2B2D31"))
                .WithDescription(
                    "### > `userinfo [ignore]`\n" +
                    $"user: <@{User.Id}>\n" +
                    $"user_id: {User.Id}\n")
                .Build(),
                new DiscordEmbedBuilder()
                .WithColor(new DiscordColor("#2B2D31"))
                .WithDescription(
                    $"### > Você esta lendo a Allowlist de {Member.DisplayName}\nㅤ\n" +
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
                .AddField("> NOME REAL", $"**Nome:**\n`{Service.Data[User.Id].UserName}`", true)
                .AddField("> IDADE REAL", $"**Idade:**\n`{Service.Data[User.Id].UserAge}`", true)
                .AddField("> EXPERIÊNCIA", $"**Experiência:**\n`{Service.Data[User.Id].UserExp}`", true)
                .AddField( "ㅤ", "**INFORMAÇÕES SOBRE O PERSONAGEM**", false)
                .AddField("> NOME DO PERSONAGEM", $"**Nome:**\n`{Service.Data[User.Id].CharName}`",true)
                .AddField("> IDADE DO PERSONAGEM", $"**Idade:**\n`{Service.Data[User.Id].CharAge}`", true)
                .Build(),
                new DiscordEmbedBuilder()
                .WithColor(new DiscordColor("#2B2D31"))
                .WithTitle("> LORE DO PERSONAGEM")
                .WithDescription($"```\n{Service.Data[User.Id].CharLore}\n```")
                .Build()
            });
        }
        private Task<DiscordEmbed> GetEmbedToStartForm()
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
                $"Esses dados serão enviados para nossos <@&{Service.Config.Roles.ReaderId}> para que\n" +
                $"avaliem sua história e se condiz com o contexto em que se passa o roleplay,\n" +
                "após esta etapa se você passar iremos marcar a entrevista com você.")
                .WithFooter("📄 Sistema de Allowlist - Kansas Roleplay")
                .Build());
        }
        private Task<DiscordButtonComponent> GetButtonForm()
        {
            if (Form < 1) { return Task.FromResult(new DiscordButtonComponent(ButtonStyle.Success, "btn_openRealInfoModal", "Abrir formulário sobre você", false)); }
            else { return Task.FromResult(new DiscordButtonComponent(ButtonStyle.Success, "btn_openCharInfoModal", "Abrir formulário do personagem", false)); }
        }
        private Task<DiscordLinkButtonComponent> GetButtonRules()
        {
            return Task.FromResult(new DiscordLinkButtonComponent(Service.Config.Messages.MainMessage.ButtonLink, "Leia as Regras", false, null));
        }
        private Task<DiscordEmbed> GetEmbedCurrentQuestion()
        {
            string embedContent =
                $"### > Você esta fazendo a Allowlist para o Kansas Roleplay: {Service.Data[User.Id].CurrentQuestion + 1}/{Service.Config.Questions.Length}\n" +
                "-------------------------------------------------------------------------------------\n" +
                $"**{Service.Config.Questions[(int)Service.Data[User.Id].CurrentQuestion].Question}**";

            char a = 'A';
            foreach (string alt in Service.Config.Questions[(int)Service.Data[User.Id].CurrentQuestion].Alternatives)
            {
                embedContent += $"\n\n**{a} -** `{alt}`";
                a++;
            }

            DiscordEmbedBuilder eb = new DiscordEmbedBuilder()
               .WithColor(new DiscordColor("#2B2D31"))
               .WithDescription(embedContent);
            return Task.FromResult(eb.Build());
        }
        private Task<DiscordSelectComponent> GetSelectOptionsCurrentQuestion()
        {
            char a = 'A';
            int i = 1;
            List<DiscordSelectComponentOption> selectOptions = new List<DiscordSelectComponentOption>();
            selectOptions.Clear();
            foreach (string alt in Service.Config.Questions[(int)Service.Data[User.Id].CurrentQuestion].Alternatives)
            {
                selectOptions.Add(new DiscordSelectComponentOption($"{a} - {alt}", $"{i}", null, false, null));
                i++;
                a++;
            }
            return Task.FromResult(new DiscordSelectComponent($"select_AlAlternativesResponse", "Escolha a alternativa correta!", selectOptions, false, 1, 1));
        }
    }
}