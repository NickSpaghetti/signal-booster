namespace SignalBoosterCLI.Validators;

using Microsoft.Extensions.Logging;

public class LocalFileValidtor(ILogger<LocalFileValidtor> logger) : ILocalFileValidtor
{
    public void Validate(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            logger.LogError("Path is empty");
            throw new ArgumentNullException(nameof(path));
        }
        
        if (!File.Exists(path))
        {
            logger.LogError("File does not exist");
            throw new FileNotFoundException(path);
        }
        
    }
}