using Microsoft.Extensions.Logging;

namespace SignalBoosterCLI.Services.Foundation;

using SignalBoosterCLI.Brokers;
using SignalBoosterCLI.Validators;

public class LocalFileService(IFileBroker fileBroker, ILocalFileValidtor validator) : ILocalFileService
{
    public string ReadFile(string path)
    {
        validator.Validate(path);
        return fileBroker.ReadNote(path);
    }
}