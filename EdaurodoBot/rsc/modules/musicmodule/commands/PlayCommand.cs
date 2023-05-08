using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using DSharpPlus.SlashCommands;
using EdaurodoBot.rsc.exceptions;
using EdaurodoBot.rsc.modules.musicmodule.data;
using EdaurodoBot.rsc.modules.musicmodule.enums;
using EdaurodoBot.rsc.modules.musicmodule.services;
using EdaurodoBot.rsc.utils;
using System.Security.AccessControl;

namespace EdaurodoBot.rsc.modules.musicmodule.commands
{
    public sealed class PlayCommand : ApplicationCommandModule
    {
        private YoutubeSearchProvider _youtube;
        private MusicService _music;
        private GuildMusicData? _musicData;

        public PlayCommand(YoutubeSearchProvider youtube, MusicService music) { _youtube = youtube; _music = music; }
        public override async Task<bool> BeforeSlashExecutionAsync(InteractionContext ctx)
        {
            await ctx.DeferAsync(true);
            var mvs = ctx.Member.VoiceState;
            var cvs = ctx.Guild.CurrentMember.VoiceState;

            if (mvs is null) { throw new CommandCancelledException("Você deve estar conectado a um canal de voz para executar este comando", ctx.Interaction); }
            if (cvs != null && cvs.Channel != mvs.Channel) { throw new CommandCancelledException($"Já estou tocando musica em outra sala junte-se a mim aqui <#{cvs.Channel.Id}>!", ctx.Interaction); }

            _musicData = await _music.GetOrCreateMusicDataAsync(ctx.Guild);

            return true;
        }

        [SlashCommand("Play", "Qual musica senhor(a)?")]
        public async Task Play(InteractionContext ctx, [Option("ParameterType", "Tipo de argumento que será usado")] OptionsParameter paramtype, [Option("Música", "argumento de acordo com o paramentro escolhido")] string args)
        {
            try
            {
                LavalinkLoadResult lavaresult;
                switch (paramtype)
                {
                    case OptionsParameter.Link:
                        if (Uri.TryCreate(args, UriKind.Absolute, out Uri url)) { lavaresult = await _music.GetTrackAsync(url); }
                        else { throw new CommandCancelledException("URL Invalída", ctx.Interaction); }
                        break;
                    case OptionsParameter.Args:
                        var ytresult = await _youtube.SearchSongAsync(args);
                        if (!ytresult.Any()) { throw new CommandCancelledException("Não foi possível encontrar nenhuma faixa!", ctx.Interaction); }
                        lavaresult = await _music.GetTrackAsync(ytresult.First().TrackUrl);
                        break;
                    default:
                        return;
                }
                await _musicData.EnqueueMusicItem(new MusicItem(lavaresult.Tracks.First(), ctx.Member), ctx);
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
