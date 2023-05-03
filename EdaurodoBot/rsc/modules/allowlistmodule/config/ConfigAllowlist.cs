using Newtonsoft.Json;

namespace EdaurodoBot.rsc.modules.allowlistmodule.config
{
    public sealed class ConfigAllowlist
    {
        [JsonProperty("use_module")]
        public bool Use { get; private set; }

        [JsonProperty("changename_module")]
        public bool ChangeName { get; private set; }

        [JsonProperty("reproved_wait_time_from_minutes")]
        public int ReprovedWaitTime { get; private set; }


        [JsonProperty("channels")]
        public ConfigAllowlistChannels Channels { get; private set; }

        [JsonProperty("roles")]
        public ConfigAllowlistRoles Roles { get; private set; }

        [JsonProperty("questions")]
        public IEnumerable<AllowlistQuestion> Questions { get; private set; }

        [JsonProperty("messages")]
        public ConfigAllowlistMessages Messages { get; private set; }

        public ConfigAllowlist(bool? use, bool? changename, int? reprovedwaittime, ConfigAllowlistChannels? channels, ConfigAllowlistRoles? roles, IEnumerable<AllowlistQuestion>? questions, ConfigAllowlistMessages? messages)
        {
            Use = use ?? false;
            ChangeName = changename ?? false;
            ReprovedWaitTime = reprovedwaittime ?? 60;
            Channels = channels ?? new ConfigAllowlistChannels();
            Roles = roles ?? new ConfigAllowlistRoles();
            Questions = questions ?? new List<AllowlistQuestion>() { new AllowlistQuestion(null, null, null) };
            Messages = messages ?? new ConfigAllowlistMessages(null);
        }
    }
}