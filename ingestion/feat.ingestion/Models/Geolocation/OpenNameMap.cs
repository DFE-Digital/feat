using CsvHelper.Configuration;

namespace feat.ingestion.Models.Geolocation;

public sealed class OpenNameMap : ClassMap<OpenName>
{
    public OpenNameMap()
    {
        Map(m => m.Name1).Index(2);
        Map(m => m.Name1Lang).Index(3);
        Map(m => m.Name2).Index(4);
        Map(m => m.Name2Lang).Index(5);
        Map(m => m.Type).Index(6);
        Map(m => m.LocalType).Index(7);
        Map(m => m.X).Index(8);
        Map(m => m.Y).Index(9);
        Map(m => m.District).Index(21);
        Map(m => m.County).Index(24);
        Map(m => m.Country).Index(29);
    }
    
    
}