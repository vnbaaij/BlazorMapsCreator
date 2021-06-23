using System;

namespace BlazorMapsCreator.Models
{
    public record AliasListItem
    {
        public DateTime createdTimestamp { get; init; }
        public string aliasId { get; init; }
        public string creatorDataItemId { get; init; }
        public DateTime lastUpdatedTimestamp { get; init; }
    }

}
