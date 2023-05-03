using EdaurodoBot.rsc.utils;
using System.Net;

namespace EdaurodoBot.rsc.modules.musicmodule.services
{
    public sealed class YoutubeSearchProvider
    {
        private string _apiKey;
        private HttpClient _http;

        public YoutubeSearchProvider(string apikey)
        {
            _apiKey = apikey;
            _http = new HttpClient() { BaseAddress = new Uri("https://youtube.googleapis.com/youtube/v3/search") };
        }
    }
}
