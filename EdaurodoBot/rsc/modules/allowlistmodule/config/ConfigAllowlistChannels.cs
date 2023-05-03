using Newtonsoft.Json;
namespace EdaurodoBot.rsc.modules.allowlistmodule.config
{
    public sealed class ConfigAllowlistChannels
    {
        [JsonProperty("category_id")]
        public ulong? CategoryId { get; private set; }

        [JsonProperty("main_id")]
        public ulong? MainId { get; private set; }

        [JsonProperty("approved_id")]
        public ulong? ApprovedId { get; private set; }

        [JsonProperty("reproved_id")]
        public ulong? ReprovedId { get; private set; }

        [JsonProperty("interview_notice_id")]
        public ulong? InterviewId { get; private set; }

        [JsonProperty("changename_id")]
        public ulong? ChangeNameId { get; private set; }

        public ConfigAllowlistChannels()
        {
            CategoryId = null;
            MainId = null;
            ApprovedId = null;
            ReprovedId = null;
            InterviewId = null;
            ChangeNameId = null;
        }
    }
}
