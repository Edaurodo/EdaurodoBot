using DSharpPlus.SlashCommands;

namespace EdaurodoBot.rsc.modules.musicmodule.commands
{
    [SlashCommandGroup("Music", "Utilise para todo que é referente ao um bom som")]
    public sealed class MusicCommandGroup : ApplicationCommandModule
    {
        [SlashCommand("Play", "Qual musica senhor(a)?")]
        public async Task Play(InteractionContext context, [Option("song", "Nome ou link da música")] string search)
        {
            await PlayCommand.ExecuteAsync(context, search);
        }
    }
}
