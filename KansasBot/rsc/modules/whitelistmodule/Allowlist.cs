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

        private DiscordClient Client;
        private DiscordGuild Guild;
        private DiscordInteraction Interaction;
        private DiscordUser User;
        private DiscordMember? Member;

        public Allowlist(AllowlistService service, InteractionCreateEventArgs sender)
        {
            Client = service.Bot.Client;
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
                Service.Data[User.Id].IncrementAtempt();
                Member = await Guild.GetMemberAsync(User.Id);
                await CreateChannel();
                Service.Data[User.Id].SetMessage(await Service.Data[User.Id].AllowListChannel.SendMessageAsync(new DiscordMessageBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                    .WithColor(new DiscordColor("#2B2D31"))
                    .WithDescription(
                    "teste teste teste teste teste teste teste teste teste teste\n" +
                    "teste teste teste teste teste teste teste teste teste teste\n" +
                    "teste teste teste teste teste teste teste teste teste teste\n" +
                    "teste teste teste teste teste teste teste teste teste teste\n" +
                    "teste teste teste teste teste teste teste teste teste teste\n")
                    .Build())
                    .AddComponents(new[] { new DiscordButtonComponent(ButtonStyle.Success, "btn_startquiz", "Iniciar", false) })));
            }
            else { return; }
        }
        public async Task ExecuteQuizAsync()
        {
            Service.Data[User.Id].IncrementCurrentQuestion();
            if (Service.Data[User.Id].CurrentQuestion <= Service.Config.QuestConfig.Length)
            {
                await Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().AddEmbed(GetQuizEmbed()).AddComponents(GetQuizComponent()));
            }
        }
        private DiscordEmbed GetQuizEmbed()
        {
            DiscordEmbedBuilder eb = new DiscordEmbedBuilder()
               .WithColor(new DiscordColor("#2B2D31"))
               .WithTitle($"Você esta fazendo a Allowlist do Kansas Roleplay: {Service.Data[User.Id].CurrentQuestion}/{Service.Config.QuestConfig.Length}")
               .WithDescription("ㅤ");


            string alternatives = "ㅤ‎\n";
            char a = 'A';
            foreach (string alt in Service.Config.QuestConfig[(int)Service.Data[User.Id].CurrentQuestion].QuestAlternative)
            {
                alternatives += $"{a} ) {alt}\nㅤ\n";
                a++;
            }
            eb.AddField(Service.Config.QuestConfig[(int)Service.Data[User.Id].CurrentQuestion - 1].QuestName, alternatives, false);
            return eb.Build();
        }
        private DiscordSelectComponent GetQuizComponent()
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
            return new DiscordSelectComponent($"select_alt", "Escolha a alternativa correta!", selectOptions, false, 1, 1);
        }

        public Task UpdateInteraction(DiscordInteraction interaction)
        {
            Interaction = interaction;
            return Task.CompletedTask;
        }
        private async Task CreateChannel()
        {
            Service.Data[User.Id].SetChannel(await Guild.CreateChannelAsync(
                $"allowlist-{Member.DisplayName}",
                ChannelType.Text,
                Guild.GetChannel((ulong)Service.Config.ChannelConfig.CategoryChannelId),
                default, null, null, GetOverwriteChannel(), null, default, null, null, null, null, null, null, null));
            await HideAllowlistChannel();
        }
        private async Task HideAllowlistChannel()
        {
            await Guild.GetChannel((ulong)Service.Config.ChannelConfig.MainChannelId).AddOverwriteAsync(Member, Permissions.None, Permissions.AccessChannels);
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
    }
}
