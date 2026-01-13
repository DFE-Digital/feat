namespace feat.api.Models;

public class ValidationResult
{
    public bool IsValid => Errors.Count == 0;
    
    public Dictionary<string, string[]> Errors { get; } = new();

    public void AddError(string field, string message)
    {
        if (Errors.TryGetValue(field, out var existing))
        {
            Errors[field] = existing.Append(message).ToArray();
        }
        else
        {
            Errors[field] = [message];
        }
    }
}