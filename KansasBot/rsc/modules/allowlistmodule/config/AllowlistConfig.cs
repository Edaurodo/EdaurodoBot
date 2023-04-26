using EdaurodoBot.rsc.modules.genericmodule.commands.create.embed;
using Newtonsoft.Json;

namespace EdaurodoBot.rsc.modules.allowlistmodule.config
{
    public sealed class AllowlistConfig
    {
        [JsonProperty("use_module")]
        public bool Use { get; private set; }

        [JsonProperty("reproved_wait_time_from_minutes")]
        public int? ReprovedWaitTime { get; private set; }

        [JsonProperty("roles")]
        public AllowlistRoles Roles { get; private set; }


        [JsonProperty("channels")]
        public AllowlistChannels Channels { get; private set; }


        [JsonProperty("questions")]
        public AllowlistQuestion[]? Questions { get; private set; }


        [JsonProperty("messages")]
        public AllowlistMessages Messages { get; private set; }

        public AllowlistConfig(bool? use, int? reprovedwaittime, AllowlistRoles roles, AllowlistChannels channels, AllowlistQuestion[]? questions, AllowlistMessages messages)
        {
            Use = use ?? false;
            ReprovedWaitTime = reprovedwaittime ?? 60;
            Roles = roles;
            Channels = channels;
            Questions = questions;
            Messages = messages;
        }
    }
    public sealed class AllowlistRoles
    {
        [JsonProperty("reader_id")]
        public ulong? ReaderId { get; private set; }

        [JsonProperty("approved_id")]
        public ulong? ApprovedId { get; private set; }

        [JsonProperty("reproved_id")]
        public ulong? ReprovedId { get; private set; }

        [JsonProperty("allowlist_sent_id")]
        public ulong? AllowlistSentId { get; private set; }

        [JsonProperty("waiting_interview_id")]
        public ulong? WaitingInterviewId { get; private set; }

        public AllowlistRoles() { ReaderId = null; ApprovedId = null; ReprovedId = null; AllowlistSentId = null; WaitingInterviewId = null; }
    }
    public sealed class AllowlistChannels
    {
        [JsonProperty("allowlist_category_id")]
        public ulong? CategoryId { get; private set; }

        [JsonProperty("main_id")]
        public ulong? MainId { get; private set; }

        [JsonProperty("approved_id")]
        public ulong? ApprovedId { get; private set; }

        [JsonProperty("reproved_id")]
        public ulong? ReprovedId { get; private set; }

        [JsonProperty("interview_notice_id")]
        public ulong? InterviewId { get; private set; }

        public AllowlistChannels() { CategoryId = null; MainId = null; ApprovedId = null; ReprovedId = null; InterviewId = null; }
    }
    public struct AllowlistQuestion
    {
        [JsonProperty("question")]
        public string? Question { get; private set; }

        [JsonProperty("alternatives")]
        public string[]? Alternatives { get; private set; }

        [JsonProperty("correct_response")]
        public int? CorrectAnswer { get; private set; }

        public AllowlistQuestion(string? question, string[]? alternatives, int? correctanswer)
        {
            Question = question;
            Alternatives = alternatives;
            CorrectAnswer = correctanswer;
        }
    }
    public sealed class AllowlistMessages
    {
        [JsonProperty("main_fixed_message")]
        public MainMessage? MainMessage { get; private set; }

        public AllowlistMessages(MainMessage mainmessage) { MainMessage = mainmessage; }
    }
    public sealed class MainMessage
    {
        [JsonProperty("rule_button_link")]
        public string? ButtonLink { get; private set; }

        [JsonProperty("embed")]
        public Embed Embed { get; private set; }

        public MainMessage(string? buttonlink, Embed? embed)
        {
            ButtonLink = buttonlink;
            Embed = embed ?? new Embed()
            {
                Color = "#2B2D31",
                Title = new EmbedTitle() { Text = "Você ainda não configurou uma mensage para iniciar a AllowList" },
                Description =
                            "### `Esta é a mensagem padrão da aplicação`\n\n" +
                            "> Navege pelos arquivos de configuração da aplicação\n" +
                            "> Encontre o arquivo 'allowlist.cfg.json' e procure o campo\n" +
                            "> 'messages' -> 'main_fixed_message' para altertar esta mensagem\n\n" +
                            "## `Use o comando \"/Create Embed\" para criar e obter o JSON`"
            };
        }
    }
}