using SignalBoosterCLI.Models;

namespace SignalBoosterCLI.Services.Foundation;

public interface IOrderCreationService
{
    Order CreateOrderFromNote(PhysicianNote physicianNote);
}