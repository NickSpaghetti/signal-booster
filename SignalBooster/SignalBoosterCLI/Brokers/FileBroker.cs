namespace SignalBoosterCLI.Brokers;

public class FileBroker : IFileBroker
{
    public string ReadNote(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return "Patient needs a CPAP with full face mask and humidifier. AHI > 20. Ordered by Dr. Cameron.";
        }

        return File.ReadAllText(filePath);
    }

}