using System;
using System.Net;
using Azure;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Azure.Data.Tables;
using Microsoft.Azure.Functions.Worker.Extensions;
using Microsoft.Azure.Functions;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Azure;


namespace ApiFetchAndCacheApp
{
    public class GetAndSavePublicEndpointPayload
    {
        private readonly ILogger _logger;
        private const string _connection = "AzureWebJobsStorage";
        private const string _publicEndpointUrl = @"https://api.publicapis.org/random?auth=null";
        private const string _blobsContainer = @"my-blobs";
        private readonly HttpClient _client;

        private readonly BlobContainerClient _blobContainerClient;

        public GetAndSavePublicEndpointPayload(ILoggerFactory loggerFactory, IHttpClientFactory _httpFactory, IAzureClientFactory<BlobServiceClient> blobClientFactory)
        {
            _logger = loggerFactory.CreateLogger<GetAndSavePublicEndpointPayload>();
            _client = _httpFactory.CreateClient();
            _blobContainerClient = blobClientFactory.CreateClient("ApiFetchAndCache").GetBlobContainerClient("my-blobs");
            _blobContainerClient.CreateIfNotExists();
        }

        [Function("GetAndSavePublicEndpointPayload")]
        [TableOutput("responseTable2", Connection = _connection)]
        public async Task<ApiResponse> Run([TimerTrigger("*/20 * * * * *")] MyInfo myTimer)
        {
            _logger.LogInformation($"GetAndSaveFunction starts processing... : {DateTime.Now}");

            string logMessage;

            var rowKey = DateTime.UtcNow.Ticks.ToString("d20");

            try
            {
                var response = await _client.GetAsync(_publicEndpointUrl);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    if (await SaveToBlob(rowKey, await response.Content.ReadAsStreamAsync()))
                    {
                        logMessage = $"Successfully downloaded and saved payload : {DateTime.Now}";
                        _logger.LogInformation(logMessage);
                        return new ApiResponse { Log = logMessage, RowKey = rowKey, Success = true };
                    }
                    else
                    {
                        logMessage = $"Successfully downloaded but NOT saved payload : {DateTime.Now}";
                        _logger.LogError(logMessage);
                        return new ApiResponse { Log = logMessage, RowKey = rowKey, Success = false };
                    }
                }
                else
                {
                    logMessage = $"Public API responded with {((int)response.StatusCode)} : {DateTime.Now}";
                    _logger.LogError(logMessage);

                    return new ApiResponse { Log = logMessage, RowKey = rowKey, Success = false };
                }
            }
            catch (Exception ex)
            {
                logMessage = $@"Unidetified exception during execution. Probably external API is out of service : {DateTime.Now} 
                    Exception:
                    {ex.Message}";
                _logger.LogError(logMessage);

                return new ApiResponse { Log = logMessage, RowKey = rowKey, Success = false };
            }
        }

        private async Task<bool> SaveToBlob(string rowKey, Stream blobValue)
        {
            var path = @$"{rowKey}.json";

            try
            {
                BlobClient blobClient = _blobContainerClient.GetBlobClient(path);
                await blobClient.UploadAsync(blobValue);

                //blobClient = _blobContainerClient.GetBlobClient(path + "2"); //
                //var r = await blobClient.DownloadContentAsync();
                //var blobJson = r.Value.Content.ToString();
                return true;
            }
            catch { return false; }
        }
    }


    public class MyInfo
    {
        public MyScheduleStatus? ScheduleStatus { get; set; }

        public bool IsPastDue { get; set; }
    }

    public class MyScheduleStatus
    {
        public DateTime Last { get; set; }

        public DateTime Next { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}
