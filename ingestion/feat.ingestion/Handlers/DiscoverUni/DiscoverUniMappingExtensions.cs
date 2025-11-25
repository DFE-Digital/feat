using feat.common.Models.Enums;
using feat.ingestion.Models.DU.Enums;
using NetTopologySuite.Geometries;
using DU = feat.ingestion.Models.DU;
using Location = feat.common.Models.Location;

namespace feat.ingestion.Handlers.DiscoverUni;

public static class DiscoverUniMappingExtensions
{
    public static Location ToLocation(this DU.Location l, DU.Institution? i)
    {
        var location = new Location
        {
            Created = DateTime.Now,
            Updated = DateTime.Now,
            Name = l.Name,
            GeoLocation = new Point(new Coordinate(l.Longitude, l.Latitude)) { SRID = 4326 },
            Url = l.StudentUnionUrl,
            SourceReference = $"{l.UKPRN}_{l.LocationId}",
            SourceSystem = SourceSystem.DiscoverUni
        };

        if (string.IsNullOrEmpty(i?.Address)) return location;
        
        var splitAddress = i?.Address?.Split(",").Select(s => s.Trim()).ToList();
        if (splitAddress == null || splitAddress.Count < 2)
        {
            return location;
        }

        location.Postcode = splitAddress[^1];
        location.Town = splitAddress[^2];

        switch (splitAddress.Count)
        {
            case 3:
                location.Address1 = splitAddress[0];
                break;
            case 4:
                location.Address1 = splitAddress[0];
                location.Address2 = splitAddress[1];
                break;
            case 5:
                location.Address1 = splitAddress[0];
                location.Address2 = splitAddress[1];
                location.Address3 = splitAddress[2];
                break;
            case 6:
                location.Address1 = splitAddress[0];
                location.Address2 = splitAddress[1];
                location.Address3 = splitAddress[2];
                location.Address4 = splitAddress[3];
                break;
        }

        return location;

    }

    public static CourseHours? ToCourseHours(this StudyMode studyMode)
    {
        return studyMode switch
        {
            StudyMode.FullTime => CourseHours.FullTime,
            StudyMode.PartTime => CourseHours.PartTime,
            StudyMode.Both => CourseHours.Flexible,
            _ => null
        };
    }

    public static LearningMethod? ToStudyMode(this Availability distanceLearningAvailability)
    {
        return distanceLearningAvailability switch
        {
            Availability.Compulsory => LearningMethod.Online,
            Availability.Optional => LearningMethod.Hybrid,
            Availability.NotAvailable => LearningMethod.ClassroomBased,
            _ => null
        };
    }
}