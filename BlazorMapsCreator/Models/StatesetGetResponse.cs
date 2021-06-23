using System.Collections.Generic;

namespace BlazorMapsCreator.Models
{
    public record StatesetGetResponse
    {
        public string description { get; set; }
        public List<string> datasetIds { get; set; }
        public Statesetstyle statesetStyle { get; set; }
    }



}
