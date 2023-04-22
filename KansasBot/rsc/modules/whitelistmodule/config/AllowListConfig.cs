using KansasBot.rsc.modules.genericmodule.commands.create.embed;
using Newtonsoft.Json;

namespace KansasBot.rsc.modules.whitelistmodule.config
{
    public sealed class AllowListConfig
    {
        [JsonProperty("use_allowlist_module")]
        public bool Use { get; private set; } = true;

        [JsonProperty("channels", NullValueHandling = NullValueHandling.Include)]
        public AllowListChannel? Channels { get; private set; } = new AllowListChannel();

        [JsonProperty("roles", NullValueHandling = NullValueHandling.Include)]
        public AllowListRoles? Roles{ get; private set; } = new AllowListRoles();

        [JsonProperty("messages", NullValueHandling = NullValueHandling.Include)]
        public AllowlistMessages? Messages { get; private set; } = new AllowlistMessages();

        [JsonProperty("quests", NullValueHandling = NullValueHandling.Include)]
        public AllowListQuest[]? Quests{ get; private set; } = new[] {
            new AllowListQuest("Questão 1", new[] { "Alternativa 1", "Alternativa 2", "Quantas voce quiser desde que não ultrapasse 25" }, 2),
            new AllowListQuest("Questão 2", new[] { "Alternativa 1", "Alternativa 2", "Quantas voce quiser desde que não ultrapasse 25" }, 3)
        };
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

        [JsonProperty("embed", NullValueHandling = NullValueHandling.Include)]
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
        [JsonProperty("reader_id", NullValueHandling = NullValueHandling.Include)]
        public ulong? ReaderRoleId { get; private set; } = null;

        [JsonProperty("approved_id", NullValueHandling = NullValueHandling.Include)]
        public ulong? ApprovedRoleId { get; private set; } = null;

        [JsonProperty("reproved_id", NullValueHandling = NullValueHandling.Include)]
        public ulong? ReprovedRoleId { get; private set; } = null;
    }
    public sealed class AllowListChannel
    {
        [JsonProperty("allowlist_category_id", NullValueHandling = NullValueHandling.Include)]
        public ulong? CategoryChannelId { get; private set; } = null;

        [JsonProperty("main_id", NullValueHandling = NullValueHandling.Include)]
        public ulong? MainChannelId { get; private set; } = null;

        [JsonProperty("approved_id", NullValueHandling = NullValueHandling.Include)]
        public ulong? ApprovedChannelId { get; private set; } = null;

        [JsonProperty("reproved_id", NullValueHandling = NullValueHandling.Include)]
        public ulong? ReprovedChannelId { get; private set; } = null;
    }
    public struct AllowListQuest
    {
        [JsonProperty("question", NullValueHandling = NullValueHandling.Include)]
        public string? QuestName { get; private set; }

        [JsonProperty("alternatives", NullValueHandling = NullValueHandling.Include)]
        public string[]? Alternatives { get; private set; }

        [JsonProperty("correct_response", NullValueHandling = NullValueHandling.Include)]
        public int? QuestCorrectResponse { get; private set; }

        public AllowListQuest(string? questName, string[]? questAlternative, int? questCorrectResponse)
        {
            QuestName = questName;
            Alternatives = questAlternative;
            QuestCorrectResponse = questCorrectResponse;
        }
    }
}