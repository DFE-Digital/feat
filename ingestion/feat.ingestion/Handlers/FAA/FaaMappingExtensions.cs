using feat.ingestion.Models.FAA;
using Database = feat.common.Models.Staging.FAA;

namespace feat.ingestion.Handlers.FAA;

public static class FaaMappingExtensions
{
    public static Database.Apprenticeship FromDto(this Apprenticeship apprenticeship)
    {
        var dbApprenticeship = new Database.Apprenticeship
        {
            Title = apprenticeship.Title,
            Description = apprenticeship.Description,
            NumberOfPositions = apprenticeship.NumberOfPositions,
            PostedDate = apprenticeship.PostedDate,
            ClosingDate = apprenticeship.ClosingDate,
            StartDate = apprenticeship.StartDate,
            WageType = apprenticeship.Wage.WageType,
            WageAmount = apprenticeship.Wage.WageAmount,
            WageUnit = apprenticeship.Wage.WageUnit,
            WageAdditionalInformation = apprenticeship.Wage.WageAdditionalInformation,
            WorkingWeekDescription = apprenticeship.Wage.WorkingWeekDescription,
            HoursPerWeek = apprenticeship.HoursPerWeek,
            ExpectedDuration = apprenticeship.ExpectedDuration,
            ApplicationUrl = apprenticeship.ApplicationUrl,
            Distance = apprenticeship.Distance,
            EmployerName = apprenticeship.EmployerName,
            EmployerWebsiteUrl = apprenticeship.EmployerWebsiteUrl,
            EmployerContactName = apprenticeship.EmployerContactName,
            EmployerContactPhone = apprenticeship.EmployerContactPhone,
            EmployerContactEmail = apprenticeship.EmployerContactEmail,
            CourseLarsCode = apprenticeship.Course.LarsCode,
            CourseTitle = apprenticeship.Course.Title,
            CourseLevel = apprenticeship.Course.Level,
            CourseRoute = apprenticeship.Course.Route,
            CourseType = apprenticeship.Course.Type,
            ApprenticeshipLevel = apprenticeship.ApprenticeshipLevel,
            ProviderName = apprenticeship.ProviderName,
            Ukprn = apprenticeship.Ukprn,
            IsDisabilityConfident = apprenticeship.IsDisabilityConfident,
            VacancyUrl = apprenticeship.VacancyUrl,
            VacancyReference = apprenticeship.VacancyReference,
            IsNationalVacancy = apprenticeship.IsNationalVacancy,
            IsNationalVacancyDetails = apprenticeship.IsNationalVacancyDetails
        };
        
        if (apprenticeship.Addresses != null && apprenticeship.Addresses.Count != 0)
        {
            dbApprenticeship.Addresses = apprenticeship.Addresses.FromDtoList();
        }

        return dbApprenticeship;
    }
    
    public static List<Database.Apprenticeship> FromDtoList(this List<Apprenticeship> apprenticeships)
    {
        return apprenticeships.Select(apprenticeship => apprenticeship.FromDto()).ToList();
    }
    
    public static Database.Address FromDto(this Address address)
    {
        return new Database.Address
        {
            AddressLine1 = address.AddressLine1,
            AddressLine2 = address.AddressLine2,
            AddressLine3 = address.AddressLine3,
            AddressLine4 = address.AddressLine4,
            Postcode = address.Postcode,
            Latitude = address.Latitude,
            Longitude = address.Longitude
        };
    }
    
    public static List<Database.Address> FromDtoList(this List<Address> addresses)
    {
        return addresses.Select(address => address.FromDto()).ToList();
    }
}