using HostEngine.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureServices(services=>
    {
        services.AddSingleton<IEngine, Engine>();
        services.AddOptions<EngineConfig>().Configure<IConfiguration>((settings, configuration) =>
        {
            configuration.GetSection("EngineConfig").Bind(settings);
        });
    })
    .ConfigureFunctionsWorkerDefaults()
    .Build();

host.Run();
