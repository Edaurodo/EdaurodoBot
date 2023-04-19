using KansasBot.rsc.modules.genericmodule.commands.create.embed;
using Newtonsoft.Json;

namespace KansasBot.rsc.modules.whitelistmodule.config
{
    public sealed class AllowListConfig
    {
        [JsonProperty("channels_config", NullValueHandling = NullValueHandling.Include)]
        public AllowListChannel? ChannelConfig { get; private set; } = new AllowListChannel();

        [JsonProperty("roles_config", NullValueHandling = NullValueHandling.Include)]
        public AllowListRoles? RolesConfig { get; private set; } = new AllowListRoles();

        [JsonProperty("quests", NullValueHandling = NullValueHandling.Include)]
        public AllowListQuest[]? QuestConfig { get; private set; } = new[] {
            new AllowListQuest("Questão 1", new[] { "Alternativa 1", "Alternativa 2", "Quantas voce quiser desde que não ultrapasse 25" }, 2),
            new AllowListQuest("Questão 2", new[] { "Alternativa 1", "Alternativa 2", "Quantas voce quiser desde que não ultrapasse 25" }, 3)
        };

        [JsonProperty("messages", NullValueHandling = NullValueHandling.Include)]
        public AllowlistMessages? Messages { get; private set; } = new AllowlistMessages();
    }

    public sealed class AllowlistMessages
    {
        [JsonProperty("main_allowlist", NullValueHandling = NullValueHandling.Include)]
        public MainMessage? MainMessage { get; private set; } = new MainMessage();
    }
    public sealed class MainMessage
    {
        [JsonProperty("rule_button_link", NullValueHandling = NullValueHandling.Include)]
        public string? RuleButtonLink { get; private set; } = "https://youtu.be/Sagg08DrO5U?t=0";

        [JsonProperty("embed_json", NullValueHandling = NullValueHandling.Include)]
        public Embed EmbedJson { get; private set; } = new Embed()
        {
            Color = "#2B2D31",
            Title = new EmbedTitle()
            {
                Text = "Você ainda não configurou uma mensage para iniciar a AllowList",
            },
            Description = "> Va até as configurações do bot\n" +
            "> Procure o campo \"main_allowlist\" em \"messages\"\n" +
            "> E basta substituir este  JSON !",
            Fields = new[]
            {
                new EmbedField()
                {
                Title = "Use o comando /Create Embed no bot",
                Content = "> Depois que terminar de criar seu embed\n" +
                          "> Clique no botão \"Exportar JSON\".\n" +
                          "> Copie o JSON da mensagem e Cole aqui!",
                Inline = false
                }
            }
        };

    }
    public sealed class AllowListRoles
    {
        [JsonProperty("reader_role_id", NullValueHandling = NullValueHandling.Include)]
        public ulong? ReaderRoleId { get; private set; } = null;

        [JsonProperty("aproved_role_id", NullValueHandling = NullValueHandling.Include)]
        public ulong? AprovedRoleId { get; private set; } = null;

        [JsonProperty("reproved_role_id", NullValueHandling = NullValueHandling.Include)]
        public ulong? ReprovedRoleId { get; private set; } = null;
    }
    public sealed class AllowListChannel
    {
        [JsonProperty("allowlist_category_channel_id", NullValueHandling = NullValueHandling.Include)]
        public ulong? CategoryChannelId { get; private set; } = null;

        [JsonProperty("main_channel_id", NullValueHandling = NullValueHandling.Include)]
        public ulong? MainChannelId { get; private set; } = null;

        [JsonProperty("aproved_channel_id", NullValueHandling = NullValueHandling.Include)]
        public ulong? AprovedChannelId { get; private set; } = null;

        [JsonProperty("reproved_channel_id", NullValueHandling = NullValueHandling.Include)]
        public ulong? ReprovedChannelId { get; private set; } = null;
    }
    public struct AllowListQuest
    {
        [JsonProperty("question", NullValueHandling = NullValueHandling.Include)]
        public string? QuestName { get; private set; }

        [JsonProperty("quest_alternatives", NullValueHandling = NullValueHandling.Include)]
        public string[]? QuestAlternative { get; private set; }

        [JsonProperty("correct_alternative", NullValueHandling = NullValueHandling.Include)]
        public uint? QuestCorrectResponse { get; private set; }

        public AllowListQuest(string? questName, string[]? questAlternative, uint? questCorrectResponse)
        {
            QuestName = questName;
            QuestAlternative = questAlternative;
            QuestCorrectResponse = questCorrectResponse;
        }
    }
}