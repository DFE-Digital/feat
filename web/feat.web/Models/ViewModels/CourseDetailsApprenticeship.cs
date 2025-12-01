namespace feat.web.Models.ViewModels;

public class CourseDetailsApprenticeship : CourseDetailsBase
{
    public string? Wage { get; set; }
    
    public string? PositionAvailable { get; set; }
    
    public string? EmployerName { get; set; }
    
    public Location? EmployerAddress { get; set; }
    
    public string? EmployerUrl { get; set; }
    
    public string? EmployerDescription { get; set; }
    
    public string? TrainingProvider { get; set; }
    
    public StartDate? StartDate { get; set; }
}