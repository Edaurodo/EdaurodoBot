using DSharpPlus;
using DSharpPlus.Entities;
using Microsoft.VisualBasic;

namespace EdaurodoBot.rsc.exceptions
{
    public sealed class CommandCancelledException : Exception
    {
        public CommandCancelledException() : base("A execução do comando foi cancelado devido a criterios não atendidos") { }
        public CommandCancelledException(string message) : base(message) { }
        public CommandCancelledException(Exception innerException) : base("A execução do comando foi cancelado devido a criterios não atendidos", innerException) { }
        public CommandCancelledException(string messsage, Exception innerException) : base(messsage, innerException) { }
        public CommandCancelledException(string message, DiscordInteraction interaction)
        {
            interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent(message).AsEphemeral(true)).GetAwaiter().GetResult();
        }
    }
}
