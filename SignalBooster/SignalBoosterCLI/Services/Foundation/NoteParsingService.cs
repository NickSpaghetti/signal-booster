namespace SignalBoosterCLI.Services.Foundation;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Models;
using SignalBoosterCLI.Validators;

public class NoteParsingService(PhysicianNoteValidator physicianNoteValidator)
{
    private readonly PhysicianNoteValidator _physicianNoteValidator = physicianNoteValidator;

    public PhysicianNote ParseNote(string fileContent)
    {
        // Trim and check for JSON structure
        PhysicianNote physicianNote;
        var trimmedContent = fileContent.Trim();
        if (trimmedContent.StartsWith("{"))
        {
            // If the content is JSON, parse it as a JSON file.
            physicianNote = ParseJson(trimmedContent);
        }
        else
        {
            // Otherwise, assume it is a text file and parse accordingly.
            physicianNote = ParseText(trimmedContent);
        }
        physicianNoteValidator.Validate(physicianNote);
        return physicianNote;
    }
    
    private PhysicianNote ParseText(string fileContent)
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
            else if (cleanedLine.StartsWith("Prescription:"))
            {
                note.Prescription = cleanedLine.Replace("Prescription:", "").Trim();
            }
            else if (cleanedLine.StartsWith("Usage:"))
            {
                note.Usage = cleanedLine.Replace("Usage:", "").Trim();
            }
            else if (cleanedLine.StartsWith("Ordering Physician:"))
            {
                note.OrderingPhysician = cleanedLine.Replace("Ordering Physician:", "").Trim();
            }
        }
        
        return note;
    }
    
    private PhysicianNote ParseJson(string jsonContent)
    {
        // Check if the JSON has a "data" field.
        if (jsonContent.Contains("\"data\":"))
        {
            var dataWrapper = JsonSerializer.Deserialize<JsonDataWrapper>(jsonContent);
            return ParseText(dataWrapper.Data);
        }
        
        return JsonSerializer.Deserialize<PhysicianNote>(jsonContent);
    }
    
    private class JsonDataWrapper
    {
        [JsonPropertyName("data")]
        public string Data { get; set; }
    }
}
