using DSharpPlus.Entities;

namespace KansasBot.rsc.modules.allowlistmodule.data
{
    public sealed class AllowlistData
    {
        private List<uint>? ResponsesList;
        public Allowlist Allowlist { get; private set; }
        public DiscordChannel? AllowListChannel { get; private set; }
        public DateTime? StartAllowlistTime { get; private set; }
        public DateTime? FinishAllowlistTime { get; private set; }
        public int? CurrentQuestion { get; private set; }
        public uint[]? Responses { get; private set; }
        public string? UserName { get; private set; }
        public string? UserAge { get; private set; }
        public string? UserExp { get; private set; }
        public string? CharAge { get; private set; }
        public string? CharName { get; private set; }
        public string? CharLore { get; private set; }

        public AllowlistData(Allowlist allowlist) => this.Allowlist = allowlist; 
        public Task ReprovedClearDataBase()
        {
            ResponsesList = null;
            AllowListChannel = null;
            CurrentQuestion = null;
            Responses = null;
            CharAge = null;
            CharName = null;
            CharLore = null;
            return Task.CompletedTask;
        }
        public Task ClearDataBase()
        {
            ResponsesList = null;
            AllowListChannel = null;
            StartAllowlistTime = null;
            FinishAllowlistTime = null;
            CurrentQuestion = null;
            Responses = null;
            CharAge = null;
            CharName = null;
            CharLore = null;
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
        public Task SubmitResponse(uint response)
        {
            this.ResponsesList ??= new List<uint>();
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
        public Task SetChannel(DiscordChannel channel)
        {
            this.AllowListChannel = channel;
            return Task.CompletedTask;
        }
    }
}