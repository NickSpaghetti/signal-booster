namespace SignalBoosterCLI.Services;

using System.Text.Json.Nodes;

public interface INoteProcessingService
{
    JsonObject ExtractOrder(string note);

}