using SignalBoosterCLI.Models;

namespace SignalBoosterCLI.Validators;

public interface IPhysicianNoteValidator
{
    void Validate(PhysicianNote note);
}