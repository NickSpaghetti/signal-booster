using System.Text.Json.Serialization;

namespace SignalBoosterCLI.Models;

public class PhysicianNote
{

    [JsonPropertyName("PatientName")]
    public string PatientName { get; set; }
    
    [JsonPropertyName("dob")]
    public string DOB { get; set; }
    
    [JsonPropertyName("diagnosis")]
    public string Diagnosis { get; set; }
    
    [JsonPropertyName("recommendation")]
    public string? Recommendation { get; set; }
    
    [JsonPropertyName("prescription")]
    public string? Prescription { get; set; }
    
    [JsonPropertyName("usage")]
    public string Usage { get; set; }
    
    [JsonPropertyName("OrderingPhysician")]
    public string OrderingPhysician { get; set; }
    
    [JsonPropertyName("AHI")]
    public string? AHI {get;set;}
}