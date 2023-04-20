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

        public Allowlist(AllowlistService service, InteractionCreateEventArgs sender)
        {
            Service = service;
            Sender = sender;
            Interaction = Sender.Interaction;
            User = Interaction.User;
            Guild = Interaction.Guild;
            Member = null;
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
                .AddComponents(new[] { new DiscordButtonComponent(ButtonStyle.Success, "btn_alstartquiz", "Iniciar Allowlist", false) })));

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
            if (Service.Data[User.Id].CurrentQuestion < Service.Config.QuestConfig.Length) { await QuizUpdateMessage(); }
            else { await QuizFinallize(); }
        }
        public async Task OpenQuizModal()
        {
            await Interaction.CreateResponseAsync(InteractionResponseType.Modal, new DiscordInteractionResponseBuilder()
                .WithCustomId("modal_Alquiz")
                .WithTitle("Formulário Kansas Roleplay")
                .AddComponents(new TextInputComponent("Qual é a sua idade real ?", "Alrealage", "Somente números (EX: 18)", required: true, style: TextInputStyle.Short, max_length: 2))
                .AddComponents(new TextInputComponent("Você já teve experiência com Roleplay?", "Alrpexp", "Se \"SIM\" quanto tempo?",required: true, style: TextInputStyle.Short, max_length: 10))
                .AddComponents(new TextInputComponent("Qual o nome do seu personagem ?", "Alcharname", "Nome do seu personagem", required: true, style: TextInputStyle.Short, max_length: 50))
                .AddComponents(new TextInputComponent("Qual a idade do seu personagem ?", "Alcharage", "Somente números (EX: 18)", required: true, style: TextInputStyle.Short, max_length: 3))
                .AddComponents(new TextInputComponent("Conte-nos mais sobre seu personagem.", "Alcharlore", "Conte-nos sobre as caracteristicas e sobre a história de seu personagem", required: true, style: TextInputStyle.Paragraph, max_length: 4000)));
        }
        public Task UpdateInteraction(DiscordInteraction interaction)
        {
            Interaction = interaction;
            return Task.CompletedTask;
        }
        public async Task FinalizeAllowlistAsync(bool approved)
        {
            if (approved) { } else { }
        }
        private async Task QuizFinallize()
        {
            if(await QuizResponseCheck()) {
                await
                    Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage,
                    new DiscordInteractionResponseBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                    .WithColor(new DiscordColor("#2B2D31"))
                    .WithTitle("> Parabéns você passou para a segunda etapa da Allowlist\n")
                    .WithDescription(
                        "ㅤ\n" +
                        "nesta etapa eu vou pedir para que você conte-nos um pouco\n" +
                        "mais sobre você e seu personagem, para isso seria bom que\n" +
                        "você já tenha consigo em mão a história de seu personagem\n\n" +
                        "> * Tente focar em traços fortes dele como suas característica;\n" +
                        "> * Lembre-se é claro no contexto em que o universo se passa;\n" +
                        "> * Evite histórias genéricas como \"minha familía morreu por isso sou assim\";\n\n" +
                        "Assim que estiver pronto clique em **\"Abrir formulário\"** e envie seus dados\n" +
                        $"eles serão enviados para os nossos <@&{Service.Config.RolesConfig.ReaderRoleId}>,\n" +
                        "após esta etapa se você passar iremos marcar a entrevista com você.")
                    .Build())
                    .AddComponents(new DiscordButtonComponent(ButtonStyle.Success, "btn_alopenmodal", "Abrir formulário", false)));
            }
        }
        private async Task QuizUpdateMessage()
        {
            await Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().AddEmbed(QuizEmbed()).AddComponents(QuizComponent()));
        }
        private DiscordEmbed QuizEmbed()
        {
            DiscordEmbedBuilder eb = new DiscordEmbedBuilder()
               .WithColor(new DiscordColor("#2B2D31"))
               .WithTitle($"Você esta fazendo a Allowlist do Kansas Roleplay: {Service.Data[User.Id].CurrentQuestion + 1}/{Service.Config.QuestConfig.Length}")
               .WithDescription("ㅤ");

            string alternatives = "ㅤ‎\n";
            char a = 'A';
            foreach (string alt in Service.Config.QuestConfig[(int)Service.Data[User.Id].CurrentQuestion].QuestAlternative)
            {
                alternatives += $"{a} ) {alt}\nㅤ\n";
                a++;
            }
            eb.AddField(Service.Config.QuestConfig[(int)Service.Data[User.Id].CurrentQuestion].QuestName, alternatives, false);
            return eb.Build();
        }
        private DiscordSelectComponent QuizComponent()
        {
            char a = 'A';
            int i = 1;
            List<DiscordSelectComponentOption> selectOptions = new List<DiscordSelectComponentOption>();
            selectOptions.Clear();
            foreach (string alt in Service.Config.QuestConfig[(int)Service.Data[User.Id].CurrentQuestion].QuestAlternative)
            {
                selectOptions.Add(new DiscordSelectComponentOption($"{a} ) {alt}", $"{i}", null, false, null));
                i++;
                a++;
            }
            return new DiscordSelectComponent($"select_alalt", "Escolha a alternativa correta!", selectOptions, false, 1, 1);
        }
        private async Task<bool> QuizResponseCheck()
        {
            for (int i = 0; i < Service.Config.QuestConfig.Length; i++)
            {
                if (Service.Data[User.Id].Response[i] != Service.Config.QuestConfig[i].QuestCorrectResponse) { return false; }
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
            await Guild.GetChannel((ulong)Service.Config.ChannelConfig.MainChannelId).AddOverwriteAsync(Member, Permissions.None, Permissions.AccessChannels);
        }
        private async Task<DiscordChannel> CreateChannel()
        {
            await HideAllowlistChannel();
            return await Guild.CreateChannelAsync(
                name: $"allowlist-{Member.DisplayName}",
                type: ChannelType.Text,
                parent: Guild.GetChannel((ulong)Service.Config.ChannelConfig.CategoryChannelId),
                overwrites: GetOverwriteChannel());
        }
    }
}