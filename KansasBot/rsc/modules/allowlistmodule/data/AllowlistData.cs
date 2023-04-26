using DSharpPlus.Entities;
using KansasBot.rsc.modules.allowlistmodule.enums;
using Newtonsoft.Json;

namespace KansasBot.rsc.modules.allowlistmodule.data
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
            this.Interaction = interaction;
            this.User = interaction.User;
            this.Guild = interaction.Guild;
            this.Member = interaction.Guild.Members[User.Id];
            this.CurrentForm = Form.User;
        }

        public Task UpdateInteraction(DiscordInteraction interaction)
        {
            this.Interaction = interaction;
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
            this.CurrentForm = form;
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
            this.UserName = username;
            this.UserAge = userage;
            this.UserExp = userexp;
            return Task.CompletedTask;
        }
        public Task SubmitCharInfo(string? charage, string? charname, string? charlore)
        {
            this.CharName = charname;
            this.CharAge = charage;
            this.CharLore = charlore;
            return Task.CompletedTask;
        }
        public Task SubmitResponse(int response)
        {
            this.ResponsesList ??= new List<int>();
            this.ResponsesList.Add(response);
            this.Responses = ResponsesList.ToArray();
            return Task.CompletedTask;
        }
        public Task IncrementCurrentQuestion()
        {
            this.CurrentQuestion ??= -1;
            this.CurrentQuestion++;
            return Task.CompletedTask;
        }
        public Task SetAllowlistChannel(DiscordChannel channel)
        {
            this.AllowlistUserChannel = channel;
            return Task.CompletedTask;
        }
    }
}