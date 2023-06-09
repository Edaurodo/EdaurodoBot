﻿using DSharpPlus.SlashCommands;
using EdaurodoBot.rsc.modules.genericmodule.commands.info.emoji;

namespace EdaurodoBot.rsc.modules.genericmodule.commands.info
{
    [SlashCommandGroup("info", "Comando para obter informações")]
    public sealed class InfoCommand : ApplicationCommandModule
    {
        [SlashCommand("emoji", "Obtenha todas indormações disponivel sobre um emoji")]
        public async Task Emoji(InteractionContext ctx, [Option("emoji", "O emoji")] string emoji)
        {
            EmojiCommand command = new EmojiCommand(ctx, emoji);
            await command.ExecuteAsync();
        }
    }
}
