using SignalBoosterCLI.Models;
using SignalBoosterCLI.Validators;

namespace Tests;

public class PhysicianNoteValidatorTest
{
     private readonly PhysicianNoteValidator _validator;

    public PhysicianNoteValidatorTest()
    {
        _validator = new PhysicianNoteValidator();
    }

    [Fact]
    public void Validate_NoteIsNull_ThrowsArgumentNullExceptionWithCorrectMessage()
    {
        var exception = Record.Exception(() => _validator.Validate(null));
        
        Assert.NotNull(exception);
        Assert.IsType<ArgumentNullException>(exception);
        Assert.Equal("Value cannot be null. (Parameter 'note')", exception.Message);
    }

    [Fact]
    public void Validate_DOBIsNull_ThrowsArgumentExceptionWithCorrectMessage()
    {
        var note = new PhysicianNote { DOB = null, Diagnosis = "Valid Diagnosis" };
        
        var exception = Record.Exception(() => _validator.Validate(note));
        
        Assert.NotNull(exception);
        Assert.IsType<ArgumentException>(exception);
        Assert.Equal("Invalid DOB. DOB cannot be null, empty, or a future date.", exception.Message);
    }

    [Fact]
    public void Validate_DOBIsEmpty_ThrowsArgumentExceptionWithCorrectMessage()
    {
        var note = new PhysicianNote { DOB = "", Diagnosis = "Valid Diagnosis" };
        
        var exception = Record.Exception(() => _validator.Validate(note));
        
        Assert.NotNull(exception);
        Assert.IsType<ArgumentException>(exception);
        Assert.Equal("Invalid DOB. DOB cannot be null, empty, or a future date.", exception.Message);
    }

    [Fact]
    public void Validate_DOBIsInvalidDate_ThrowsArgumentExceptionWithCorrectMessage()
    {
        var note = new PhysicianNote { DOB = "not a date", Diagnosis = "Valid Diagnosis" };
        
        var exception = Record.Exception(() => _validator.Validate(note));
        
        Assert.NotNull(exception);
        Assert.IsType<ArgumentException>(exception);
        Assert.Equal("Invalid DOB. DOB cannot be null, empty, or a future date.", exception.Message);
    }

    [Fact]
    public void Validate_DOBIsFutureDate_ThrowsArgumentExceptionWithCorrectMessage()
    {
        var futureDate = DateTime.Today.AddDays(1).ToString("MM/dd/yyyy");
        var note = new PhysicianNote { DOB = futureDate, Diagnosis = "Valid Diagnosis" };
        
        var exception = Record.Exception(() => _validator.Validate(note));
        
        Assert.NotNull(exception);
        Assert.IsType<ArgumentException>(exception);
        Assert.Equal("Invalid DOB. DOB cannot be null, empty, or a future date.", exception.Message);
    }

    [Fact]
    public void Validate_DiagnosisIsNull_ThrowsArgumentExceptionWithCorrectMessage()
    {
        var note = new PhysicianNote { DOB = "01/01/2000", Diagnosis = null };
        
        var exception = Record.Exception(() => _validator.Validate(note));
        
        Assert.NotNull(exception);
        Assert.IsType<ArgumentException>(exception);
        Assert.Equal("Diagnosis cannot be null or empty.", exception.Message);
    }

    [Fact]
    public void Validate_DiagnosisIsEmpty_ThrowsArgumentExceptionWithCorrectMessage()
    {
        var note = new PhysicianNote { DOB = "01/01/2000", Diagnosis = "" };
        
        var exception = Record.Exception(() => _validator.Validate(note));
        
        Assert.NotNull(exception);
        Assert.IsType<ArgumentException>(exception);
        Assert.Equal("Diagnosis cannot be null or empty.", exception.Message);
    }
    
    [Fact]
    public void Validate_ValidNote_DoesNotThrowException()
    {
        var note = new PhysicianNote { DOB = "01/01/2000", Diagnosis = "Valid Diagnosis" };
        
        var exception = Record.Exception(() => _validator.Validate(note));
        
        Assert.Null(exception);
    }
}