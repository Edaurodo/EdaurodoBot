using DSharpPlus.Entities;

namespace KansasBot.rsc.modules.whitelistmodule.data
{
    public sealed class AllowlistData
    {
        public Allowlist Allowlist { get; private set; }
        public DiscordChannel? AllowListChannel { get; private set; }
        public DiscordMessage? AllowListMessage { get; private set; }
        public byte? Age { get; private set; }
        public string? Lore { get; private set; }
        public byte? Atempt { get; private set; }
        public int? CurrentQuestion { get; private set; }
        public AllowlistData (Allowlist allowlist) => this.Allowlist = allowlist;

        public void IncrementCurrentQuestion()
        {
            this.CurrentQuestion ??= 0;
            this.CurrentQuestion++;
        }
        public void IncrementAtempt()
        {
            this.Atempt ??= 1; 
            this.Atempt++;
        }
        public void SetChannel(DiscordChannel channel)
        {
            this.AllowListChannel = channel;
        }
        public void SetMessage(DiscordMessage message)
        {
            this.AllowListMessage = message;
        }
    }
}
