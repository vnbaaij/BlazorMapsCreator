using System.Collections.Generic;

namespace BlazorMapsCreator.Models
{
    public record AliasListResponse
    {
        public List<AliasListItem> aliases { get; set; }
    }

}
