using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using DSharpPlus.Lavalink.EventArgs;
using EdaurodoBot.rsc.modules.musicmodule.services;

namespace EdaurodoBot.rsc.modules.musicmodule.data
{
    public sealed class GuildMusicData
    {
        public LavalinkGuildConnection? Player { get; private set; }
        public MusicItem? NowPlaying { get; private set; }

        LavalinkService _lavalink;
        private List<MusicItem> _queue;


        public GuildMusicData(LavalinkService lavalink)
        {
            NowPlaying = default;

            _lavalink = lavalink;
            _queue = new List<MusicItem>();
        }

        private async Task Player_PlaybackFinished(LavalinkGuildConnection node, TrackFinishEventArgs args)
        {
            await Task.Delay(500);
            await PlaybackHandlerAsync();
        }

        private async Task PlaybackHandlerAsync()
        {
            var item = Dequeue();
            if (item is null)
            {
                NowPlaying = default;
                return;
            }

            NowPlaying = item;
            await Player.PlayAsync(item.Track);
        }
        public async Task PlayAsync()
        {
            if (Player == null || !Player.IsConnected) { return; }
            if(NowPlaying is null) { await PlaybackHandlerAsync(); }
        }

        public void Enqueue(MusicItem item)
        {
            lock (_queue)
            {
                _queue.Add(item);
            }
        }

        private MusicItem? Dequeue()
        {
            lock (_queue)
            {
                if (_queue.Count == 0) { return null; }
                var item = _queue[0];
                _queue.RemoveAt(0);
                return item;
            }
        }

        private void EmptyQueue()
        {
            lock (_queue)
            {
                _queue.Clear();
            }
        }

        public async Task CreatePlayerAsync(DiscordChannel channel)
        {
            if (!(Player is null) && Player.IsConnected) { return; }

            Player = await _lavalink.Node.ConnectAsync(channel);
            await Player.SetVolumeAsync(100);
            Player.PlaybackFinished += Player_PlaybackFinished;
        }

        private async Task DestroyPlayerAsync()
        {
            if (Player is null) { return; }
            await Player.DisconnectAsync();
            Player = null;
        }
    }
}
