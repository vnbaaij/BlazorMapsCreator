namespace BlazorMapsCreator.Models
{
    public record FeatureCollection
    {
        public string type { get; init; }
        public string ontology { get; init; }
        public Feature[] features { get; init; }
        public int numberReturned { get; init; }
        public SelfLink[] links { get; init; }
    }



}
