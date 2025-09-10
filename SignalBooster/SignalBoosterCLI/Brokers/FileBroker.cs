namespace SignalBoosterCLI.Brokers;

// Brokers are meant to be the lowest level of abstraction there should not be any control flow those are designated for Services.
// A broker should not have a dependency on any Services.
public class FileBroker : IFileBroker
{
    public string ReadNote(string filePath)
    {
        return File.ReadAllText(filePath);
    }

}