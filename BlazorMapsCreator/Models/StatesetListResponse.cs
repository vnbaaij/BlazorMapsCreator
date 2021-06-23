using System.Collections.Generic;

namespace BlazorMapsCreator.Models
{
    public record StatesetListResponse
    {
        public List<StatesetInfoObject> statesets { get; set; }
    }



}
