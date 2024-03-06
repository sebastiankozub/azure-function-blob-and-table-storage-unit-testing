using System.Net;
using ApiFetchAndCacheApp.Options;
using Azure;
using Azure.Core;
using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Logging;

namespace ApiFetchAndCacheApp
{
    public class GetPayload
    {
        private readonly ILogger _logger;

        private const string _connection = "AzureWebJobsStorage";

        private readonly PayloadStorageOptions _payloadStorageOptions;
        private readonly PublicApiOptions _publicApiOptions;

        public GetPayload(ILoggerFactory loggerFactory, 
            IAzureClientFactory<BlobServiceClient> blobClientFactory, 
            PayloadStorageOptions payloadStorageOptions, 
            PublicApiOptions publicApiOptions)
        {
            _logger = loggerFactory.CreateLogger<GetPayload>();

            _publicApiOptions = publicApiOptions;
            _payloadStorageOptions = payloadStorageOptions;
            _blobContainerClient = blobClientFactory.CreateClient("ApiFetchAndCache").GetBlobContainerClient(_payloadStorageOptions.Container);
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
                var path = $"{id}.json";

                var blobClient = _blobContainerClient.GetBlobClient(path);
                var blobResponse = await blobClient.DownloadContentAsync();

                var blobResponseBinaryData = blobResponse.Value.Content; //.ToString();

                var response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteAsJsonAsync(new MemoryStream());
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
