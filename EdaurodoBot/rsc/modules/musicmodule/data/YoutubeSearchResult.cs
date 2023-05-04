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
            this.Thumbnail = response.Snippet.Thumbnails.Large.Url;
            this.TrackUrl = new Uri($"https://youtu.be/{response.Id.VideoId}");
        }
        public override string ToString()
        {
            return $"Nome: {this.Name}\n\nAuthor: {this.Author}\n\nThumbnail_Link: {this.Thumbnail}\n\nVideo_Link: {this.TrackUrl.OriginalString}";
        }
    }
    public struct YoutubeApiResponse
    {
        [JsonProperty("id")]
        public IdResponse Id { get; private set; }
        [JsonProperty("snippet")]
        public SnippetResponse Snippet { get; private set; }

        public override string ToString()
        {
            return $"id:\n-{Id}\nsnippet:\n-{Snippet}\n";
        }
    }
    public struct IdResponse
    {
        [JsonProperty("videoId")]
        public string VideoId { get; private set; }

        public override string ToString()
        {
            return $"videoId: {VideoId}\n";
        }
    }

    public struct SnippetResponse
    {
        [JsonProperty("title")]
        public string Title { get; private set; }

        [JsonProperty("thumbnails")]
        public SnippetThumbnail Thumbnails { get; private set; }

        [JsonProperty("channelTitle")]
        public string ChannelTitle { get; private set; }
        public override string ToString()
        {
            return $"title: {Title}\nthumbnail:\n-{Thumbnails}\nchannelTitle: {ChannelTitle}\n";
        }
    }

    public struct SnippetThumbnail
    {
        [JsonProperty("high")]
        public ThumbnailLarge Large { get; private set; }

        public override string ToString()
        {
            return $"high:\n-{Large}\n";
        }
    }

    public struct ThumbnailLarge
    {
        [JsonProperty("url")]
        public string Url { get; private set; }

        public override string ToString()
        {
            return $"url: {Url}\n";
        }
    }
}