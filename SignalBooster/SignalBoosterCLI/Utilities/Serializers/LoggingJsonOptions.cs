namespace SignalBoosterCLI.Utilities.Serializers;

using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// Provides a pre-configured JsonSerializerOptions instance for logging purposes.
/// This ensures a consistent and readable JSON format in logs.
/// </summary>
public static class LoggingJsonOptions
{

    public static JsonSerializerOptions Options { get; } = new()
    {
        // Makes the JSON output human-readable by adding indentation.
        WriteIndented = true,

        // Ignores properties with null values to reduce log clutter.
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,

        //Ignores Circular references
        ReferenceHandler = ReferenceHandler.IgnoreCycles
    };
}