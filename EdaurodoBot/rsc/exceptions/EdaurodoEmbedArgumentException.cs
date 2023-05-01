using DSharpPlus;
using DSharpPlus.Entities;

namespace EdaurodoBot.rsc.exceptions
{
    public sealed class EdaurodoEmbedArgumentException : Exception
    {
        public EdaurodoEmbedArgumentException(string message, DiscordInteraction interaction) : base(message) {
            interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent(message).AsEphemeral(true)).GetAwaiter().GetResult();
        }
    }
}
