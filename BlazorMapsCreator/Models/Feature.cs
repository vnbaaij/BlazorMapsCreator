namespace BlazorMapsCreator.Models
{
    public record Feature
    {
        public string type { get; init; }
        public Geometry geometry { get; init; }
        public Properties properties { get; init; }
        public string id { get; init; }
        public string featureType { get; init; }
    }
}
