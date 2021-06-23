namespace BlazorMapsCreator.Models
{
    public record Collection
    {
        public string name { get; init; }
        public string description { get; init; } = "";
        public Link[] links { get; init; }
    }

}
