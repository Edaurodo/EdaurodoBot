using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using EdaurodoBot.rsc.modules.musicmodule.data;
using System.Collections.Concurrent;

namespace EdaurodoBot.rsc.modules.musicmodule.services
{
    public sealed class MusicService
    {
        private LavalinkService _lavalink;
        private ConcurrentDictionary<ulong, GuildMusicData> _data;

        public MusicService(LavalinkService lavalink)
        {
            _lavalink = lavalink;
            _data = new ConcurrentDictionary<ulong, GuildMusicData>();
        }

        public async Task<GuildMusicData> GetOrCreateMusicDataAsync(DiscordGuild guild)
        {
            if(_data.TryGetValue(guild.Id, out var data)){ return data; }

            data = _data.AddOrUpdate(guild.Id, new GuildMusicData(), (k, v) => v);
            return data;
        }

        public async Task<LavalinkLoadResult> GetTrackAsync(Uri url) => await _lavalink.Node.Rest.GetTracksAsync(url);

    }
}