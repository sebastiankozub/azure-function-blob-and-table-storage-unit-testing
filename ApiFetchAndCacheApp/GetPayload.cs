using System.Net;
using System.Text;
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

        //private const string _connection = "AzureWebJobsStorage";

        //private readonly PayloadStorageOptions _payloadStorageOptions;
        //private readonly PublicApiOptions _publicApiOptions;

        private readonly IBlobRepository _blobRepository;
        
        public GetPayload(ILoggerFactory loggerFactory,
            //IAzureClientFactory<BlobServiceClient> blobClientFactory, 
            //PayloadStorageOptions payloadStorageOptions, 
            //PublicApiOptions publicApiOptions,
            IBlobRepository blobRepository)
        {
            _logger = loggerFactory.CreateLogger<GetPayload>();

            //_publicApiOptions = publicApiOptions;
            //_payloadStorageOptions = payloadStorageOptions;
            //_blobContainerClient = blobClientFactory.CreateClient("ApiFetchAndCache").GetBlobContainerClient(_payloadStorageOptions.Container);
            //_blobContainerClient.CreateIfNotExists();

            _blobRepository = blobRepository;
        }

        //private readonly BlobContainerClient _blobContainerClient;

        [Function("GetPayload")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "payload/{payloadId}")] HttpRequestData req, string payloadId)
        //[BlobInput("my-blobs" + "/{payloadId}.json", Connection = "AzureWebJobsStorage")] string json, string id)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                var path = $"{payloadId}.json";

                var blobContent = await _blobRepository.GetAsync(path); 

                var response = req.CreateResponse(HttpStatusCode.OK);
                response.Headers.Add("Content-Type", "application/json");
                await response.WriteStringAsync(blobContent.ToString(), Encoding.UTF8);

                return response;
            }
            catch (RequestFailedException ex)
            {
                if (ex.Status == 404)
                {                    
                    _logger.LogInformation($"Item {payloadId} not found");
                    return req.CreateResponse(HttpStatusCode.NotFound);                    
                }
                else 
                {
                    _logger.LogInformation($"Problem communicating underlying system");
                    return req.CreateResponse(HttpStatusCode.InternalServerError);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Problem undefined: {ex.Message}");
                return req.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }
    }
}
