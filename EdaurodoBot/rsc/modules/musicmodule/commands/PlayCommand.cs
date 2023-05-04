using DSharpPlus.SlashCommands;
using EdaurodoBot.rsc.exceptions;

namespace EdaurodoBot.rsc.modules.musicmodule.commands
{
    public sealed class PlayCommand : ApplicationCommandModule
    {
        public override async Task<bool> BeforeSlashExecutionAsync(InteractionContext ctx)
        {
            var mvc = ctx.Member.VoiceState.Channel;
            var cvc = ctx.Guild.CurrentMember.VoiceState.Channel;

            if (mvc is null) { throw new CommandCancelledException("Você deve estar conectado a um canal de voz para executar este comando", ctx.Interaction); }
            if (cvc != null && cvc != mvc) { throw new CommandCancelledException($"Já estou tocando musica em outra sala junte-se a mim aqui <#{cvc.Id}>!", ctx.Interaction); }

            return true;
        }
        [SlashCommand("Play", "Qual musica senhor(a)?")]
        public async Task Play(InteractionContext context, [Option("song", "Nome ou link da música")] string search)
        {

        }
    }
}
