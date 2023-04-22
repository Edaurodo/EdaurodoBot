﻿using DSharpPlus;
using DSharpPlus.SlashCommands;
using KansasBot.rsc.modules.whitelistmodule.commands.update;
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

            [SlashCommand("MainMessage", $"Atualiza a mensagem do canal que se inicia a Allowlist")]
            public async Task UpdateMainMessage(InteractionContext ctx)
            {
                var command = new UpdateMainMessage(ctx, Module);
                await command.ExecuteAsync();
            }

            [SlashCommand("Readers", $"Atualiza os canais dos leitores da Allowlist")]
            public async Task UpdateReaders(InteractionContext ctx)
            {
                var command = new UpdateReadersChannels(ctx, Module);
                await command.ExecuteAsync();
            }
        }
    }
}
