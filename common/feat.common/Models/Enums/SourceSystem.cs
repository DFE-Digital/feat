using System.ComponentModel;

namespace feat.common.Models.Enums;

public enum SourceSystem
{
    [Description("Find A Course / Publish To Course Directory")]
    FAC,
    [Description("Find An Apprenticeship")]
    FAA,
    [Description("Discover Uni / HESA")]
    DiscoverUni
}