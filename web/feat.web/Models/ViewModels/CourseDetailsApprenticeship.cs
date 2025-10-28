using System.Globalization;

namespace feat.web.Models.ViewModels;

public class CourseDetailsApprenticeship : CourseDetailsBase
{
    public decimal? Wage { private get; init; }
    
    public string WageDisplay => Wage?.ToString("C", new CultureInfo("en-GB")) ?? NotAvailableString;
    
    public string? PositionAvailable { get; init; }
    
    public string? EmployerName { get; init; }
    
    public Location? EmployerAddress { get; init; }
    
    public string? EmployerDescription { get; init; }
    
    public string? TrainingProvider { get; init; }
    
    public StartDate? StartDate { get; init; }
}