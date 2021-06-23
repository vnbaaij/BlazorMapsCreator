namespace BlazorMapsCreator.Models
{
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
