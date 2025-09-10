using System.Text.Json;
using SignalBoosterCLI.Utilities.Serializers;

namespace SignalBoosterCLI.Services.Foundation;

using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using SignalBoosterCLI.Validators;
using Models;

public class OrderCreationService(IOrderValidator orderValidator, ILogger<OrderCreationService> logger) : IOrderCreationService
{
    
    public Order CreateOrderFromNote(PhysicianNote physicianNote)
    {   
        var perscription = physicianNote.Prescription ?? physicianNote.Recommendation;
        var device = GetDevice(perscription);
        var maskType = GetMaskType(perscription,device);
        var addOns = GetAddOns(perscription);
        var qualifiers = GetQualifier(physicianNote.AHI);
        var oxygenDetails = GetOxygenDetails(perscription);

        var order = new Order()
        {
            AddOns = addOns,
            Device = device,
            MaskType = maskType,
            OrderingProvider = physicianNote.OrderingPhysician,
            Qualifier = qualifiers,
            Usage = oxygenDetails?.Usage ?? physicianNote?.Usage,
            Liters = oxygenDetails?.Liters,
        };
        
        logger.LogInformation($"Created order {JsonSerializer.Serialize(order, LoggingJsonOptions.Options)}");
        
        orderValidator.Validate(order);
        
        return order;
    }
    
    private string? GetDevice(string prescription)
    {
        if (prescription.Contains("CPAP", StringComparison.OrdinalIgnoreCase))
        {
            return "CPAP";
        }

        if (prescription.Contains("oxygen", StringComparison.OrdinalIgnoreCase))
        {
            return "Oxygen Tank";
        }

        if (prescription.Contains("wheelchair", StringComparison.OrdinalIgnoreCase))
        {
            return "Wheelchair";
        }

        return null;
    }

    private string? GetMaskType(string prescription, string device)
    {
        if (!string.IsNullOrEmpty(device) && device != "CPAP")
        {
            return null;
        }
        
        if (prescription.Contains("full face", StringComparison.OrdinalIgnoreCase))
        {
            return "full face";
        }
        
        return  null;
    }

    private List<string>? GetAddOns(string prescription)
    {
        if (prescription.Contains("humidifier", StringComparison.OrdinalIgnoreCase))
        {
            return new List<string>(){"humidifier"};
        }

        return null;
    }


    private string GetQualifier(string qualifier)
    {
        int? qualifierValue = int.TryParse(qualifier, out var result) ? result : null;

        return qualifierValue switch
        {
            null => string.Empty,
            > 0 => $"AHI > {qualifierValue}",
            _ => string.Empty
        };
    }
        

    private string GetOrderingProvider(string note)
    {
        int index = note.IndexOf("Dr.", StringComparison.OrdinalIgnoreCase);
        //remove /r/n when read from file
        return index >= 0 ? note.Substring(index).Replace("Ordered by ", "").Trim('.') : "Unknown";
    }
    
    private OxygenDetails GetOxygenDetails(string note)
    {
        var match = Regex.Match(note, @"(\d+(\.\d+)?) ?L", RegexOptions.IgnoreCase);
        var liters = match.Success ? $"{match.Groups[1].Value} L" : null;
        
        var sleep = note.Contains("sleep", StringComparison.OrdinalIgnoreCase);
        var exertion = note.Contains("exertion", StringComparison.OrdinalIgnoreCase);
        
        string? usage = null;
        if (sleep && exertion)
        {
            usage = "sleep and exertion";
        }
        else if (sleep)
        {
            usage = "sleep";
        }
        else if (exertion)
        {
            usage = "exertion";
        }

        return new OxygenDetails(liters, usage);
    }

    private record OxygenDetails(string? Liters, string? Usage);
}