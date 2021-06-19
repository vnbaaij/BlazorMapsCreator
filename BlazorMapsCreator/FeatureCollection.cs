namespace BlazorMapsCreator
{
    public record FeatureCollection
    {
        public string type { get; init; }
        public string ontology { get; init; }
        public Feature[] features { get; init; }
        public int numberReturned { get; init; }
        public SelfLink[] links { get; init; }
    }

    public record Feature
    {
        public string type { get; init; }
        public Geometry geometry { get; init; }
        public Properties properties { get; init; }
        public string id { get; init; }
        public string featureType { get; init; }
    }

    public record Geometry
    {
        public string type { get; init; }
        public float[][][] coordinates { get; init; }
    }

    public record Properties
    {
        public string originalId { get; init; }
        public string categoryId { get; init; }
        public bool isOpenArea { get; init; }
        public bool isRoutable { get; init; }
        public string levelId { get; init; }
        public object[] occupants { get; init; }
        public string addressId { get; init; }
        public string name { get; init; }
    }
}
