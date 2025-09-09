namespace SignalBoosterCLI.Models;

using System.Text.Json.Serialization;
using System.Collections.Generic;

public class Order
{
    [JsonPropertyName("device")]
    public string Device { get; set; }
    
    [JsonPropertyName("mask_type")]
    public string MaskType { get; set; }
    
    [JsonPropertyName("add_ons")]
    public List<string> AddOns { get; set; }
    
    [JsonPropertyName("qualifier")]
    public string Qualifier { get; set; }
    
    [JsonPropertyName("ordering_provider")]
    public string OrderingProvider { get; set; }
    
    [JsonPropertyName("liters")]
    public string? Liters { get; set; }
    
    [JsonPropertyName("usage")]
    public string? Usage { get; set; }
}