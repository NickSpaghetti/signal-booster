using System.Text.Json;
using System.Text.Json.Serialization;
using VendorApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.Map("/api/DrExtract", api =>
{
    api.Run(async context =>
    {
        if (context.Request.Method == "POST")
        {
            try
            {
                var clientOrder = await context.Request.ReadFromJsonAsync<ClientOrder>();
                Console.WriteLine(
                    $"Received POST request to /api/DrExtract with: {JsonSerializer.Serialize(clientOrder, new JsonSerializerOptions { WriteIndented = true })}");
                                    
                var response = new VendorOrderResponse
                {
                    OrderId = Guid.NewGuid()
                };

                context.Response.StatusCode = StatusCodes.Status202Accepted;
                await context.Response.WriteAsJsonAsync(response);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync($"Error processing request: {ex.Message}");
            }
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status405MethodNotAllowed;
        }
    });
});

app.MapGet("/weatherforecast", () =>
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .ToArray();
        return forecast;
    })
    .WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}