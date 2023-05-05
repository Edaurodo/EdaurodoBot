using EdaurodoBot.rsc.modules.musicmodule.data;
using EdaurodoBot.rsc.utils;
using Newtonsoft.Json.Linq;
using System.Web;

namespace EdaurodoBot.rsc.modules.musicmodule.services
{
    public sealed class YoutubeSearchProvider
    {
        private string _apiKey;
        private HttpClient _http;

        public YoutubeSearchProvider(string apikey)
        {
            _apiKey = apikey;
            _http = new HttpClient() { BaseAddress = new Uri("https://www.googleapis.com/youtube/v3/search") };
        }

        public async Task<IEnumerable<YoutubeSearchResult>> SearchSongAsync(string terms)
        {
            Uri url = new Uri($"https://www.googleapis.com/youtube/v3/search?maxResults=1&type=video&videoCategoryId=10&part=snippet&fields=items(id.videoId,snippet.channelTitle,snippet.title,snippet.thumbnails.high.url)&key={_apiKey}&q={HttpUtility.UrlEncode(terms)}");

            IEnumerable<YoutubeApiResponse>? values;
            using (var sr = new StreamReader(_http.GetAsync(url).GetAwaiter().GetResult().Content.ReadAsStream(), EdaurodoUtilities.UTF8))
            {
                string json = await sr.ReadToEndAsync();
                sr.Close();
                Console.WriteLine(json);
                var data = JObject.Parse(json);
                values = data["items"].ToObject<IEnumerable<YoutubeApiResponse>>();
            }
            return values.Select(x => new YoutubeSearchResult(x));
        }
    }
}