using System.Text;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

var filePath = @"C:\sers\nc_ci\GitProjects\NickSpaghetti\signal-booster\physician_note1.txt";

var fileContent = string.Empty;

if (!File.Exists(filePath)) 
{
    fileContent = "Patient needs a CPAP with full face mask and humidifier. AHI > 20. Ordered by Dr. Cameron.";
}
else
{
    fileContent = File.ReadAllText(filePath);
}

Console.WriteLine(fileContent);

var device = string.Empty;
switch (fileContent)
{
    case { } s when s.Contains("CPAP",StringComparison.OrdinalIgnoreCase):
        Console.WriteLine("Detected CPAP device.");
        device = "CPAP";
        break;
    case { } s when s.Contains("oxygen",StringComparison.OrdinalIgnoreCase):
        Console.WriteLine("Detected Oxygen Tank.");
        device = "Oxygen Tank";
        break;
    case { } s when s.Contains("wheelchair",StringComparison.OrdinalIgnoreCase):
        Console.WriteLine("Detected Wheelchair.");
        device = "Wheelchair";
        break;
    default:
        Console.WriteLine("Unknown device.");
        device = "Unknown";
        break;
}

string? maskType = null;
if (device == "CPAP" && fileContent.Contains("full face", StringComparison.OrdinalIgnoreCase))
{
    maskType = "full face";
}

string? addOns = null;
if (fileContent.Contains("humidifier", StringComparison.OrdinalIgnoreCase))
{
    addOns = "humidifier";
}

var qualifier = string.Empty;
if (fileContent.Contains("AHI > 20", StringComparison.OrdinalIgnoreCase))
{
    qualifier = "AHI > 20";
}

var orderingProvider = string.Empty;
var doctorIndex = fileContent.IndexOf("Dr.", StringComparison.OrdinalIgnoreCase);
if (doctorIndex > 0)
{
    //remove /r/n
    orderingProvider = fileContent.Substring(doctorIndex).Replace("Ordered by ", "").Trim('.');
}

string? liters = null;
string? usage = null;
if (device == "Oxygen Tank")
{
    Match literMatch = Regex.Match(fileContent, @"(\d+(\.\d+)?) ?L", RegexOptions.IgnoreCase);
    if (literMatch.Success)
    {
        liters = $"{literMatch.Groups[1].Value} L";

        if (fileContent.Contains("sleep", StringComparison.OrdinalIgnoreCase) &&
            fileContent.Contains("exertion", StringComparison.OrdinalIgnoreCase))
        {
            usage = "sleep and exertion";
        }
        else if (fileContent.Contains("sleep", StringComparison.OrdinalIgnoreCase))
        {
            usage = "sleep";
        }
        else if (fileContent.Contains("exertion", StringComparison.OrdinalIgnoreCase))
        {
            usage = "exertion";
        }
    }
}

var orderRequest = new JsonObject
{
    ["device"] = device,
    ["mask_type"] = maskType,
    ["add_ons"] = addOns != null ? new JsonArray(addOns) : null,
    ["qualifier"] = qualifier,
    ["ordering_provider"] = orderingProvider,
    
};


if (device == "Oxygen Tank")
{
    orderRequest["liters"] = liters;
    orderRequest["usage"] = usage;
}

var jsonString = orderRequest.ToJsonString();

var content = new StringContent(jsonString, Encoding.UTF8, "application/json");


using var httpClient = new HttpClient();
var response = await httpClient.PostAsync("https://alert-api.com/DrExtract", content);

var responseBody = await response.Content.ReadAsStringAsync();
Console.WriteLine(responseBody);






Console.WriteLine();
Console.WriteLine(device);
Console.WriteLine("Press any key to exit...");
Console.ReadKey();


