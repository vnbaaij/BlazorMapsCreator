namespace BlazorMapsCreator.Models
{
    public record Range
    {
        public double minimum { get; init; }
        public double maximum { get; init; }
        public double exclusiveMinumum { get; init; }
        public double exclusiveMaximum { get; init; }
    }

}
