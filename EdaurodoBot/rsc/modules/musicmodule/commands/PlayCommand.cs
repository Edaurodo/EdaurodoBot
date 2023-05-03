using DSharpPlus.SlashCommands;

namespace EdaurodoBot.rsc.modules.musicmodule.commands
{
    public sealed class PlayCommand
    {
        private InteractionContext _context;
        private string _search;
        private PlayCommand(InteractionContext context, string search)
        {
            _context = context;
            _search = search;
        }

        public async static Task ExecuteAsync(InteractionContext context, string search)
        {
            var cmd = new PlayCommand(context, search);
        }
    }
}
