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
        private YoutubeSearchProvider _youtube;
        private MusicService _music;
        private GuildMusicData? _musicData;
        public TestCommand(YoutubeSearchProvider youtube, MusicService music) { _youtube = youtube; _music = music; }

        public override async Task<bool> BeforeSlashExecutionAsync(InteractionContext ctx)
        {
            var mvs = ctx.Member.VoiceState;
            var cvs = ctx.Guild.CurrentMember.VoiceState;

            if (mvs is null) { throw new CommandCancelledException("Você deve estar conectado a um canal de voz para executar este comando", ctx.Interaction); }
            if (cvs != null && cvs.Channel != mvs.Channel) { throw new CommandCancelledException($"Já estou tocando musica em outra sala junte-se a mim aqui <#{cvs.Channel.Id}>!", ctx.Interaction); }

            _musicData = await _music.GetOrCreateMusicDataAsync(ctx.Guild);

            return true;
        }

        [SlashCommand("teste", "comando para testes")]
        public async Task Test(InteractionContext ctx, [Option("teste", "Use para testes")] string args)
        {
            try
            {

                var ytresult = await _youtube.SearchSongAsync(args);
                if (!ytresult.Any()) { throw new CommandCancelledException("Não foi possível encontrar nenhuma faixa!", ctx.Interaction); }
                await ctx.DeferAsync(true);

                LavalinkLoadResult lavaresult = await _music.GetTrackAsync(ytresult.First().TrackUrl);

                _musicData.Enqueue(new MusicItem(lavaresult.Tracks.First(), ytresult.First()));
                await _musicData.CreatePlayerAsync(ctx.Member.VoiceState.Channel);
                await _musicData.PlayAsync();

















                var embeds = new List<DiscordEmbed>() {

                    EdaurodoUtilities.DiscordEmbedParse(new EdaurodoEmbed(description:
                    $"`lavaresult.LoadResultType:`\n" +
                    $"```diff\n- RESULT:\n\n" +
                    $"+ {lavaresult.LoadResultType}\n" +
                    $"--- +++++++++++++++++++++++++++++++++++++++++++++++++++++ ---\n" +
                    $"- POSSIBLES RESULTS:\n\n" +
                    $"+ {LavalinkLoadResultType.NoMatches}\n\n" +
                    $"+ {LavalinkLoadResultType.SearchResult}\n\n" +
                    $"+ {LavalinkLoadResultType.LoadFailed}\n\n" +
                    $"+ {LavalinkLoadResultType.TrackLoaded}\n\n" +
                    $"+ {LavalinkLoadResultType.PlaylistLoaded}```")),

                    EdaurodoUtilities.DiscordEmbedParse(new EdaurodoEmbed(description:
                    $"`lavaresult.PlaylistInfo:`\n" +
                    $"```diff\n- ToString RESULT:\n\n" +
                    $"+ PlaylistInfo.Name: {lavaresult.PlaylistInfo.Name}\n\n" +
                    $"+ PlaylistInfo.SelectedTrack: {lavaresult.PlaylistInfo.SelectedTrack}```")),

                    EdaurodoUtilities.DiscordEmbedParse(new EdaurodoEmbed(description:
                    $"`lavaresult.Tracks`\n" +
                    $"```diff\n- IENUMERABLE RESULT\n\n" +
                    $"+ Tracks.Count(): {lavaresult.Tracks.Count()}\n" +
                    $"--- +++++++++++++++++++++++++++++++++++++++++++++++++++++ ---\n" +
                    $"- FIRST RESULTS:\n\n" +
                    $"+ Lavalinktrack.Author: {lavaresult.Tracks.First().Author}\n\n" +
                    $"+ Lavalinktrack.Title: {lavaresult.Tracks.First().Title}\n\n" +
                    $"+ Lavalinktrack.Identifier: {lavaresult.Tracks.First().Identifier}\n\n" +
                    $"+ Lavalinktrack.TrackString: {lavaresult.Tracks.First().TrackString}\n\n" +
                    $"+ Lavalinktrack.Position: {lavaresult.Tracks.First().Position}\n\n" +
                    $"+ Lavalinktrack.IsStream: {lavaresult.Tracks.First().IsStream}\n\n" +
                    $"+ Lavalinktrack.IsSeekable: {lavaresult.Tracks.First().IsSeekable}\n\n" +
                    $"+ LavalinkTack.Uri: {lavaresult.Tracks.First().Uri}```"))
                    };
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbeds(embeds));

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