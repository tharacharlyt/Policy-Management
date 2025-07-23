namespace PolicyManagement.Models.DTOs;

public class CreatePolicyDto
{
    public string PolicyId { get; set; } = string.Empty;
    public string PolicyName { get; set; } = string.Empty;
    public double Premium { get; set; }
    public string Coverage { get; set; } = string.Empty;
    public int ValidityInYears { get; set; }
}