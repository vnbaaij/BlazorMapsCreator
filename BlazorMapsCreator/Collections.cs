namespace BlazorMapsCreator
{
    public record Collections
    {
        public string ontology { get; init; }
        public Collection[] collections { get; init; }
        public SelfLink[] links { get; init; }
    }

    public record Collection
    {
        public string name { get; init; }
        public string description { get; init; }
        public Link[] links { get; init; }
    }

    public record Link
    {
        public string href { get; init; }
        public string rel { get; init; }
        public string title { get; init; }
    }

    public record SelfLink
    {
        public string href { get; init; }
        public string rel { get; init; }
    }

}
