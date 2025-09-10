using System.Text;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using SignalBoosterCLI.Brokers;
using SignalBoosterCLI.Models;
using SignalBoosterCLI.Services.Foundation;
using SignalBoosterCLI.Services.Orchestrations;
using SignalBoosterCLI.Validators;

var services = new ServiceCollection();

services.AddLogging(configure => configure.AddConsole());

services.TryAddTransient<IFileBroker,FileBroker>();
services.TryAddTransient<LocalFileValidtor>();
services.TryAddTransient<ILocalFileService,LocalFileService>();
services.TryAddSingleton<PhysicianNoteValidator>();
services.TryAddTransient<IPhysicianNoteParsingService,PhysicianNoteParsingService>();
services.TryAddSingleton<OrderValidator>();
services.TryAddTransient<IOrderCreationService,OrderCreationService>();
services.TryAddTransient<IOrderOrchestrationService,OrderOrchestrationService>();

await using var serviceProvider = services.BuildServiceProvider();

var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

string? filePathFromArg = null;
string? inputStringFromArg = null;

if (args.Length == 0)
{
    logger.LogInformation("Please provide either a file path (--file) or an input string (--input).");
    logger.LogInformation($"Usage: {nameof(SignalBoosterCLI)} --file <path> | --input <string>");
    logger.LogInformation("  --file, -f    The path to the input file.");
    logger.LogInformation("  --input, -i   A string to be used as input.");
    logger.LogInformation("  --help, -h    Display this help message.");
    return;
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
                return;
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
                return;
            }
            break;
        case "--help":
        case "-h":
            logger.LogInformation($"Usage: {nameof(SignalBoosterCLI)} --file <path> | --input <string>");
            logger.LogInformation("  --file, -f    The path to the input file.");
            logger.LogInformation("  --input, -i   A string to be used as input.");
            return;
        default:
            logger.LogError("Unknown argument: {Argument}. Use --help for usage information.", args[i]);
            return;
    }
}

if (!string.IsNullOrEmpty(filePathFromArg) && !string.IsNullOrEmpty(inputStringFromArg))
{
    logger.LogError("You cannot specify both --input and --file. Use --help for usage information.");
    return;
}

var orderOrchestrationService = serviceProvider.GetRequiredService<IOrderOrchestrationService>();

Order? order = null;
if (filePathFromArg != null)
{
    order = orderOrchestrationService.CreateOrderFromPhysicianNoteFile(filePathFromArg);
}
else if (inputStringFromArg != null)
{
    order = orderOrchestrationService.CreateOrderFromPhysicianNote(inputStringFromArg);
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



// var filePath = @"C:\Users\nc_ci\GitProjects\NickSpaghetti\signal-booster\physician_note1.txt";
//
// var fileContent = string.Empty;
//
// if (!File.Exists(filePath)) 
// {
//     fileContent = "Patient needs a CPAP with full face mask and humidifier. AHI > 20. Ordered by Dr. Cameron.";
// }
// else
// {
//     fileContent = File.ReadAllText(filePath);
// }
//
// Console.WriteLine(fileContent);
//
// var device = string.Empty;
// switch (fileContent)
// {
//     case { } s when s.Contains("CPAP",StringComparison.OrdinalIgnoreCase):
//         Console.WriteLine("Detected CPAP device.");
//         device = "CPAP";
//         break;
//     case { } s when s.Contains("oxygen",StringComparison.OrdinalIgnoreCase):
//         Console.WriteLine("Detected Oxygen Tank.");
//         device = "Oxygen Tank";
//         break;
//     case { } s when s.Contains("wheelchair",StringComparison.OrdinalIgnoreCase):
//         Console.WriteLine("Detected Wheelchair.");
//         device = "Wheelchair";
//         break;
//     default:
//         Console.WriteLine("Unknown device.");
//         device = "Unknown";
//         break;
// }
//
// string? maskType = null;
// if (device == "CPAP" && fileContent.Contains("full face", StringComparison.OrdinalIgnoreCase))
// {
//     maskType = "full face";
// }
//
// string? addOns = null;
// if (fileContent.Contains("humidifier", StringComparison.OrdinalIgnoreCase))
// {
//     addOns = "humidifier";
// }
//
// var qualifier = string.Empty;
// if (fileContent.Contains("AHI > 20", StringComparison.OrdinalIgnoreCase))
// {
//     qualifier = "AHI > 20";
// }
//
// var orderingProvider = string.Empty;
// var doctorIndex = fileContent.IndexOf("Dr.", StringComparison.OrdinalIgnoreCase);
// if (doctorIndex > 0)
// {
//     //remove /r/n
//     orderingProvider = fileContent.Substring(doctorIndex).Replace("Ordered by ", "").Trim('.');
// }
//
// string? liters = null;
// string? usage = null;
// if (device == "Oxygen Tank")
// {
//     Match literMatch = Regex.Match(fileContent, @"(\d+(\.\d+)?) ?L", RegexOptions.IgnoreCase);
//     if (literMatch.Success)
//     {
//         liters = $"{literMatch.Groups[1].Value} L";
//
//         if (fileContent.Contains("sleep", StringComparison.OrdinalIgnoreCase) &&
//             fileContent.Contains("exertion", StringComparison.OrdinalIgnoreCase))
//         {
//             usage = "sleep and exertion";
//         }
//         else if (fileContent.Contains("sleep", StringComparison.OrdinalIgnoreCase))
//         {
//             usage = "sleep";
//         }
//         else if (fileContent.Contains("exertion", StringComparison.OrdinalIgnoreCase))
//         {
//             usage = "exertion";
//         }
//     }
// }
//
// var orderRequest = new JsonObject
// {
//     ["device"] = device,
//     ["mask_type"] = maskType,
//     ["add_ons"] = addOns != null ? new JsonArray(addOns) : null,
//     ["qualifier"] = qualifier,
//     ["ordering_provider"] = orderingProvider,
//     
// };
//
//
// if (device == "Oxygen Tank")
// {
//     orderRequest["liters"] = liters;
//     orderRequest["usage"] = usage;
// }
//
// var jsonString = orderRequest.ToJsonString();
//
// var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
//
//
// using var httpClient = new HttpClient();
// var response = await httpClient.PostAsync("https://alert-api.com/DrExtract", content);
//
// var responseBody = await response.Content.ReadAsStringAsync();
// Console.WriteLine(responseBody);
//
//
//
//
//
//
// Console.WriteLine();
// Console.WriteLine(device);
// Console.WriteLine("Press any key to exit...");
Console.ReadKey();


