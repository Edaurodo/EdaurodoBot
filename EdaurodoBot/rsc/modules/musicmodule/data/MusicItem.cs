using DSharpPlus.Lavalink;

namespace EdaurodoBot.rsc.modules.musicmodule.data
{
    public sealed class MusicItem
    {
        public LavalinkTrack Track { get; }
        public string Author { get; }
        public string Name { get; }
        public string Thumbnail { get; }

        public MusicItem(LavalinkTrack track, YoutubeSearchResult result)
        {
            Track = track;
            Author = result.Author;
            Name = result.Name;
            Thumbnail = result.Thumbnail;
        }

        public MusicItem(MusicItem musicitem)
        {
            Track = musicitem.Track;
            Author = musicitem.Author;
            Name = musicitem.Name;
            Thumbnail = musicitem.Thumbnail;
        }


        public static MusicItem Parse(YoutubeSearchResult item)
        {
            return null;
        }
    }
}
