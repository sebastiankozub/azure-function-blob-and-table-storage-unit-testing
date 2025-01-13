using ApiFetchAndCacheApp;
using ApiFetchAndCacheApp.Options;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Opt = Microsoft.Extensions.Options;
using Microsoft.Azure.Functions.Worker;


var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    //.ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((host, services) =>
    {
        services.AddHttpClient();

        services.AddAzureClients(clientBuilder =>
        {
            var str1 = host.Configuration.GetSection("PayloadStorage:StorageConnectionString");
            clientBuilder.AddBlobServiceClient(host.Configuration.GetSection("PayloadStorage:StorageConnectionString"))
                .WithName("ApiFetchAndCache");

            var str2 = host.Configuration.GetSection("LogStorage:StorageConnectionString");
            clientBuilder.AddTableServiceClient(host.Configuration.GetSection("LogStorage:StorageConnectionString"));
        });


        //options
        services.AddOptions<PayloadStorageOptions>()
            .Bind(host.Configuration.GetSection(PayloadStorageOptions.Section))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton(resolver =>
            resolver.GetRequiredService<Opt.IOptions<PayloadStorageOptions>>().Value);

        services.AddOptions<PublicApiOptions>()
            .Bind(host.Configuration.GetSection(PublicApiOptions.Section))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton(resolver =>
            resolver.GetRequiredService<Opt.IOptions<PublicApiOptions>>().Value);

        services.AddOptions<LogStorageOptions>()
            .Bind(host.Configuration.GetSection(LogStorageOptions.Section))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton(resolver =>
            resolver.GetRequiredService<Opt.IOptions<LogStorageOptions>>().Value);

        //internal
        services.AddScoped<IBlobRepository, BlobRepository>();

    })
    .Build();

host.Run();