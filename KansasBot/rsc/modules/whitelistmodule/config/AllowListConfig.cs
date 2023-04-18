using Newtonsoft.Json;

namespace KansasBot.rsc.modules.whitelistmodule.config
{
    public sealed class AllowListConfig
    {
        [JsonProperty("channels_config")]
        public AllowListChannel? ChannelConfig { get; private set; } = new AllowListChannel();

        [JsonProperty("roles_config")]
        public AllowListRoles? RolesConfig { get; private set; } = new AllowListRoles();

        [JsonProperty("quests")]
        public AllowListQuest[]? QuestConfig { get; private set; } = new[] {
            new AllowListQuest("Questão 1", new[] { "Alternativa 1", "Alternativa 2", "Quantas voce quiser desde que não ultrapasse 25" }, 2),
            new AllowListQuest("Questão 2", new[] { "Alternativa 1", "Alternativa 2", "Quantas voce quiser desde que não ultrapasse 25" }, 3)
        };
    }
    public sealed class AllowListRoles
    {
        [JsonProperty("reader_role_id")]
        public ulong? ReaderRoleId { get; private set; } = null;

        [JsonProperty("aproved_role_id")]
        public ulong? AprovedRoleId { get; private set; } = null;

        [JsonProperty("reproved_role_id")]
        public ulong? ReprovedRoleId { get; private set; } = null;
    }
    public sealed class AllowListChannel
    {
        [JsonProperty("allowlist_category_channel_id")]
        public ulong? CategoryChannelId { get; private set; } = null;

        [JsonProperty("main_channel_id")]
        public ulong? MainChannelId { get; private set; } = null;

        [JsonProperty("aproved_channel_id")]
        public ulong? AprovedChannelId { get; private set; } = null;

        [JsonProperty("reproved_channel_id")]
        public ulong? ReprovedChannelId { get; private set; } = null;
    }
    public struct AllowListQuest
    {
        [JsonProperty("question")]
        public string? QuestName { get; private set; }

        [JsonProperty("quest_alternatives")]
        public string[]? QuestAlternative { get; private set; }

        [JsonProperty("correct_alternative")]
        public uint? QuestCorrectResponse { get; private set; }

        public AllowListQuest(string? questName, string[]? questAlternative, uint? questCorrectResponse)
        {
            QuestName = questName;
            QuestAlternative = questAlternative;
            QuestCorrectResponse = questCorrectResponse;
        }
    }
}