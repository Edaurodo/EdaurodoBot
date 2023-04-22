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
            Form = 1;
        }
        public async Task ExecuteAsync()
        {
            if (Service.Data[User.Id].Atempt == null || Service.Data[User.Id].Atempt < 3)
            {
                Member = await Guild.GetMemberAsync(User.Id);
                await Service.Data[User.Id].IncrementAtempt();
                await Service.Data[User.Id].SetChannel(await CreateChannel());
                await Service.Data[User.Id].SetMessage(
                await Service.Data[User.Id].AllowListChannel
                .SendMessageAsync(new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                .WithColor(new DiscordColor("#2B2D31"))
                .WithTitle("> Você está preste a iniciar a primeira etapa da Allowlist")
                .WithDescription(
                "ㅤ\n" +
                "> `COMO VAI FUNCIONAR ?`\n" +
                "A **Allowlist** é feita em 3 partes você está na primeira\n" +
                "parte deste processo, onde você pássara por um questionário,\n" +
                "este questionário é de múltipla escolha, se passar você\n" +
                "terá que fornecer alguns dados sobre você e seu personagem,\n" +
                "na segunda segunda parte deste processo!\n\n" +
                "> `PRESTE MUITA ATENÇÃO`\n" +
                "Prepare sua história para ter no máximo **4000 CARACTERES**, `NÃO\n" +
                "É PERMITIDO` o envio de `LINKS` ou de `ARQUIVOS` nas respostas da\n" +
                "sua **ALLOWLIST**, caso seja enviado a sua **ALLOWLIST** será\n" +
                "reprovada na **HORA**.\n\n" +
                "> `ENTÃO ESTA TUDO PRONTO ?`\n" +
                "Se você já esta com tudo pronto e já leu todas as nossas regras,\n" +
                "basta clicar no botão **INICIAR**\n" +
                $"Boa sorte! <@{User.Id}>")
                .WithFooter("📄 Sistema de Allowlist - Kansas Roleplay")
                .Build())
                .AddComponents(new[] { new DiscordButtonComponent(ButtonStyle.Success, "btn_AlStartQuiz", "Iniciar Allowlist", false) })));

                await Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                .WithColor(new DiscordColor("#2B2D31"))
                .WithDescription(
                    $"> O canal <#{Service.Data[User.Id].AllowListChannel.Id}> foi criado para você fazer sua **Allowlist**,\n" +
                    "> a partir deste ponto você tem **15 minutos** para enviar sua **Allowlist**,\n" +
                    "> após este tempo seu canal será **excluido** e você terá que refazer\n" +
                    "> sua **Allowlist**, a equipe do Kansas agradece e lhe deseja boa sorte!"))
                .AsEphemeral(true));
            }
            else { return; }
        }
        public async Task ExecuteQuizAsync()
        {
            await Service.Data[User.Id].IncrementCurrentQuestion();
            if (Service.Data[User.Id].CurrentQuestion < Service.Config.Quests.Length) { await QuizUpdateMessage(); }
            else
            {
                if (await QuizApproved()) { await SendFormToUser(); }
                else { }
            }
        }
        public async Task SendFormToUser()
        {
            await Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().AddEmbed(GetEmbedModal()).AddComponents(GetComponentEmbedModal()));
        }
        private DiscordEmbed GetEmbedModal()
        {
            return new DiscordEmbedBuilder()
                    .WithColor(new DiscordColor("#2B2D31"))
                    .WithTitle("> Parabéns você passou para a segunda etapa da Allowlist\n")
                    .WithDescription(
                        "Nesta etapa eu vou pedir para que você conte-nos um pouco\n" +
                        "mais sobre você e seu personagem, para isso seria bom que\n" +
                        "você já tenha consigo em mãos a história de seu personagem\n\n" +
                        "> * Tente focar em traços fortes dele como suas característica;\n" +
                        "> * Lembre-se é claro no contexto em que o universo se passa;\n" +
                        "> * Evite histórias genéricas como \"minha familía morreu por isso sou assim\";\n\n" +
                        "Assim que estiver pronto clique em **\"Abrir formulário\"** e envie seus dados\n" +
                        $"eles serão enviados para os nossos <@&{Service.Config.Roles.ReaderRoleId}>,\n" +
                        "após esta etapa se você passar iremos marcar a entrevista com você.")
                    .WithFooter("📄 Sistema de Allowlist - Kansas Roleplay")
                    .Build();

        }
        private DiscordButtonComponent GetComponentEmbedModal()
        {
            if (Form == 1) { return new DiscordButtonComponent(ButtonStyle.Success, "btn_openRealInfoModal", "Abrir formulário", false); }
            else { return new DiscordButtonComponent(ButtonStyle.Success, "btn_openCharInfoModal", "Abrir formulário", false); }
        }
        public Task ChangeForm()
        {
            Console.WriteLine(Form);
            Form = 0;
            Console.WriteLine(Form);
            return Task.CompletedTask;
        }
        public async Task OpenRealInfoModal()
        {
            await Interaction.CreateResponseAsync(InteractionResponseType.Modal, new DiscordInteractionResponseBuilder()
                .WithCustomId("modal_RealInfoModal")
                .WithTitle("INFORMAÇÕES SOBRE VOCÊ")
                .AddComponents(new TextInputComponent("Qual é o seu nome real ?", "AlRealName", "Seu nome completo", required: true, style: TextInputStyle.Short, max_length: 50))
                .AddComponents(new TextInputComponent("Qual é a sua idade real ?", "AlRealAge", "Somente números (EX: 18)", required: true, style: TextInputStyle.Short, max_length: 2))
                .AddComponents(new TextInputComponent("Você já teve experiência com Roleplay?", "AlExp", "Se \"SIM\" quanto tempo?", required: true, style: TextInputStyle.Short, max_length: 20)));
        }
        public async Task OpenCharInfoModal()
        {
            await Interaction.CreateResponseAsync(InteractionResponseType.Modal, new DiscordInteractionResponseBuilder()
                .WithCustomId("modal_CharInfoModal")
                .WithTitle("INFORMAÇÕES SOBRE SEU PERSONAGEM")
                .AddComponents(new TextInputComponent("Qual o nome do seu personagem ?", "AlCharName", "Nome do seu personagem", required: true, style: TextInputStyle.Short, max_length: 50))
                .AddComponents(new TextInputComponent("Qual a idade do seu personagem ?", "AlCharAge", "Somente números (EX: 18)", required: true, style: TextInputStyle.Short, max_length: 3))
                .AddComponents(new TextInputComponent("Conte-nos mais sobre seu personagem.", "AlCharLore", "Conte-nos sobre as caracteristicas e sobre a história de seu personagem", required: true, style: TextInputStyle.Paragraph, max_length: 4000)));
        }
        public Task UpdateInteraction(DiscordInteraction interaction)
        {
            Interaction = interaction;
            return Task.CompletedTask;
        }
        public async Task FinalizeAllowlistAsync(bool approved)
        {
            if (approved)
            {
                await SendFormToReader();

                await Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                    .WithColor(new DiscordColor("#2B2D31"))
                    .WithTitle("> Parabêns você finalizou sua aplicação para a Allowlist")
                    .WithDescription(
                        $"Seu formulário foi enviado para os nossos <@&{Service.Config.Roles.ReaderRoleId}>,\n" +
                        "agora basta esperar eles ler sua lore, e em breve você ira\n" +
                        $"receber uma notificação em <#{Service.Config.Channels.ApprovedChannelId}> se aprovado.\n\n" +
                        "> * **Seu canal será **deletado** em 1 minuto.**")
                    .WithFooter("📄 Sistema de Allowlist - Kansas Roleplay")
                    .Build()));
                await Task.Delay(60000);
                await Service.Data[User.Id].AllowListChannel.DeleteAsync();
            }
            else { }
        }
        private async Task SendFormToReader()
        {
            var readerCategoryes = Guild.GetChannelsAsync().GetAwaiter().GetResult().ToList().FindAll(_ => _.IsCategory == true && _.Name.StartsWith("reader-"));

            var category = readerCategoryes.First();
            foreach (var tempReaderCategory in readerCategoryes)
            {
                if (tempReaderCategory.Children.Count < category.Children.Count) { category = tempReaderCategory; }
            }
            await Guild.CreateChannelAsync(
                name: $"allowlist-{Member.DisplayName}",
                type: ChannelType.Text,
                parent: category).GetAwaiter().GetResult()
                .SendMessageAsync(new DiscordMessageBuilder()
                .AddEmbeds(new List<DiscordEmbed>(){
                    new DiscordEmbedBuilder()
                .WithColor(new DiscordColor("#2B2D31"))
                .WithTitle($"Você esta lendo Allowlist de {Member.DisplayName}")
                .WithDescription(
                        "ㅤ\n" +
                        "> **Leia com todo cuidado e seja detalhista**\n\n" +
                        "Você é o filtro para fazer o servidor um local imersivo e\n" +
                        "seguro para todos os jogadores, leia sem pressa e se precisar\n" +
                        "tome notas de dúvidas que você teve sobre a história do personagem,\n" +
                        "na hora da entrevista seja atencioso(a) e explique para os player sobre\n" +
                        "o servidor e sobre o roleplay, pois muitos será a primeira vez jogando.\n")
                .Build(),
                new DiscordEmbedBuilder()
                .WithColor(new DiscordColor("#2B2D31"))
                .WithDescription("**INFORMÇÕES PESSOAIS**")
                .AddField("> NOME REAL", $"**Nome:**\n`{Service.Data[User.Id].UserName}`", true)
                .AddField("> IDADE REAL", $"**Idade:**\n`{Service.Data[User.Id].UserAge}`", true)
                .AddField("> EXPERIÊNCIA", $"**Experiência:**\n`{Service.Data[User.Id].UserExp}`", true)
                .AddField( "ㅤ", "**INFORMAÇÕES SOBRE O PERSONAGEM**", false)
                .AddField("> NOME DO PERSONAGEM", $"**Nome:**\n`{Service.Data[User.Id].CharName}`",true)
                .AddField("> IDADE DO PERSONAGEM", $"**Idade:**\n`{Service.Data[User.Id].CharAge}`", true)
                .Build(),
                new DiscordEmbedBuilder()
                .WithColor(new DiscordColor("#2B2D31"))
                .WithTitle("LORE DO PERSONAGEM\nㅤ")
                .WithDescription($"```{Service.Data[User.Id].CharLore}```")
                .Build(),
                new DiscordEmbedBuilder()
                .WithColor(new DiscordColor("#2B2D31"))
                .WithTitle("userinfo [ignore]")
                .WithDescription($"user_id: {User.Id}\n")
                .Build()})
                .AddComponents(new List<DiscordComponent>(){
                new DiscordButtonComponent(ButtonStyle.Danger, "btn_AlReproved", "REPROVAR", false),
                new DiscordButtonComponent(ButtonStyle.Success, "btn_AlApproved", "APROVAR", false)}));
        }
        private async Task QuizUpdateMessage()
        {
            await Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().AddEmbed(QuizEmbed()).AddComponents(QuizComponent()));
        }
        private DiscordEmbed QuizEmbed()
        {
            DiscordEmbedBuilder eb = new DiscordEmbedBuilder()
               .WithColor(new DiscordColor("#2B2D31"))
               .WithTitle($"Você esta fazendo a Allowlist do Kansas Roleplay: {Service.Data[User.Id].CurrentQuestion + 1}/{Service.Config.Quests.Length}")
               .WithDescription("ㅤ");

            string alternatives = "ㅤ\n";
            char a = 'A';
            foreach (string alt in Service.Config.Quests[(int)Service.Data[User.Id].CurrentQuestion].Alternatives)
            {
                alternatives += $"{a} ) {alt}\nㅤ\n";
                a++;
            }
            eb.AddField(Service.Config.Quests[(int)Service.Data[User.Id].CurrentQuestion].QuestName, alternatives, false);
            return eb.Build();
        }
        private DiscordSelectComponent QuizComponent()
        {
            char a = 'A';
            int i = 1;
            List<DiscordSelectComponentOption> selectOptions = new List<DiscordSelectComponentOption>();
            selectOptions.Clear();
            foreach (string alt in Service.Config.Quests[(int)Service.Data[User.Id].CurrentQuestion].Alternatives)
            {
                selectOptions.Add(new DiscordSelectComponentOption($"{a} ) {alt}", $"{i}", null, false, null));
                i++;
                a++;
            }
            return new DiscordSelectComponent($"select_AlAlternativesResponse", "Escolha a alternativa correta!", selectOptions, false, 1, 1);
        }
        private async Task<bool> QuizApproved()
        {
            for (int i = 0; i < Service.Config.Quests.Length; i++)
            {
                if (Service.Data[User.Id].Response[i] != Service.Config.Quests[i].QuestCorrectResponse) { return false; }
            }
            return true;
        }
        private IEnumerable<DiscordOverwriteBuilder> GetOverwriteChannel()
        {
            List<DiscordOverwriteBuilder> list = new List<DiscordOverwriteBuilder>() {
            new DiscordOverwriteBuilder()
            .For(Member)
            .Allow(Permissions.ReadMessageHistory)
            .Allow(Permissions.AccessChannels),
            new DiscordOverwriteBuilder()
            .For(Guild.EveryoneRole)
            .Deny(Permissions.AccessChannels)
            };
            return list;
        }
        private async Task HideAllowlistChannel()
        {
            await Guild.GetChannel((ulong)Service.Config.Channels.MainChannelId).AddOverwriteAsync(Member, Permissions.None, Permissions.AccessChannels);
        }
        private async Task<DiscordChannel> CreateChannel()
        {
            await HideAllowlistChannel();
            return await Guild.CreateChannelAsync(
                name: $"allowlist-{Member.DisplayName}",
                type: ChannelType.Text,
                parent: Guild.GetChannel((ulong)Service.Config.Channels.CategoryChannelId),
                overwrites: GetOverwriteChannel());
        }
    }
}