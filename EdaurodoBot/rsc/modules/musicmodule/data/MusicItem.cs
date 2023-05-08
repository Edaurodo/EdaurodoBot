using DSharpPlus.Entities;
using DSharpPlus.Lavalink;

namespace EdaurodoBot.rsc.modules.musicmodule.data
{
    public sealed class MusicItem
    {
        public string Author { get; }
        public string Name { get; }
        public string Thumbnail { get; }
        public LavalinkTrack Track { get; }
        public DiscordMember Requester { get; }

        public MusicItem(LavalinkTrack track, DiscordMember requester)
        {
            Author = track.Author;
            Name = track.Title;
            Thumbnail = null;
            Track = track;
            Requester = requester;
        }
    }
}
