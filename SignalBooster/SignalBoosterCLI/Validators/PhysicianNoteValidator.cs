using SignalBoosterCLI.Models;

namespace SignalBoosterCLI.Validators;

public class PhysicianNoteValidator : IPhysicianNoteValidator
{
    public void Validate(PhysicianNote note)
    {
        // note cannot be null
        if (note is null)
        {
            throw new ArgumentNullException(nameof(note));
        }
        
        // DOB must be a valid date and not greater than today
        if (string.IsNullOrEmpty(note.DOB) ||
            !DateTime.TryParse(note.DOB, out DateTime dob) ||
            dob > DateTime.Today)
        {
            throw new ArgumentException("Invalid DOB. DOB cannot be null, empty, or a future date.");
        }

        // Diagnosis must not be null or empty
        if (string.IsNullOrEmpty(note.Diagnosis))
        {
            throw new ArgumentException("Diagnosis cannot be null or empty.");
        }
        
    }
}