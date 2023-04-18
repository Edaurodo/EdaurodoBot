using Newtonsoft.Json;

namespace KansasBot.rsc.modules.genericmodule.commands.create.embed
{
    public sealed class Embed
    {
        [JsonProperty("color", DefaultValueHandling = DefaultValueHandling.Include)]
        public string? Color { get; set; }

        [JsonProperty("author", NullValueHandling = NullValueHandling.Ignore)]
        public EmbedAuthor? Author { get; set; }

        [JsonProperty("thumbnail", NullValueHandling = NullValueHandling.Ignore)]
        public string? Thumbnail { get; set; }

        [JsonProperty("image", NullValueHandling = NullValueHandling.Ignore)]
        public string? Image { get; set; }

        [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
        public EmbedTitle? Title { get; set; }

        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public string? Description { get; set; }

        [JsonProperty("fields", NullValueHandling = NullValueHandling.Ignore)]
        public EmbedField[]? Fields { get; set; }

        [JsonProperty("footer", NullValueHandling = NullValueHandling.Ignore)]
        public EmbedFooter? Footer { get; set; }
    }
    public sealed class EmbedAuthor
    {
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string? Name { get; set; }

        [JsonProperty("image", NullValueHandling = NullValueHandling.Ignore)]
        public string? Image { get; set; }

        [JsonProperty("url", NullValueHandling = NullValueHandling.Ignore)]
        public string? Url { get; set; }
    }
    public sealed class EmbedTitle
    {
        [JsonProperty("text", NullValueHandling = NullValueHandling.Ignore)]
        public string? Text { get; set; }

        [JsonProperty("url", NullValueHandling = NullValueHandling.Ignore)]
        public string? Url { get; set; }
    }

    public sealed class EmbedField
    {
        [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
        public string? Title { get; set; }

        [JsonProperty("content", NullValueHandling = NullValueHandling.Ignore)]
        public string? Content { get; set; }

        [JsonProperty("inline", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Inline { get; set; }
    }
    public sealed class EmbedFooter
    {
        [JsonProperty("text", NullValueHandling = NullValueHandling.Ignore)]
        public string? Text { get; set; }

        [JsonProperty("image", NullValueHandling = NullValueHandling.Ignore)]
        public string? Image { get; set; }

        [JsonProperty("timestamp", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Timestamp { get; set; }
    }
}
