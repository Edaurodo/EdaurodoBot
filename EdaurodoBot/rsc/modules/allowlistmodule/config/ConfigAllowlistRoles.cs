using Newtonsoft.Json;
namespace EdaurodoBot.rsc.modules.allowlistmodule.config
{
    public sealed class ConfigAllowlistRoles
    {
        [JsonProperty("reader_id")]
        public ulong? ReaderId { get; private set; }

        [JsonProperty("resident_id")]
        public ulong? ResidentId { get; private set; }

        [JsonProperty("approved_id")]
        public ulong? ApprovedId { get; private set; }

        [JsonProperty("waiting_interview_id")]
        public ulong? WaitingInterviewId { get; private set; }

        [JsonProperty("allowlist_sent_id")]
        public ulong? AllowlistSentId { get; private set; }

        [JsonProperty("default_id")]
        public ulong? DefaultId { get; private set; }

        public ConfigAllowlistRoles() {
            ReaderId = null;
            ResidentId = null;
            ApprovedId = null;
            WaitingInterviewId = null;
            AllowlistSentId = null;
            DefaultId = null;
        }
    }
}
