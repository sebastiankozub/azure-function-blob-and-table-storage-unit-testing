using System.Net;
using System.Text;
using Azure;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace ApiFetchAndCacheApp
{
    public class GetPayload
    {
        private readonly ILogger _logger;
        private readonly IBlobRepository _blobRepository;
        
        public GetPayload(ILoggerFactory loggerFactory, IBlobRepository blobRepository)
        {
            _logger = loggerFactory.CreateLogger<GetPayload>();
            _blobRepository = blobRepository;
        }

        [Function("GetPayload")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "payload/{payloadId}")] HttpRequestData req, string payloadId)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                var path = $"{payloadId}.json";

                var blobContent = await _blobRepository.GetAsync(path);

                var response = req.CreateResponse(HttpStatusCode.OK);
                response.Headers.Add("Content-Type", "application/json; charset=utf-8");
                response.Body = blobContent;
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
