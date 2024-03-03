using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ApiFetchAndCacheApp
{
    public class GetPayload
    {
        private readonly ILogger _logger;

        public GetPayload(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<GetLog>();
        }

        [Function("GetPayload")]
        public HttpResponseMessage Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "payload/{id}")] HttpRequestData req,
                [BlobInput("my-blobs" + "/{id}.json", Connection = "AzureWebJobsStorage")] string json, string id)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            if (json == null)
            {
                _logger.LogInformation($"Item {id} not found");
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }
    }
}
