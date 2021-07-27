
namespace BlazorMapsCreator.Models
{
    public class Manifest
    {
        public string Version { get; set; }
        public Directoryinfo DirectoryInfo { get; set; }
        public Buildinglevels BuildingLevels { get; set; }
        public Georeference Georeference { get; set; }
        public Dwglayers DwgLayers { get; set; }
        public Unitproperty[] UnitProperties { get; set; }
        public Zoneproperty[] ZoneProperties { get; set; }
    }

    public class Directoryinfo
    {
        public string Name { get; set; }
        public string StreetAddress { get; set; }
        public string Unit { get; set; }
        public string Locality { get; set; }
        public string PostalCode { get; set; }
        public string[] AdminDivisions { get; set; }
        public string HoursOfOperation { get; set; }
        public string Phone { get; set; }
        public string Website { get; set; }
        public bool NonPublic { get; set; }
        public float AnchorLatitude { get; set; }
        public float AnchorLongitude { get; set; }
        public int AnchorHeightAboveSeaLevel { get; set; }
        public int DefaultLevelVerticalExtent { get; set; }
    }

    public class Buildinglevels
    {
        public Level[] Levels { get; set; }
    }

    public class Level
    {
        public string LevelName { get; set; }
        public int Ordinal { get; set; }
        public string Filename { get; set; }
        public int VerticalExtent { get; set; }
        public float HeightAboveFacilityAnchor { get; set; }
    }

    public class Georeference
    {
        public float Lat { get; set; }
        public float Lon { get; set; }
        public int Angle { get; set; }
    }

    public class Dwglayers
    {
        public string[] Exterior { get; set; }
        public string[] Unit { get; set; }
        public string[] Wall { get; set; }
        public string[] Door { get; set; }
        public string[] UnitLabel { get; set; }
        public string[] Zone { get; set; }
        public string[] ZoneLabel { get; set; }
    }

    public class Unitproperty
    {
        public string UnitName { get; set; }
        public string CategoryName { get; set; }
        public string[] NavigableBy { get; set; }
        public string RouteThroughBehavior { get; set; }
        public Occupant[] Occupants { get; set; }
        public string NameAlt { get; set; }
        public string NameSubtitle { get; set; }
        public string AddressRoomNumber { get; set; }
        public bool NonPublic { get; set; }
        public bool IsRoutable { get; set; }
        public bool IsOpenArea { get; set; }
        public string VerticalPenetrationCategory { get; set; }
        public string VerticalPenetrationDirection { get; set; }
    }

    public class Occupant
    {
        public string Name { get; set; }
        public string Phone { get; set; }
    }

    public class Zoneproperty
    {
        public string ZoneName { get; set; }
        public string CategoryName { get; set; }
        public string ZoneNameAlt { get; set; }
        public string ZoneNameSubtitle { get; set; }
        public string ZoneSetId { get; set; }
    }

}
