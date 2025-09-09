using System.Text.Json;
using System.Text.Json.Serialization;
using SignalBoosterCLI.Validators;

namespace SignalBoosterCLI.Services.Foundation;

using SignalBoosterCLI.Models;

public class OrderParseService
{

    public Order ParseOrder(string fileContent)
    {
        // Check if the content is a JSON object. This is a simple check
        // but could be expanded with more robust validation.
        if (fileContent.Trim().StartsWith("{"))
        {
            // If the content is JSON, parse it as a JSON file.
            return ParseJson(fileContent);
        }
        else
        {
            // Otherwise, assume it is a text file and parse accordingly.
            return ParseText(fileContent);
        }
    }
    
    private Order ParseText(string fileContent)
    {
        var order = new Order();
        var lines = fileContent.Split('\n');

        foreach (var line in lines)
        {
            // Clean up the line by trimming whitespace and carriage returns
            var cleanedLine = line.Trim().Replace("\r", "");
            if (cleanedLine.StartsWith("Patient Name:"))
            {
                // This data doesn't map to the current model, so we can ignore it
                // or handle it in a more complex model later.
            }
            else if (cleanedLine.StartsWith("Prescription:"))
            {
                var prescriptionData = cleanedLine.Replace("Prescription:", "").Trim();
                // Extract liters from a string like "Requires a portable oxygen tank delivering 2 L per minute."
                var parts = prescriptionData.Split("delivering ");
                if (parts.Length > 1)
                {
                    order.Liters = parts[1].Split(" ")[0] + " " + parts[1].Split(" ")[1];
                }
            }
            else if (cleanedLine.StartsWith("Usage:"))
            {
                order.Usage = cleanedLine.Replace("Usage:", "").Trim();
            }
            else if (cleanedLine.StartsWith("Ordering Physician:"))
            {
                order.OrderingProvider = cleanedLine.Replace("Ordering Physician:", "").Trim();
            }
        }
        
        // This example shows how to handle the logic when there is no data provided
        // or a null is expected for the model's properties.
        if (string.IsNullOrEmpty(order.Liters))
        {
            order.Liters = null;
        }

        if (string.IsNullOrEmpty(order.Usage))
        {
            order.Usage = null;
        }

        return order;
        
    }
    
    private Order ParseJson(string jsonContent)
    {
        // The first JSON example has a 'data' field, which contains the text.
        // We need to parse that first.
        if (jsonContent.Contains("\"data\":"))
        {
            var dataWrapper = JsonSerializer.Deserialize<JsonDataWrapper>(jsonContent);
            return ParseText(dataWrapper.Data);
        }

        // The third JSON example has the fields directly, so we can deserialize it.
        return JsonSerializer.Deserialize<Order>(jsonContent);
    }

    private class JsonDataWrapper
    {
        [JsonPropertyName("data")]
        public string Data { get; set; }
    }

}