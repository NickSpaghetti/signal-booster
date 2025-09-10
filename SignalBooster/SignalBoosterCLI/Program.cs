using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SignalBoosterCLI;
using SignalBoosterCLI.Brokers;
using SignalBoosterCLI.Services.Foundation;
using SignalBoosterCLI.Services.Orchestrations;
using SignalBoosterCLI.Validators;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddLogging(configure => configure.AddConsole());
        
        services.TryAddTransient<IFileBroker, FileBroker>();
        services.TryAddTransient<ILocalFileValidtor,LocalFileValidtor>();
        services.TryAddTransient<ILocalFileService, LocalFileService>();
        services.TryAddSingleton<IPhysicianNoteValidator,PhysicianNoteValidator>();
        services.TryAddTransient<IPhysicianNoteParsingService, PhysicianNoteParsingService>();
        services.TryAddSingleton<IOrderValidator,OrderValidator>();
        services.TryAddTransient<IOrderCreationService, OrderCreationService>();
        services.TryAddTransient<IOrderOrchestrationService, OrderOrchestrationService>();

        // Register the main application service to be run
        services.AddTransient<SignalBooster>();
    })
    .Build();

var cli = host.Services.GetService<SignalBooster>();

await cli.RunAsync(args);


