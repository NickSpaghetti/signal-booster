namespace SignalBoosterCLI.Services;

using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

public class NoteProcessingService : INoteProcessingService
{
    public JsonObject ExtractOrder(string note)
    {
        string device = GetDevice(note);
        string? maskType = GetMaskType(note, device);
        string? addOns = GetAddOns(note);
        string qualifier = GetQualifier(note);
        string orderingProvider = GetOrderingProvider(note);
        string? liters = null;
        string? usage = null;

        if (device == "Oxygen Tank")
        {
            (liters, usage) = GetOxygenDetails(note);
        }

        var order = new JsonObject
        {
            ["device"] = device,
            ["mask_type"] = maskType,
            ["add_ons"] = addOns != null ? new JsonArray(addOns) : null,
            ["qualifier"] = qualifier,
            ["ordering_provider"] = orderingProvider
        };

        if (liters != null)
        {
            order["liters"] = liters;
        }

        if (usage != null)
        {
            order["usage"] = usage;
        }

        return order;
    }

    private string GetDevice(string note)
    {
        if (note.Contains("CPAP", StringComparison.OrdinalIgnoreCase))
        {
            return "CPAP";
        }

        if (note.Contains("oxygen", StringComparison.OrdinalIgnoreCase))
        {
            return "Oxygen Tank";
        }

        if (note.Contains("wheelchair", StringComparison.OrdinalIgnoreCase))
        {
            return "Wheelchair";
        }
        
        return "Unknown";
    }

    private string? GetMaskType(string note, string device) =>
        device == "CPAP" && note.Contains("full face", StringComparison.OrdinalIgnoreCase) ? "full face" : null;

    private string? GetAddOns(string note) =>
        note.Contains("humidifier", StringComparison.OrdinalIgnoreCase) ? "humidifier" : null;

    private string GetQualifier(string note) =>
        note.Contains("AHI > 20", StringComparison.OrdinalIgnoreCase) ? "AHI > 20" : string.Empty;

    private string GetOrderingProvider(string note)
    {
        int index = note.IndexOf("Dr.", StringComparison.OrdinalIgnoreCase);
        //remove /r/n when read from file
        return index >= 0 ? note.Substring(index).Replace("Ordered by ", "").Trim('.') : "Unknown";
    }

    private (string?, string?) GetOxygenDetails(string note)
    {
        Match match = Regex.Match(note, @"(\d+(\.\d+)?) ?L", RegexOptions.IgnoreCase);
        string? liters = match.Success ? $"{match.Groups[1].Value} L" : null;

        string? usage = null;
        bool sleep = note.Contains("sleep", StringComparison.OrdinalIgnoreCase);
        bool exertion = note.Contains("exertion", StringComparison.OrdinalIgnoreCase);

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

        return (liters, usage);
    }
}
