using DSharpPlus.Entities;
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
            var mvc = ctx.Member.VoiceState.Channel;
            var cvc = ctx.Guild.CurrentMember.VoiceState.Channel;

            if (mvc is null) { throw new CommandCancelledException("Você deve estar conectado a um canal de voz para executar este comando", ctx.Interaction); }
            if (cvc != null && cvc != mvc) { throw new CommandCancelledException($"Já estou tocando musica em outra sala junte-se a mim aqui <#{cvc.Id}>!", ctx.Interaction); }


            _musicData = await _music.GetOrCreateMusicDataAsync(ctx.Guild);


            return true;
        }

        [SlashCommand("teste", "comando para testes")]
        public async Task Test(InteractionContext ctx, [Option("teste", "Use para testes")] string args)
        {
            try
            {
                await ctx.DeferAsync(false);
                var value = await _youtube.SearchSong(args);
                var lavaresult = await _music.GetTrackAsync(value.TrackUrl);

                lavaresult.Tracks.Count();

                var track = lavaresult.Tracks.First();

                var embeds = new List<DiscordEmbed>() {
                    EdaurodoUtilities.DiscordEmbedParse(new EdaurodoEmbed(description:$"```\n{value}```"))
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

                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("Pressione qualquer botão para sair...");
                Console.ResetColor();
                Console.ReadKey();
            }
        }
    }
}