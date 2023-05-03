using Newtonsoft.Json;

namespace EdaurodoBot.rsc.modules.musicmodule.data
{
    public struct YoutubeSearchResult
    {
        public string Author { get; }
        public string Title { get;}
        public string Id { get;}

        public YoutubeSearchResult(string author, string title, string Id)
        {
            this.Author = author;
            this.Title = title;
            this.Id = Id;
        }
    }
    
    public struct YoutubeApiResponse
    {
        [JsonProperty("id")]
        public ResponseId Id { get; }

        [JsonProperty("snippet")]
        public ResponseSnippet Snippet { get; }


        public struct ResponseId
        {
            [JsonProperty("videoId")]
            public string VideoId { get; }
        }
        public struct ResponseSnippet
        {
            [JsonProperty("channelTitle")]
            public string Author { get; }
            [JsonProperty("title")]
            public string Title { get; }
        }
    }
}
