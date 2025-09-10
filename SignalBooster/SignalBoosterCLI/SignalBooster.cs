namespace SignalBoosterCLI;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SignalBoosterCLI.Models;
using SignalBoosterCLI.Services.Orchestrations;

public class SignalBooster(
    IOrderOrchestrationService orderOrchestrationService,
    ILogger<SignalBooster> logger,
    IHostApplicationLifetime lifetime)
{
    public async Task RunAsync(string[] args)
    {
        var cliArgs = ParseArgs(args);
        
        if (!string.IsNullOrEmpty(cliArgs.FilePathFromArg) && !string.IsNullOrEmpty(cliArgs.InputStringFromArg))
        {
            logger.LogError("You cannot specify both --input and --file. Use --help for usage information.");
            return;
        }

        Order? order = null;
        if (cliArgs.FilePathFromArg != null)
        {
            order = orderOrchestrationService.CreateOrderFromPhysicianNoteFile(cliArgs.FilePathFromArg);
        }
        else if (cliArgs.InputStringFromArg != null)
        {
            order = orderOrchestrationService.CreateOrderFromPhysicianNote(cliArgs.InputStringFromArg);
        }
        else
        {
            logger.LogDebug("We Should never get to this condition");
            return;
        }

        if (order == null)
        {
            logger.LogError("Order could not be created.");
            return;
        }

        await orderOrchestrationService.SendOrderToVendorAsync(order);
       
        lifetime.StopApplication();
    }

    private ClIArgs ParseArgs(string[] args)
    {
        string? filePathFromArg = null;
        string? inputStringFromArg = null;

        if (args.Length == 0)
        {
            logger.LogInformation("Please provide either a file path (--file) or an input string (--input).");
            logger.LogInformation($"Usage: {nameof(SignalBoosterCLI)} --file <path> | --input <string>");
            logger.LogInformation("  --file, -f    The path to the input file.");
            logger.LogInformation("  --input, -i   A string to be used as input.");
            logger.LogInformation("  --help, -h    Display this help message.");
            Environment.Exit(0);
        }

        for (var i = 0; i < args.Length; i++)
        {
            switch (args[i].ToLower())
            {
                case "--file":
                case "-f":
                    if (i + 1 < args.Length)
                    {
                        filePathFromArg = args[++i];
                    }
                    else
                    {
                        logger.LogError("The --file or -f option requires a file path argument.");
                        Environment.Exit(1);
                    }
                    break;
                case "--input":
                case "-i":
                    if (i + 1 < args.Length)
                    {
                        inputStringFromArg = args[++i];
                    }
                    else
                    {
                        logger.LogError("The --input or -i option requires an input string.");
                        Environment.Exit(1);
                    }
                    break;
                case "--help":
                case "-h":
                    logger.LogInformation($"Usage: {nameof(SignalBoosterCLI)} --file <path> | --input <string>");
                    logger.LogInformation("  --file, -f    The path to the input file.");
                    logger.LogInformation("  --input, -i   A string to be used as input.");
                    Environment.Exit(0);
                    break;
                default:
                    logger.LogError("Unknown argument: {Argument}. Use --help for usage information.", args[i]);
                    break;
            }
        }
        return new ClIArgs(filePathFromArg, inputStringFromArg);
    }
}