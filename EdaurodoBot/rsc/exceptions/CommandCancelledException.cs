using DSharpPlus.Entities;
using EdaurodoBot.rsc.utils;

namespace EdaurodoBot.rsc.exceptions
{
    public sealed class CommandCancelledException : Exception
    {
        public CommandCancelledException() : base("A execução do comando foi cancelado devido a criterios não atendidos") { }
        public CommandCancelledException(string message) : base(message) { }
        public CommandCancelledException(Exception innerException) : base("A execução do comando foi cancelado devido a criterios não atendidos", innerException) { }
        public CommandCancelledException(string messsage, Exception innerException) : base(messsage, innerException) { }
        public CommandCancelledException(string message, DiscordInteraction interaction) : base(message)
        {
            interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().AddEmbed(EdaurodoUtilities.DiscordEmbedParse(new EdaurodoEmbed(description: $"> **{message}**")))).GetAwaiter().GetResult();
        }
    }
}
