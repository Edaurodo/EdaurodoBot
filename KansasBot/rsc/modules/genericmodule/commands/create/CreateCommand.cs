using DSharpPlus;
using DSharpPlus.SlashCommands;
using KansasBot.rsc.modules.genericmodule.commands.create.embed;
using KansasBot.rsc.modules.whitelistmodule;

namespace KansasBot.rsc.modules.genericmodule.commands.create
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
