using DSharpPlus.Entities;
using EdaurodoBot.rsc.modules.allowlistmodule.enums;
using Newtonsoft.Json;
using System.Diagnostics;

namespace EdaurodoBot.rsc.modules.allowlistmodule.data
{
    public sealed class AllowlistData
    {
        [JsonIgnore]
        public DiscordInteraction? Interaction { get; private set; }

        [JsonIgnore]
        public DiscordChannel? AllowlistUserChannel { get; private set; }

        [JsonIgnore]
        public int? CurrentQuestion { get; private set; }

        [JsonIgnore]
        public int[]? Responses { get; private set; }

        [JsonIgnore]
        public Form CurrentForm { get; private set; }

        [JsonIgnore]
        private List<int>? ResponsesList;


        [JsonProperty("start_time")]
        public DateTime? StartAllowlistTime { get; private set; }

        [JsonProperty("finish_time")]
        public DateTime? FinishAllowlistTime { get; private set; }

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

        [JsonProperty("user")]
        public DiscordUser DiscordUser { get; private set; }

        public AllowlistData(DiscordUser user)
        {
            DiscordUser = user;
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
            CurrentForm = Form.User;
            return Task.CompletedTask;
        }
        public async Task ClearDataBase()
        {
            await ReprovedClearDataBase();
            StartAllowlistTime = null;
            FinishAllowlistTime = null;
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