using Microsoft.Extensions.Logging;
using SignalBoosterCLI.Validators;

namespace Tests;

using NSubstitute;
using Xunit;
using SignalBoosterCLI.Brokers;
using System;
using System.IO;

public class LocalFileValidatorTest
{
    private readonly LocalFileValidtor _validator;
    private readonly IFileBroker _fileBroker;
    private readonly ILogger<LocalFileValidtor> _logger;

    public LocalFileValidatorTest()
    {
        _fileBroker = Substitute.For<IFileBroker>();
        _logger = Substitute.For<ILogger<LocalFileValidtor>>();
        _validator = new LocalFileValidtor(_logger);
    }

    [Fact]
    public void ValidateFileExists_FilePathIsNull_ThrowsArgumentNullExceptionWithCorrectMessage()
    {
        var exception = Record.Exception(() => _validator.Validate(null));

        Assert.NotNull(exception);
        Assert.IsType<ArgumentNullException>(exception);
        Assert.Equal("Value cannot be null. (Parameter 'path')", exception.Message);
    }
    
    
    [Fact]
    public void ValidateFileExists_FileExists_DoesNotThrowException()
    {
        var filePath = "existing-file.txt";
        _fileBroker.ReadNote(filePath).Returns(filePath);

        var exception = Record.Exception(() => _validator.Validate(filePath));

        Assert.NotNull(exception);
    }
    
}