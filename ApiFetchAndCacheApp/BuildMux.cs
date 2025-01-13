using System;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace ApiFetchAndCacheApp
{
    public class BuildMux
    {
        private readonly ILogger<BuildMux> _logger;

        public BuildMux(ILogger<BuildMux> logger)
        {
            _logger = logger;
        }

        [Function(nameof(BuildMux))]
        public void Run([QueueTrigger("ut-download-finished", Connection = "AzureWebJobsStorage")] QueueMessage msg)
        {
            _logger.LogInformation($"C# Queue trigger function processed: {msg.MessageText}");
        }
    }



}
