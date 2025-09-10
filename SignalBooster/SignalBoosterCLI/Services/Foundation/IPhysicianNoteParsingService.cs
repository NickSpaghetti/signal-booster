using SignalBoosterCLI.Models;

namespace SignalBoosterCLI.Services.Foundation;

public interface IPhysicianNoteParsingService
{
    PhysicianNote ParseNote(string noteContent);
}