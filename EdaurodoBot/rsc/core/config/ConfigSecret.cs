namespace EdaurodoBot.rsc.core.config
{
    public sealed class ConfigSecret
    {
        public ulong GuildId { get; private set; }

        public ConfigSecret()
        {
            //this.GuildId = 1091496219706134528; //Kansas
            GuildId = 1097821267085758504; //Dummy World - Test Server
        }
    }
}
