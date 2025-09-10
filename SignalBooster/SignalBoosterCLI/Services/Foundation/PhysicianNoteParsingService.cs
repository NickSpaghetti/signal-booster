namespace SignalBoosterCLI.Services.Foundation;

using System.Text.Json;
using System.Text.Json.Serialization;
using Models;
using SignalBoosterCLI.Validators;
using Microsoft.Extensions.Logging;
using SignalBoosterCLI.Utilities.Serializers;
using System.Text.RegularExpressions;

public class PhysicianNoteParsingService(PhysicianNoteValidator physicianNoteValidator, ILogger<PhysicianNoteParsingService> logger) : IPhysicianNoteParsingService
{
    private readonly PhysicianNoteValidator _physicianNoteValidator = physicianNoteValidator;

    public PhysicianNote ParseNote(string noteContent)
    {
        // Trim and check for JSON structure
        PhysicianNote physicianNote;
        var trimmedContent = noteContent.Trim();
        if (IsValidJson(trimmedContent))
        {
            // If the content is JSON, parse it as a JSON file.
            physicianNote = ParseJson(trimmedContent);
        }
        else if (trimmedContent.Contains(Environment.NewLine))
        {
            logger.LogInformation("parsing as CRLF");
            // Otherwise, assume it is a text file and parse accordingly.
            physicianNote = ParseCrLfText(trimmedContent);
        }
        else
        {
            logger.LogInformation("parsing with regex");
            // Try old logic
            physicianNote = ParseText(trimmedContent);
        }

        logger.LogInformation(
            $"Created Physician Note {JsonSerializer.Serialize(physicianNote, LoggingJsonOptions.Options)}");
        
        physicianNoteValidator.Validate(physicianNote);
        return physicianNote;
    }

    private bool IsValidJson(string json)
    {
        try
        {
            JsonDocument.Parse(json);
            logger.LogInformation("Note is valid json");
            return true;
        }
        catch (JsonException)
        {
            logger.LogInformation("Note is not valid json");
            return false;
        }
    }
    
    private PhysicianNote ParseCrLfText(string fileContent)
    {
        var note = new PhysicianNote();
        var lines = fileContent.Split('\n');

        foreach (var line in lines)
        {
            var cleanedLine = line.Trim().Replace("\r", "");
            if (cleanedLine.StartsWith("Patient Name:"))
            {
                note.PatientName = cleanedLine.Replace("Patient Name:", "").Trim();
            }
            else if (cleanedLine.StartsWith("DOB:"))
            {
                note.DOB = cleanedLine.Replace("DOB:", "").Trim();
            }
            else if (cleanedLine.StartsWith("Diagnosis:"))
            {
                note.Diagnosis = cleanedLine.Replace("Diagnosis:", "").Trim();
            }
            else if (cleanedLine.StartsWith("Recommendation:"))
            {
                note.Recommendation = cleanedLine.Replace("Recommendation:", "").Trim();
            }
            else if (cleanedLine.StartsWith("Prescription:"))
            {
                note.Prescription = cleanedLine.Replace("Prescription:", "").Trim();
            }
            else if (cleanedLine.StartsWith("Usage:"))
            {
                note.Usage = cleanedLine.Replace("Usage:", "").Trim();
            }
            else if (cleanedLine.StartsWith("AHI:"))
            {
                note.AHI = cleanedLine.Replace("AHI:", "").Trim();
            }
            else if (cleanedLine.StartsWith("Ordering Physician:"))
            {
                note.OrderingPhysician = cleanedLine.Replace("Ordering Physician:", "").Trim();
            }
        }
        
        return note;
    }

    /// <summary>
    /// This is for backwards compatability with the default hard coded value.
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    public PhysicianNote ParseText(string content)
    {
        var note = new PhysicianNote();
        
        // Pattern for the detailed note with equipment, AHI, and physician.
        string detailedPattern = @"AHI > (?<ahi>.*?)\. Ordered by (?<physician>Dr\.\s+[A-Za-z\s]+?)\.";

        // Attempt to match the detailed pattern first.
        Match detailedMatch = Regex.Match(content, detailedPattern);
        if (detailedMatch.Success)
        {
            note.AHI = detailedMatch.Groups["ahi"].Value;
            note.OrderingPhysician = detailedMatch.Groups["physician"].Value;
        }
        
        // Pattern for the full diagnostic sentence.
        // It matches a sentence starting with either "Patient needs a" or "Requires a"
        // and captures the whole thing.
        string diagnosticPattern = @"(?<diagnosis>(Patient needs a|Requires a).*?\.)";

        // Now, attempt to match the diagnostic pattern.
        Match diagnosticMatch = Regex.Match(content, diagnosticPattern);
        if (diagnosticMatch.Success)
        {
            // If the pattern is found, set the Diagnostics property.
            note.Diagnosis = diagnosticMatch.Groups["diagnosis"].Value;
        }

        return note;
    }
    
    private PhysicianNote? ParseJson(string jsonContent)
    {
        if (jsonContent.Contains("\"data\":"))
        {
            var dataWrapper = JsonSerializer.Deserialize<JsonDataWrapper>(jsonContent);
            if (dataWrapper.Data.Contains(Environment.NewLine) ||  dataWrapper.Data.Contains('\n'))
            {
                logger.LogInformation("Found CrLF text inside of data...parsing as CRLF");
                return ParseCrLfText(dataWrapper.Data);
            }
            else
            {
                logger.LogInformation("Didn't find CrLF text inside of data...parsing as legacy with regex");
                return ParseText(dataWrapper.Data);
            }
            
        }
        
        return JsonSerializer.Deserialize<PhysicianNote>(jsonContent);
    }
    
    private class JsonDataWrapper
    {
        [JsonPropertyName("data")]
        public string Data { get; set; }
    }
}
