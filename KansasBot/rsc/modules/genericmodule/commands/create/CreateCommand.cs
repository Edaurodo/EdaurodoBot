using DSharpPlus;
using DSharpPlus.SlashCommands;
using EdaurodoBot.rsc.modules.genericmodule.commands.create.embed;

namespace EdaurodoBot.rsc.modules.genericmodule.commands.create
{

    [SlashCommandGroup("create", "Use de acordo com sua criatividade"), SlashCommandPermissions(Permissions.Administrator)]
    public sealed class CreateCommand : ApplicationCommandModule
    {
        [SlashCommand("embed", "Usado para criar embeds")]
        public async Task CreateEmbed(InteractionContext ctx)
        {
            EmbedCommand command = new EmbedCommand(ctx);
            await command.ExecuteAsync();
        }
    }
}
