using System.ComponentModel;

namespace feat.common.Models.Enums;

public enum SourceSystem
{
    [Description("Find A Course / Publish To Course Directory")]
    FAC,
    [Description("Find An Apprenticeship")]
    FAA,
    [Description("Discover Uni / HESA")]
    DiscoverUni,
    [Description("Not Specified - This should not be used")]
    [Obsolete(error: false, message: "This should not be used")]
    NotSpecified
}