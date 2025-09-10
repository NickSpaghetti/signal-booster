namespace SignalBoosterCLI.Services.Orchestrations;

using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SignalBoosterCLI.Models;
using SignalBoosterCLI.Services.Foundation;

public class OrderOrchestrationService(ILogger<OrderOrchestrationService> logger,
    IConfiguration config,
    IOrderCreationService orderCreationService, 
    IPhysicianNoteParsingService  physicianNoteParsingService,
    ILocalFileService localFileService) : IOrderOrchestrationService
{
    public Order? CreateOrderFromPhysicianNoteFile(string physicianNoteFilePath)
    {
        try
        {
           var content = localFileService.ReadFile(physicianNoteFilePath);
           var physiciansNote = physicianNoteParsingService.ParseNote(content);
           var order = orderCreationService.CreateOrderFromNote(physiciansNote);
           return order;

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating order");
            return null;
        }
    }

    public Order CreateOrderFromPhysicianNote(string note)
    {
        var physiciansNote = physicianNoteParsingService.ParseNote(note);
        var order = orderCreationService.CreateOrderFromNote(physiciansNote);
        return order;
    }

    public async ValueTask SendOrderToVendorAsync(Order order)
    {
        var jsonString = JsonSerializer.Serialize(order, new JsonSerializerOptions {  DefaultIgnoreCondition = JsonIgnoreCondition.Never});

        var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

        logger.LogInformation("Sent order to vendor");
        var baseAddress = config["vendor_api_baseurl"] ?? "http://localhost:5000";
        using var httpClient = new HttpClient();
        //var response = await httpClient.PostAsync("https://alert-api.com/DrExtract", content);
        var response = await httpClient.PostAsync($"{baseAddress}/api/DrExtract", content);
        response.EnsureSuccessStatusCode();
        logger.LogInformation($"Response from vendor:   {await response.Content.ReadAsStringAsync()}");
        
    }
}