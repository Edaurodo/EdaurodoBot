using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using DSharpPlus.Lavalink.EventArgs;
using EdaurodoBot.rsc.modules.musicmodule.enums;
using EdaurodoBot.rsc.modules.musicmodule.services;
using EdaurodoBot.rsc.utils;
using System.Collections.ObjectModel;

namespace EdaurodoBot.rsc.modules.musicmodule.data
{
    public sealed class GuildMusicData
    {

        public LavalinkGuildConnection? Player { get; private set; }
        public bool IsPlaying { get; private set; }
        public MusicItem? NowPlaying { get; private set; }
        public IReadOnlyCollection<MusicItem> Queue { get; private set; }

        private LavalinkService _lavalink;
        private List<MusicItem> _queueInternal;

        private int _volume;
        private RepeatMode _repeatMode;
        private DiscordMessage? _display;
        private DiscordChannel? _displayChannel;

        public GuildMusicData(LavalinkService lavalink)
        {
            _lavalink = lavalink;
            _queueInternal = new List<MusicItem>();
            _volume = 100;
            _repeatMode = RepeatMode.None;
            _display = null;
            _displayChannel = null;

            Player = null;
            IsPlaying = false;
            NowPlaying = null;
            Queue = new ReadOnlyCollection<MusicItem>(_queueInternal);
        }

        private async Task PauseAsync()
        {
            if (Player is null || !Player.IsConnected) { return; }
            IsPlaying = false;
            await Player.PauseAsync();
        }
        private async Task ResumeAsync() {
            if (Player is null || !Player.IsConnected) { return; }
            IsPlaying = true;
            await Player.ResumeAsync();
        }
        private async Task SkipAsync() { }
        private async Task StopAsync() {
            if (Player is null || !Player.IsConnected) { return; }
            NowPlaying = null;
            await Player.StopAsync();
        }
        private async Task SetVolumeAsync(int volume) {
            if (Player is null || !Player.IsConnected) { return; }
            _volume = volume;
            await Player.SetVolumeAsync(_volume);
        }
        private void SetRepeatMode() { }

        public async Task Enqueue(MusicItem music)
        {
            lock (_queueInternal) { _queueInternal.Add(music); }
            if (Player is not null && Player.IsConnected) { await UpdateOrCreateDisplay(); }
        }
        public async Task CreatePlayerAsync(DiscordChannel playerChannel, DiscordChannel displayChannel)
        {
            if (Player is not null && Player.IsConnected) { return; }
            _displayChannel = displayChannel;
            Player = await _lavalink.Node.ConnectAsync(playerChannel);
            if (_volume != 100) { await Player.SetVolumeAsync(100); }
            Player.PlaybackFinished += Player_PlaybackFinished;
        }
        public async Task PlayAsync()
        {
            if (Player is null || !Player.IsConnected) { return; }
            if (NowPlaying is null) { await PlaybackHandlerAsync(); }
        }
        private async Task PlaybackHandlerAsync()
        {
            var item = Dequeue();
            if (item is null)
            {
                await DestroyPlayerAsync();
                return;
            }
            NowPlaying = item;
            _display?.DeleteAsync().GetAwaiter();
            _display = await UpdateOrCreateDisplay();
            await Player.PlayAsync(item.Track);
            IsPlaying = true;
        }
        private MusicItem? Dequeue()
        {
            if (_queueInternal.Count == 0) { return null; }

            switch (_repeatMode)
            {
                case RepeatMode.Single:
                    return _queueInternal[0];
                case RepeatMode.All:
                    lock (_queueInternal)
                    {
                        var item = _queueInternal[0];
                        _queueInternal.RemoveAt(0);
                        _queueInternal.Add(item);
                        return item;
                    }
                default:
                    lock (_queueInternal)
                    {
                        var item = _queueInternal[0];
                        _queueInternal.RemoveAt(0);
                        return item;
                    }
            }
        }
        private async Task Player_PlaybackFinished(LavalinkGuildConnection node, TrackFinishEventArgs args)
        {
            await Task.Delay(500);
            IsPlaying = false;
            await PlaybackHandlerAsync();
        }
        private async Task DestroyPlayerAsync()
        {
            NowPlaying = null;
            if (_display is not null)
            {
                await _display.DeleteAsync();
                _display = null;
            }
            if (Player is null) { return; }
            await Player.DisconnectAsync();
            Player = null;
        }
        private async Task<DiscordMessage> UpdateOrCreateDisplay()
        {
            if (_display is not null) { await _display.DeleteAsync(); }
            return await _displayChannel.SendMessageAsync(GetDisplay());
        }
        private DiscordMessageBuilder GetDisplay()
        {
            TimeSpan playlistime = NowPlaying.Track.Length;
            string repeatmode = _repeatMode == RepeatMode.None ? "desativado" :
                _repeatMode == RepeatMode.Single ? "🔂 single" :
                _repeatMode == RepeatMode.All ? "🔁 playlist" : "undefined";

            if (Queue.Count > 0) { Queue.ToList().ForEach(x => { playlistime = playlistime.Add(x.Track.Length); }); }
            return new DiscordMessageBuilder().WithEmbed(EdaurodoUtilities.DiscordEmbedParse(
                new(author: new("🎶 Tocando agora!"),
                    fields: new List<EdaurodoEmbedField> {
                        new ("Música:", $"✨ [`{NowPlaying?.Name}`]({NowPlaying?.Track?.Uri?.OriginalString})", true),
                        new ("Tamanho:", $"`{NowPlaying?.Track?.Length:hh\\:mm\\:ss}`", true),
                        new ("Pedido por:", $"<@{NowPlaying ?.Requester ?.Id}>", true),
                        new ("‎", $"‎", true),
                        new ("Itens na fila:", $"`{Queue.Count}`", true),
                        new ("Playlist time:", $"`{playlistime:hh\\:mm\\:ss}`", true)
                    })))
                .AddComponents(new List<DiscordActionRowComponent>()
                {
                    new (new List<DiscordComponent>()
                    {
                        new DiscordButtonComponent(
                            style: ButtonStyle.Secondary,
                            customId: "|",
                            label: $"Repeat: {repeatmode}",
                            disabled: true,
                            emoji: null),
                        new DiscordButtonComponent(
                            style: ButtonStyle.Secondary,
                            customId: "||",
                            label: $"Volume: {_volume}%",
                            disabled: true,
                            emoji: null)
                    })
                });
        }
    }
}