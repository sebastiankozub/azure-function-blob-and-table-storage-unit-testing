using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((host, services) =>
    {
        services.AddHttpClient();

        services.AddAzureClients(clientBuilder =>
        {
            clientBuilder.AddBlobServiceClient(host.Configuration.GetSection("AzureWebJobsStorage"))
                .WithName("ApiFetchAndCache");
        });
    })
    .Build();





host.Run();