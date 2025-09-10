using SignalBoosterCLI.Models;

namespace SignalBoosterCLI.Services.Orchestrations;

public interface IOrderOrchestrationService
{
    Order? CreateOrderFromPhysicianNoteFile(string physicianNoteFilePath);
    Order CreateOrderFromPhysicianNote(string note);
    ValueTask SendOrderToVendorAsync(Order order);
}