namespace Tests;

using NSubstitute;
using Xunit;
using Microsoft.Extensions.Logging;
using SignalBoosterCLI.Validators;
using SignalBoosterCLI.Services.Foundation;
using SignalBoosterCLI.Models;

public class PhysicianNoteParsingServiceTest
{
    private readonly ILogger<PhysicianNoteParsingService> _logger;
    private readonly IPhysicianNoteValidator _physicianNoteValidator;
    private readonly PhysicianNoteParsingService _service;

    public PhysicianNoteParsingServiceTest()
    {
        _logger = Substitute.For<ILogger<PhysicianNoteParsingService>>();
        _physicianNoteValidator = Substitute.For<IPhysicianNoteValidator>();
        _service = new PhysicianNoteParsingService(_physicianNoteValidator, _logger);
    }
    
    [Fact]
    public void ParseNote_WithValidJsonNote_ReturnsCorrectlyParsedNote()
    {
        var path = Path.Combine("TestFiles", "physican_note1.json");
        var noteContent = File.ReadAllText(path);
        var expectedNote = new PhysicianNote
        {
            PatientName = "Harold Finch",
            DOB = "04/12/1952",
            Diagnosis = "COPD",
            Prescription = "Requires a portable oxygen tank delivering 2 L per minute.",
            Usage = "During sleep and exertion.",
            OrderingPhysician = "Dr. Cuddy"
        };


        var result = _service.ParseNote(noteContent);

        Assert.Equal(expectedNote.PatientName, result.PatientName);
        Assert.Equal(expectedNote.DOB, result.DOB);
        Assert.Equal(expectedNote.Diagnosis, result.Diagnosis);
        Assert.Equal(expectedNote.Prescription, result.Prescription);
        Assert.Equal(expectedNote.Usage, result.Usage);
        
    }

    [Fact]
    public void ParseNote_WithValidJsonWithCrlf_ReturnsCorrectlyParsedNote()
    {
        var noteContent = "{\"data\":\"Patient Name: Jane Doe\\nDOB: 02/02/1980\\nDiagnosis: Valid Diagnosis\"}";
        var expectedNote = new PhysicianNote
        {
            PatientName = "Jane Doe",
            DOB = "02/02/1980",
            Diagnosis = "Valid Diagnosis"
        };

        var result = _service.ParseNote(noteContent);

        Assert.Equal(expectedNote.PatientName, result.PatientName);
        Assert.Equal(expectedNote.DOB, result.DOB);
        Assert.Equal(expectedNote.Diagnosis, result.Diagnosis);
        
    }

    [Fact]
    public void ParseNote_WithCrlf_ReturnsCorrectlyParsedNote()
    {
        var path = Path.Combine("TestFiles", "physician_note1.txt");
        var noteContent = File.ReadAllText(path);
        
        var expectedNote = new PhysicianNote
        {
            PatientName = "Harold Finch",
            DOB = "04/12/1952",
            Diagnosis = "COPD",
            Prescription = "Requires a portable oxygen tank delivering 2 L per minute.",
            Usage = "During sleep and exertion.",
            OrderingPhysician = "Dr. Cuddy"
        };


        var result = _service.ParseNote(noteContent);

        Assert.Equal(expectedNote.PatientName, result.PatientName);
        Assert.Equal(expectedNote.DOB, result.DOB);
        Assert.Equal(expectedNote.Diagnosis, result.Diagnosis);
        Assert.Equal(expectedNote.Prescription, result.Prescription);
        Assert.Equal(expectedNote.Usage, result.Usage);
    }
    
    [Fact]
    public void ParseNote_WithRegex_ReturnsCorrectlyParsedNote()
    {
        var path = Path.Combine("TestFiles", "physician_note3.txt");
        var noteContent = File.ReadAllText(path);
        
        var expectedNote = new PhysicianNote
        {
            OrderingPhysician = "Dr. Cameron",
            AHI = "20"
        };

        _physicianNoteValidator.When(v => v.Validate(Arg.Any<PhysicianNote>()))
            .Do(x => { });
        
        var result = _service.ParseNote(noteContent);

        Assert.Equal(expectedNote.AHI, result.AHI);
        Assert.Equal(expectedNote.OrderingPhysician, result.OrderingPhysician);
    }
    
}