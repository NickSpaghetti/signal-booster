namespace Tests;
using Xunit;
using NSubstitute;
using Microsoft.Extensions.Logging;
using SignalBoosterCLI.Validators;
using SignalBoosterCLI.Models;

public class OrderValidatorTests
{
    private readonly OrderValidator _validator;
    private readonly ILogger<OrderValidator> _logger;

    public OrderValidatorTests()
    {
        _logger = Substitute.For<ILogger<OrderValidator>>();
        
        _validator = new OrderValidator(_logger);
    }
    
    
    [Fact] 
    public void ValidateOrder_OrderIsValid_DoesNotThrow()
    {
        var exception = Record.Exception(() => _validator.Validate(new Order()
        {
            Device = "CPAP",
            OrderingProvider = "Dr. Cicchetti",
            Usage = "Better sleep"
        }));

        Assert.Null(exception);
    }
    
    [Fact]
    public void Validate_OrderIsNull_ThrowsNullReferenceExceptionAndLogsError()
    {
        var exception = Record.Exception(() => _validator.Validate(null));

        Assert.NotNull(exception);
        Assert.IsType<NullReferenceException>(exception);
        Assert.Equal("Order cannot be null", exception.Message);
        
    }

    [Fact]
    public void Validate_DeviceIsNull_ThrowsNullReferenceExceptionAndLogsError()
    {
        var order = new Order { Device = null };

        var exception = Record.Exception(() => _validator.Validate(order));

        Assert.NotNull(exception);
        Assert.IsType<NullReferenceException>(exception);
        Assert.Equal("Device cannot be null or have a device name of Unknown", exception.Message);
        
    }
    
    [Fact]
    public void Validate_DeviceIsEmpty_ThrowsNullReferenceExceptionAndLogsError()
    {
        var order = new Order { Device = "" };

        var exception = Record.Exception(() => _validator.Validate(order));

        Assert.NotNull(exception);
        Assert.IsType<NullReferenceException>(exception);
        Assert.Equal("Device cannot be null or have a device name of Unknown", exception.Message);

    }

    [Fact]
    public void Validate_DeviceIsUnknown_ThrowsNullReferenceExceptionAndLogsError()
    {
        var order = new Order { Device = "Unknown" };

        var exception = Record.Exception(() => _validator.Validate(order));

        Assert.NotNull(exception);
        Assert.IsType<NullReferenceException>(exception);
        Assert.Equal("Device cannot be null or have a device name of Unknown", exception.Message);

    }

    [Fact]
    public void Validate_InvalidDeviceHasLiters_ThrowsInvalidDataExceptionAndLogsError()
    {
        var order = new Order { Device = "Other Device", Liters = "5", OrderingProvider = "Test Provider" };

        var exception = Record.Exception(() => _validator.Validate(order));

        Assert.NotNull(exception);
        Assert.IsType<InvalidDataException>(exception);
        Assert.Equal("Only the device Oxygen Tank can have Liters are supported", exception.Message);
        
    }

    [Fact]
    public void Validate_OrderingProviderIsNull_ThrowsNullReferenceExceptionAndLogsError()
    {
        var order = new Order { Device = "Oxygen Tank", Liters = "5", OrderingProvider = null };

        var exception = Record.Exception(() => _validator.Validate(order));

        Assert.NotNull(exception);
        Assert.IsType<NullReferenceException>(exception);
        Assert.Equal("Ordering Provider cannot be null or empty", exception.Message);

    }

    [Fact]
    public void Validate_OrderingProviderIsEmpty_ThrowsNullReferenceExceptionAndLogsError()
    {
        var order = new Order { Device = "Oxygen Tank", Liters = "5", OrderingProvider = "" };

        var exception = Record.Exception(() => _validator.Validate(order));

        Assert.NotNull(exception);
        Assert.IsType<NullReferenceException>(exception);
        Assert.Equal("Ordering Provider cannot be null or empty", exception.Message);
        
    }
    
}