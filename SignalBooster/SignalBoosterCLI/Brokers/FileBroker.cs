namespace SignalBoosterCLI.Brokers;
using Exceptions;
using Microsoft.Extensions.Logging;
public class FileBroker : IFileBroker
{
    public string ReadNote(string filePath)
    {
        return File.ReadAllText(filePath);
    }

}