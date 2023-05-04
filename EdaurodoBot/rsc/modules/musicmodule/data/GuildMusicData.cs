namespace EdaurodoBot.rsc.modules.musicmodule.data
{
    public sealed class GuildMusicData
    {
        private List<MusicItem> _queue; 
        public GuildMusicData() { 
        
            _queue = new List<MusicItem>();
        }


        private void ClearQueue()
        {
            _queue.Clear();
        }
    }
}
