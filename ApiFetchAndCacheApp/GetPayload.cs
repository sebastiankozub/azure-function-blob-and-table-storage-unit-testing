using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Web.Http.Results;
using Azure;
using Azure.Core;
using Azure.Storage.Blobs;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NotFoundResult = Microsoft.AspNetCore.Mvc.NotFoundResult;

namespace ApiFetchAndCacheApp
{
    public class GetPayload
    {
        private readonly ILogger _logger;
        private const string _connection = "AzureWebJobsStorage";
        private const string _blobsContainer = @"my-blobs";

        public GetPayload(ILoggerFactory loggerFactory, IAzureClientFactory<BlobServiceClient> blobClientFactory)
        {
            _logger = loggerFactory.CreateLogger<GetLog>();

            _blobContainerClient = blobClientFactory.CreateClient("ApiFetchAndCache").GetBlobContainerClient("my-blobs");
            _blobContainerClient.CreateIfNotExists();
        }

        private readonly BlobContainerClient _blobContainerClient;

        [Function("GetPayload")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "payload/{id}")] HttpRequestData req, string id)
                //[BlobInput("my-blobs" + "/{id}.json", Connection = "AzureWebJobsStorage")] string json, string id)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                var path = @$"3434343344343.json";
                var path2 = @$"00638451090000732413.json";

                var blobClient = _blobContainerClient.GetBlobClient(path);
                var blobResponse = await blobClient.DownloadContentAsync();

                var blobResponseJson = blobResponse.Value.Content.ToString();

                var response = req.CreateResponse(HttpStatusCode.OK);
                response.Headers.Add("Content-Type", "application/json; charset=utf-8");
                response.WriteString(blobResponseJson);
                return response;
            }
            catch (RequestFailedException ex)
            {
                if (ex.Status == 404)
                {
                    
                    _logger.LogInformation($"Item {id} not found");
                    return req.CreateResponse(HttpStatusCode.NotFound);
                    
                }
                else 
                {
                    _logger.LogInformation($"Problem communicating underlying system");
                    return req.CreateResponse(HttpStatusCode.InternalServerError);
                }
            }
            catch (Exception)
            {
                _logger.LogInformation($"Problem communicating underlying system");
                return req.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }
    }
}
