using System.Collections.Generic;

namespace BlazorMapsCreator.Models
{
    public record Style
    {
        public string keyName { get; init; }
        public string type { get; init; }
        public List<Dictionary<string, string>> rules { get; init; }
    }

}
