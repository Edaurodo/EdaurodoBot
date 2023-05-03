using EdaurodoBot.rsc.utils;
using Newtonsoft.Json;

namespace EdaurodoBot.rsc.modules.allowlistmodule.config.messages
{
    public sealed class MainMessage
    {
        [JsonProperty("rule_button_link")]
        public string ButtonLink { get; private set; }

        [JsonProperty("content")]
        public string? Content { get; private set; }

        [JsonProperty("embed")]
        public EdaurodoEmbed Embed { get; private set; }

        public MainMessage(string? buttonlink, string? content, EdaurodoEmbed? embed)
        {
            ButtonLink = buttonlink ?? "https://youtu.be/Sagg08DrO5U?t=0";
            Content = content;
            Embed = embed ?? new EdaurodoEmbed(
                title: new EdaurodoEmbedTitle("Você ainda não configurou uma mensage para iniciar a AllowList", null),
                description:
                "### `Esta é a mensagem padrão da aplicação`\n\n" +
                "> Navege pelos arquivos de configuração da aplicação\n" +
                "> Encontre o arquivo 'allowlist.cfg.json' e procure o campo\n" +
                "> 'messages' -> 'main_fixed_message' para altertar esta mensagem\n\n" +
                "## `Use o comando \"/Create Embed\" para criar e obter o JSON`");
        }
    }
}
