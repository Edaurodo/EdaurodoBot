using DSharpPlus;
using DSharpPlus.SlashCommands;
using KansasBot.rsc.modules.whitelistmodule.commands.commandclass;
using KansasBot.rsc.modules.whitelistmodule.services;

namespace KansasBot.rsc.modules.whitelistmodule.commands
{
    [SlashCommandGroup("Allowlist", "Comandos da AllowList"), SlashCommandPermissions(Permissions.Administrator)]
    public sealed class AllowlistCommands : ApplicationCommandModule
    {
        [SlashCommandGroup("Update", "Update itens da allowlist")]
        public sealed class AllowlistUpdateCommands : ApplicationCommandModule
        {
            private AllowlistService Module;
            public AllowlistUpdateCommands(AllowlistService module) => Module = module;

            [SlashCommand("AllowlistMessage", $"Atualiza a mensagem do canal principal da Allowlist")]
            public async Task UpdateMainMessage(InteractionContext ctx)
            {
                var command = new UpdateWlMessage(ctx, Module);
                await command.ExecuteAsync();
            }
        }
    }
}
