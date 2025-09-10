using SignalBoosterCLI.Models;

namespace SignalBoosterCLI.Validators;

public class OrderValidator
{
    public void Validate(Order? order)
    {
        if (order is null)
        {
            throw new NullReferenceException("Order cannot be null");
        }

        if (string.IsNullOrEmpty(order.Device) || order.Device == "Unknown")
        {
            throw new NullReferenceException("Device cannot be null or have a device name of Unknown");
        }

        if (order.Device != "Oxygen Tank" && order.Liters != null)
        {
            throw new InvalidDataException("Only the device Oxygen Tank can have Liters are supported");
        }

        if (string.IsNullOrEmpty(order.OrderingProvider))
        {
            throw new NullReferenceException("Ordering Provider cannot be null or empty");
        }
        
        
    }
}