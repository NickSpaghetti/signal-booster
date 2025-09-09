namespace SignalBoosterCLI.Brokers;
using Exceptions;
public class FileBroker : IFileBroker
{
    public string ReadNote(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                return "Patient needs a CPAP with full face mask and humidifier. AHI > 20. Ordered by Dr. Cameron.";
            }

            return File.ReadAllText(filePath);
        }
        catch (Exception exception)
        {
            throw new FileValidationException($"{filePath} had Validation errors see inner exception.",exception);
        }
    }

}