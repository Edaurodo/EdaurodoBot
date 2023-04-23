using DSharpPlus.Entities;

namespace KansasBot.rsc.modules.whitelistmodule.data
{
    public sealed class AllowlistData
    {
        private List<uint>? ResponseList;
        public Allowlist Allowlist { get; private set; }
        public DiscordChannel? AllowListChannel { get; private set; }
        public DiscordMessage? AllowListMessage { get; private set; }
        public DateTime? StartAllowlistTime { get; private set; }
        public DateTime? FinishAllowlistTime { get; private set; }
        public int? CurrentQuestion { get; private set; }
        public uint[]? Response { get; private set; }
        public string? UserName { get; private set; }
        public string? UserAge { get; private set; }
        public string? UserExp { get; private set; }
        public string? CharAge { get; private set; }
        public string? CharName { get; private set; }
        public string? CharLore { get; private set; }

        public AllowlistData(Allowlist allowlist) => this.Allowlist = allowlist;

        public Task ClearDataBase()
        {
            ResponseList = null;
            AllowListChannel = null;
            AllowListMessage = null;
            StartAllowlistTime = null;
            FinishAllowlistTime = null;
            CurrentQuestion = null;
            Response = null;
            UserName = null;
            UserAge = null;
            UserExp = null;
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
        public Task SetFinishAllowlistTimeNull()
        {
            FinishAllowlistTime = null;
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
            this.ResponseList ??= new List<uint>();
            this.ResponseList.Add(response);
            this.Response = ResponseList.ToArray();
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
        public Task SetMessage(DiscordMessage message)
        {
            this.AllowListMessage = message;
            return Task.CompletedTask;
        }
    }
}