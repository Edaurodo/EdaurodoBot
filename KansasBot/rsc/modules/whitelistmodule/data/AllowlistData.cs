using DSharpPlus.Entities;

namespace KansasBot.rsc.modules.whitelistmodule.data
{
    public sealed class AllowlistData
    {
        private List<uint>? ResponseList;
        public Allowlist Allowlist { get; private set; }
        public DiscordChannel? AllowListChannel { get; private set; }
        public DiscordMessage? AllowListMessage { get; private set; }
        public byte? Atempt { get; private set; }
        public int? CurrentQuestion { get; private set; }
        public uint[]? Response { get; private set; }

        public string? UserAge { get; private set; }
        public string? UserRpExp { get; private set; }
        public string? CharAge { get; private set; }
        public string? CharName { get; private set; }
        public string? CharLore { get; private set; }
        public AllowlistData (Allowlist allowlist) => this.Allowlist = allowlist;


        public Task SubmitUserInfo(string? userage, string? userrpexp, string? charage, string? charname, string? charlore)
        {
            this.UserAge = userage;
            this.UserRpExp = userrpexp;
            this.CharAge = charage;
            this.CharName = charname;
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
        public Task IncrementAtempt()
        {
            this.Atempt ??= 1; 
            this.Atempt++;
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
