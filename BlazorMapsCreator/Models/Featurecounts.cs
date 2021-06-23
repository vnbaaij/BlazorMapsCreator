namespace BlazorMapsCreator.Models
{

    public record Featurecounts
    {
        public int directoryInfo { get; init; }
        public int category { get; init; }
        public int facility { get; init; }
        public int level { get; init; }
        public int unit { get; init; }
        public int opening { get; init; }
        public int areaElement { get; init; }
        public int zone { get; init; }
        public int verticalPenetration { get; init; }
    }
}
