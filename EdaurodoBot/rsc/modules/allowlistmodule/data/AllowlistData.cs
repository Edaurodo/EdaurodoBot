using DSharpPlus.Entities;
using EdaurodoBot.rsc.modules.allowlistmodule.enums;
using Newtonsoft.Json;

namespace EdaurodoBot.rsc.modules.allowlistmodule.data
{
    public sealed class AllowlistData
    {
        [JsonProperty("interaction")]
        public DiscordInteraction Interaction { get; private set; }

        [JsonProperty("user")]
        public DiscordUser User { get; private set; }

        [JsonProperty("guild")]
        public DiscordGuild Guild { get; private set; }

        [JsonProperty("member")]
        public DiscordMember Member { get; private set; }



        [JsonProperty("internal_responses_list")]
        private List<int>? ResponsesList;

        [JsonProperty("user_allowlist_channel")]
        public DiscordChannel? AllowlistUserChannel { get; private set; }

        [JsonProperty("start_time")]
        public DateTime? StartAllowlistTime { get; private set; }

        [JsonProperty("finish_time")]
        public DateTime? FinishAllowlistTime { get; private set; }

        [JsonProperty("current_question")]
        public int? CurrentQuestion { get; private set; }

        [JsonProperty("responses")]
        public int[]? Responses { get; private set; }

        [JsonProperty("user_name")]
        public string? UserName { get; private set; }

        [JsonProperty("user_age")]
        public string? UserAge { get; private set; }

        [JsonProperty("user_exp")]
        public string? UserExp { get; private set; }

        [JsonProperty("char_age")]
        public string? CharAge { get; private set; }

        [JsonProperty("char_name")]
        public string? CharName { get; private set; }

        [JsonProperty("char_lore")]
        public string? CharLore { get; private set; }

        [JsonProperty("current_form")]
        public Form CurrentForm { get; private set; }

        public AllowlistData(DiscordInteraction interaction)
        {
            Interaction = interaction;
            User = interaction.User;
            Guild = interaction.Guild;
            Member = interaction.Guild.Members[User.Id];
            CurrentForm = Form.User;
        }

        public Task UpdateInteraction(DiscordInteraction interaction)
        {
            Interaction = interaction;
            return Task.CompletedTask;
        }

        public Task ReprovedClearDataBase()
        {
            ResponsesList = null;
            AllowlistUserChannel = null;
            CurrentQuestion = null;
            Responses = null;
            CharAge = null;
            CharName = null;
            CharLore = null;
            CurrentForm = (Form)1;
            return Task.CompletedTask;
        }
        public Task ClearDataBase()
        {
            ResponsesList = null;
            AllowlistUserChannel = null;
            StartAllowlistTime = null;
            FinishAllowlistTime = null;
            CurrentQuestion = null;
            Responses = null;
            CharAge = null;
            CharName = null;
            CharLore = null;
            CurrentForm = (Form)1;
            return Task.CompletedTask;
        }

        public Task UpdateCurrentForm(Form form)
        {
            CurrentForm = form;
            return Task.CompletedTask;
        }
        public Task SubmitStartAllowlistTime()
        {
            StartAllowlistTime = DateTime.Now.ToUniversalTime();
            return Task.CompletedTask;
        }
        public Task SubmitFinishAllowlistTime()
        {
            FinishAllowlistTime = DateTime.Now.ToUniversalTime();
            return Task.CompletedTask;
        }
        public Task SubmitRealInfo(string? username, string? userage, string? userexp)
        {
            UserName = username;
            UserAge = userage;
            UserExp = userexp;
            return Task.CompletedTask;
        }
        public Task SubmitCharInfo(string? charage, string? charname, string? charlore)
        {
            CharName = charname;
            CharAge = charage;
            CharLore = charlore;
            return Task.CompletedTask;
        }
        public Task SubmitResponse(int response)
        {
            ResponsesList ??= new List<int>();
            ResponsesList.Add(response);
            Responses = ResponsesList.ToArray();
            return Task.CompletedTask;
        }
        public Task IncrementCurrentQuestion()
        {
            CurrentQuestion ??= -1;
            CurrentQuestion++;
            return Task.CompletedTask;
        }
        public Task SetAllowlistChannel(DiscordChannel channel)
        {
            AllowlistUserChannel = channel;
            return Task.CompletedTask;
        }
    }
}