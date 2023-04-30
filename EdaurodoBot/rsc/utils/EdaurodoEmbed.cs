using Newtonsoft.Json;
using System.Collections.Immutable;
using System.Drawing;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

namespace EdaurodoBot.rsc.utils
{
    public sealed class EdaurodoEmbed
    {
        [JsonProperty("color", NullValueHandling = NullValueHandling.Ignore)]
        private string _color;

        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        private string? _description;

        [JsonProperty("thumbnail", NullValueHandling = NullValueHandling.Ignore)]
        private string? _thumbnail;

        [JsonProperty("image", NullValueHandling = NullValueHandling.Ignore)]
        private string? _image;

        [JsonProperty("author", NullValueHandling = NullValueHandling.Ignore)]
        private EdaurodoEmbedAuthor? _author;

        [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
        private EdaurodoEmbedTitle? _title;

        [JsonProperty("footer", NullValueHandling = NullValueHandling.Ignore)]
        private EdaurodoEmbedFooter? _footer;

        [JsonProperty("fields", NullValueHandling = NullValueHandling.Ignore)]
        private IEnumerable<EdaurodoEmbedField>? _fields;

        [JsonIgnore]
        public string? Color
        {
            get => _color;
            set => _color = Regex.IsMatch(value ?? "#2B2D31", "[#][a-fA-F0-9]{6}|[a-fA-F0-9]{6}") ? value ?? "#2B2D31" : "#2B2D31";
        }
        [JsonIgnore]
        public string? Description
        {
            get => _description;
            set => _description = !string.IsNullOrWhiteSpace(value) && value.Length <= 4096 ? value : null;
        }
        [JsonIgnore]
        public string? Thumbnail
        {
            get => _thumbnail;
            set => _thumbnail = Uri.TryCreate(value, UriKind.Absolute, out _) ? value : null;
        }
        [JsonIgnore]
        public string? Image
        {
            get => _image;
            set => _image = Uri.TryCreate(value, UriKind.Absolute, out _) ? value : null;
        }
        [JsonIgnore]
        public EdaurodoEmbedAuthor? Author
        {
            get => _author;
            set => _author = value != null && (value.Name != null || value.Image != null) ? value : new EdaurodoEmbedAuthor(null, null, null);
        }
        [JsonIgnore]
        public EdaurodoEmbedTitle? Title
        {
            get => _title;
            set => _title = value != null && value.Value != null ? value : new EdaurodoEmbedTitle(null, null);
        }
        [JsonIgnore]
        public EdaurodoEmbedFooter? Footer
        {
            get => _footer;
            set => _footer = value != null && (value.Value != null || value.Image != null || value.Timestamp == true) ? value : new EdaurodoEmbedFooter(null, null, false);
        }
        [JsonIgnore]
        public IEnumerable<EdaurodoEmbedField>? Fields
        {
            get => _fields;
            set
            {
                if (value != null && value.Count() <= 25) { _fields = value.ToList().FindAll(_ => _ != null && _.Title != null && _.Value != null && _.Inline != null); }
                else { _fields = new List<EdaurodoEmbedField>(); }
            }
        }
        [JsonIgnore]
        public static ImmutableDictionary<string, string> Colors { get; } = new Dictionary<string, string>() {
            {"#2B2D31", "Neutro" },
            {"#F0FFFF", "Azure"},
            {"#6495ED", "CornflowerBlue"},
            {"#FF7F50", "Coral"},
            {"#E9967A", "DarkSalmon"},
            {"#FF1493", "DeepPink"},
            {"#00BFFF", "DeepSkyBlue"},
            {"#B22222", "FireBrick"},
            {"#FF69B4", "HotPink"},
            {"#CD5C5C", "IndianRed"},
            {"#F0E68C", "Khaki"},
            {"#20B2AA", "LightSeaGreen"},
            {"#B0C4DE", "LightSteelBlue"},
            {"#FFE4E1", "MistyRose"},
            {"#FFE4B5", "Moccasin"},
            {"#98FB98", "PaleGreen"},
            {"#AFEEEE", "PaleTurquoise"},
            {"#DB7093", "PaleVioletRed"},
            {"#FFDAB9", "PeachPuff"},
            {"#B0E0E6", "PowderBlue"},
            {"#FA8072", "Salmon"},
            {"#008080", "Teal"},
            {"#FF6347", "Tomato"},
            {"#FFFF00", "Yellow"},
        }.ToImmutableDictionary();
        public EdaurodoEmbed(string? color, string? description, string? thumbnail, string? image, EdaurodoEmbedAuthor? author, EdaurodoEmbedTitle? title, EdaurodoEmbedFooter? footer, IEnumerable<EdaurodoEmbedField>? fields)
        {
            Color = color;
            Description = description;
            Thumbnail = thumbnail;
            Image = image;
            Author = author;
            Title = title;
            Footer = footer;
            Fields = fields;
        }
    }
    public sealed class EdaurodoEmbedAuthor
    {
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        private string? _name;
        [JsonProperty("image", NullValueHandling = NullValueHandling.Ignore)]
        private string? _image;
        [JsonProperty("url", NullValueHandling = NullValueHandling.Ignore)]
        private string? _url;
        [JsonIgnore]
        public string? Name
        {
            get => _name;
            set => _name = !string.IsNullOrWhiteSpace(value) && value.Length <= 256 ? value : null;
        }
        [JsonIgnore]
        public string? Image
        {
            get => _image;
            set => _image = Uri.TryCreate(value, UriKind.Absolute, out _) ? value : null;
        }
        [JsonIgnore]
        public string? Url
        {
            get => _url;
            set => _url = Uri.TryCreate(value, UriKind.Absolute, out _) && this._name != null ? value : null;
        }
        public EdaurodoEmbedAuthor(string? name, string? image, string? url)
        {
            Name = name;
            Image = image;
            Url = url;
        }
    }
    public sealed class EdaurodoEmbedTitle
    {
        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        private string? _value;
        [JsonProperty("url", NullValueHandling = NullValueHandling.Ignore)]
        private string? _url;
        [JsonIgnore]
        public string? Value
        {
            get => _value;
            set => _value = !string.IsNullOrWhiteSpace(value) && value.Length <= 256 ? value : null;
        }
        [JsonIgnore]
        public string? Url
        {
            get => _url;
            set => _url = this._value != null && Uri.TryCreate(value, UriKind.Absolute, out _) ? value : null;
        }
        public EdaurodoEmbedTitle(string? value, string? url)
        {
            Value = value;
            Url = url;
        }
    }
    public sealed class EdaurodoEmbedField
    {
        [JsonProperty("title")]
        private string? _title;
        [JsonProperty("value")]
        private string? _value;
        [JsonProperty("inline")]
        private bool? _inline;
        [JsonIgnore]
        public string? Title
        {
            get => _title;
            set => _title = !string.IsNullOrWhiteSpace(value) && value.Length <= 256 ? value : null;
        }
        [JsonIgnore]
        public string? Value
        {
            get => _value;
            set => _value = !string.IsNullOrWhiteSpace(value) && _title != null && value.Length <= 1024 ? value : null;
        }
        [JsonIgnore]
        public bool? Inline
        {
            get => _inline;
            set => _inline = value != null && _title != null && _value != null ? value : null;
        }
        public EdaurodoEmbedField(string title, string value, bool inline)
        {
            Title = title;
            Value = value;
            Inline = inline;
        }
    }
    public sealed class EdaurodoEmbedFooter
    {
        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        private string? _value;
        [JsonProperty("image", NullValueHandling = NullValueHandling.Ignore)]
        private string? _image;
        [JsonProperty("timestamp", NullValueHandling = NullValueHandling.Ignore)]
        public bool? _timestamp;
        [JsonIgnore]
        public string? Value
        {
            get => _value;
            set => _value = !string.IsNullOrEmpty(value) && value.Length <= 2048 ? value : null;
        }
        [JsonIgnore]
        public string? Image
        {
            get => _image;
            set => _image = Uri.TryCreate(value, UriKind.Absolute, out _) ? value : null;
        }
        [JsonIgnore]
        public bool? Timestamp
        {
            get => _timestamp;
            set => _timestamp = value != null ? value : null;
        }
        public EdaurodoEmbedFooter(string? value, string? image, bool? timestamp)
        {
            Value = value;
            Image = image;
            Timestamp = timestamp;
        }
    }
    public sealed class EdaurodoEmbedSerializable
    {
        [JsonProperty("color", NullValueHandling = NullValueHandling.Ignore)]
        private string _color;

        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        private string? _description;

        [JsonProperty("thumbnail", NullValueHandling = NullValueHandling.Ignore)]
        private string? _thumbnail;

        [JsonProperty("image", NullValueHandling = NullValueHandling.Ignore)]
        private string? _image;

        [JsonProperty("author", NullValueHandling = NullValueHandling.Ignore)]
        private EdaurodoEmbedAuthor? _author;

        [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
        private EdaurodoEmbedTitle? _title;

        [JsonProperty("footer", NullValueHandling = NullValueHandling.Ignore)]
        private EdaurodoEmbedFooter? _footer;

        [JsonProperty("fields", NullValueHandling = NullValueHandling.Ignore)]
        private IEnumerable<EdaurodoEmbedField>? _fields;
        [JsonIgnore]
        public string? Color
        {
            get => _color;
            private set => _color = Regex.IsMatch(value ?? "#2B2D31", "[#][a-fA-F0-9]{6}|[a-fA-F0-9]{6}") ? value ?? "#2B2D31" : "#2B2D31";
        }
        [JsonIgnore]
        public string? Description
        {
            get => _description;
            private set => _description = !string.IsNullOrWhiteSpace(value) && value.Length <= 4096 ? value : null;
        }
        [JsonIgnore]
        public string? Thumbnail
        {
            get => _thumbnail;
            private set => _thumbnail = Uri.TryCreate(value, UriKind.Absolute, out _) ? value : null;
        }
        [JsonIgnore]
        public string? Image
        {
            get => _image;
            private set => _image = Uri.TryCreate(value, UriKind.Absolute, out _) ? value : null;
        }
        [JsonIgnore]
        public EdaurodoEmbedAuthor? Author
        {
            get => _author;
            private set => _author = value != null && (value.Name != null || value.Image != null) ? value : null;
        }
        [JsonIgnore]
        public EdaurodoEmbedTitle? Title
        {
            get => _title;
            private set => _title = value != null && value.Value != null ? value : null;
        }
        [JsonIgnore]
        public EdaurodoEmbedFooter? Footer
        {
            get => _footer;
            private set => _footer = value != null && (value.Value != null || value.Image != null || value.Timestamp == true) ? value : null;
        }

        [JsonIgnore]
        public IEnumerable<EdaurodoEmbedField>? Fields
        {
            get => _fields;
            private set
            {
                if (value != null && value.Count() <= 25)
                {
                    var temp = value.ToList().FindAll(_ => _ != null && _.Title != null && _.Value != null && _.Inline != null);
                    if (temp.Count > 0) { _fields = temp; }
                    else { _fields = null; }
                }
                else
                {
                    _fields = null;
                }
            }
        }
        public EdaurodoEmbedSerializable(EdaurodoEmbed embed)
        {
            Color = embed.Color;
            Description = embed.Description;
            Thumbnail = embed.Thumbnail;
            Image = embed.Image;
            Author = embed.Author;
            Title = embed.Title;
            Footer = embed.Footer;
            Fields = embed.Fields;
        }
    }
}