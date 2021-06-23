namespace BlazorMapsCreator.Models
{
    public record Collections
    {
        public string ontology { get; init; }
        public Collection[] collections { get; init; }
        public SelfLink[] links { get; init; }
    }

}
