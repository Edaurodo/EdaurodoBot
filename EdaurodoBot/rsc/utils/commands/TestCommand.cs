using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using DSharpPlus.SlashCommands;
using EdaurodoBot.rsc.exceptions;
using EdaurodoBot.rsc.modules.musicmodule.data;
using EdaurodoBot.rsc.modules.musicmodule.services;

namespace EdaurodoBot.rsc.utils.commands
{
    [SlashModuleLifespan(SlashModuleLifespan.Transient)]
    public sealed class TestCommand : ApplicationCommandModule
    {
        public override async Task<bool> BeforeSlashExecutionAsync(InteractionContext ctx)
        {
            return true;
        }

        [SlashCommand("teste", "comando para testes")]
        public async Task Test(InteractionContext ctx)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("Mesage: ");
                Console.ForegroundColor = ConsoleColor.Red;

                Console.WriteLine(ex.Message);

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("Source: ");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Source);

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("Target Site: ");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.TargetSite);

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("Stack Trace: ");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.StackTrace + "\n");
                Console.ResetColor();
            }
        }
    }
}