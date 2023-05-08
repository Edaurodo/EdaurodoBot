using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using DSharpPlus.Lavalink.EventArgs;
using DSharpPlus.SlashCommands;
using EdaurodoBot.rsc.modules.musicmodule.enums;
using EdaurodoBot.rsc.modules.musicmodule.services;
using EdaurodoBot.rsc.utils;
using System.Collections.ObjectModel;

namespace EdaurodoBot.rsc.modules.musicmodule.data
{
    public sealed class GuildMusicData
    {
        public MusicItem? NowPlaying { get; private set; }
        public IReadOnlyCollection<MusicItem> Queue { get; private set; }

        private LavalinkService _lavalink;
        private List<MusicItem> _queueInternal;

        private LavalinkGuildConnection? _player;
        private int _volume;
        private RepeatMode _repeatMode;
        private DiscordChannel? _displayChannel;
        private DiscordMessage? _displayMessage;
        public GuildMusicData(LavalinkService lavalink)
        {
            _lavalink = lavalink;
            _queueInternal = new List<MusicItem>();
            _volume = 100;
            _repeatMode = RepeatMode.None;
            _displayMessage = null;
            _displayChannel = null;

            NowPlaying = null;
            Queue = new ReadOnlyCollection<MusicItem>(_queueInternal);
        }
        public async Task EnqueueMusicItem(MusicItem item, InteractionContext ctx)
        {
            lock (_queueInternal)
            {
                _queueInternal.Add(item);
            }
            if (!(_displayMessage is null)) { _displayMessage = await _displayMessage.ModifyAsync(GetDisplay()); }
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(
                EdaurodoUtilities.DiscordEmbedParse(new EdaurodoEmbed(
                    author: new EdaurodoEmbedAuthor("Música adicionada na fila!", ctx.Guild.CurrentMember.AvatarUrl),
                    fields: new List<EdaurodoEmbedField>() {
                        new ("Música:",$"`#{Queue.Count}` - [`{Queue.Last().Name}`]({Queue.Last().Track.Uri.OriginalString})", true),
                        new ("Duração:", $"`{Queue.Last().Track.Length:hh\\:mm\\:ss}`", true),
                        new ("Requested by:",$"<@{ctx.Member.Id}>",true)
                    }))));
            await CreatePlayerAsync(ctx.Member.VoiceState.Channel, ctx.Channel);
            await PlayAsync();
        }
        private async Task CreatePlayerAsync(DiscordChannel playerChannel, DiscordChannel displayChannel)
        {
            if (!(_player is null) && _player.IsConnected) { return; }
            _displayChannel = displayChannel;
            _player = await _lavalink.Node.ConnectAsync(playerChannel);
            if (_volume != 100) { await _player.SetVolumeAsync(100); }
            _player.PlaybackFinished += Player_PlaybackFinished;
        }
        private async Task PlayAsync()
        {
            if (_player == null || !_player.IsConnected) { return; }
            if (NowPlaying is null) { await PlaybackHandlerAsync(); }
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
        private async Task PlaybackHandlerAsync()
        {
            var item = Dequeue();
            if (item is null)
            {
                await DestroyPlayerAsync();
                return;
            }

            NowPlaying = item;
            _displayMessage = await UpdateOrCreateDisplay();
            await _player.PlayAsync(item.Track);
        }
        private async Task Player_PlaybackFinished(LavalinkGuildConnection node, TrackFinishEventArgs args)
        {
            await Task.Delay(500);
            await PlaybackHandlerAsync();
        }
        private void EmptyQueue()
        {
            lock (_queueInternal)
            {
                _queueInternal.Clear();
            }
        }
        private async Task DestroyPlayerAsync()
        {
            NowPlaying = null;
            if (!(_displayMessage is null))
            {
                await _displayMessage.DeleteAsync();
                _displayMessage = null;
            }
            if (_player is null) { return; }
            await _player.DisconnectAsync();
            _player = null;
        }
        private async Task<DiscordMessage> UpdateOrCreateDisplay()
        {
            if (!(_displayMessage is null)) { await _displayMessage.DeleteAsync(); }
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
                            label: $"Repeat Mode: {repeatmode}",
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
