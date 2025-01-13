using System;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace UtubeFunctionApp
{
    public class Function6
    {
        private readonly ILogger<Function6> _logger;

        public Function6(ILogger<Function6> logger)
        {
            _logger = logger;
        }

        [Function(nameof(Function6))]
        public void Run([QueueTrigger("myqueue-items", Connection = "AzuriteStorageConnection")] QueueMessage message)
        {
            _logger.LogInformation($"C# Queue trigger function processed: {message.MessageText}");
        }
    }
}
