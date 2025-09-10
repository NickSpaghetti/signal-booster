using Microsoft.Extensions.Logging;
using SignalBoosterCLI.Models;

namespace SignalBoosterCLI.Validators;

public interface IOrderValidator
{
    void Validate(Order? order);
}

public class OrderValidator(ILogger<OrderValidator> logger) : IOrderValidator
{
    public void Validate(Order? order)
    {
        if (order is null)
        {
            logger.LogError($"{nameof(order)} is null");
            throw new NullReferenceException("Order cannot be null");
        }

        if (string.IsNullOrEmpty(order.Device) || order.Device == "Unknown")
        {
            logger.LogError($"{nameof(order.Device)} is null, empty, or Unknown");
            throw new NullReferenceException("Device cannot be null or have a device name of Unknown");
        }

        if (order.Device != "Oxygen Tank" && order.Liters != null)
        {
            logger.LogError($"{nameof(order.Device)} is invalid");
            throw new InvalidDataException("Only the device Oxygen Tank can have Liters are supported");
        }

        if (string.IsNullOrEmpty(order.OrderingProvider))
        {
            logger.LogError($"{nameof(order.OrderingProvider)} is null or empty");
            throw new NullReferenceException("Ordering Provider cannot be null or empty");
        }
        
        
    }
}