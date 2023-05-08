using Newtonsoft.Json;

namespace EdaurodoBot.rsc.modules.musicmodule.data
{
    public class YoutubeSearchResult
    {
        public string Author { get; }
        public string Name { get; }
        public string Thumbnail { get; }
        public Uri TrackUrl { get; }

        public YoutubeSearchResult(YoutubeApiResponse response)
        {
            this.Author = response.Snippet.ChannelTitle;
            this.Name = response.Snippet.Title;
            this.TrackUrl = new Uri($"https://youtu.be/{response.Id.VideoId}");
        }
    }
    public struct YoutubeApiResponse
    {
        [JsonProperty("id")]
        public IdResponse Id { get; private set; }
        [JsonProperty("snippet")]
        public SnippetResponse Snippet { get; private set; }
    }
    public struct IdResponse
    {
        [JsonProperty("videoId")]
        public string VideoId { get; private set; }
    }

    public struct SnippetResponse
    {
        [JsonProperty("title")]
        public string Title { get; private set; }

        [JsonProperty("channelTitle")]
        public string ChannelTitle { get; private set; }
    }

    public struct SnippetThumbnail
    {
        [JsonProperty("high")]
        public ThumbnailLarge Large { get; private set; }
    }

    public struct ThumbnailLarge
    {
        [JsonProperty("url")]
        public string Url { get; private set; }
    }
}