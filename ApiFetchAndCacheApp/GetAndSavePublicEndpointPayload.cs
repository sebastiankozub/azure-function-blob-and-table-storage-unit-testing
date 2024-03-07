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
using ApiFetchAndCacheApp.Options;
using Azure.Storage.Blobs.Models;
using ApiFetchAndCacheApp.Model;


namespace ApiFetchAndCacheApp
{
    public class GetAndSavePublicEndpointPayload
    {        
        private const string _connection = "AzureWebJobsStorage";

        private readonly PublicApiOptions _publicApiOptions;
        private readonly PayloadStorageOptions _payloadStorageOptions;
        private readonly LogStorageOptions _logStorageOptions;

        private readonly BlobContainerClient _blobContainerClient;
        private readonly HttpClient _client;

        private readonly ILogger _logger;

        public GetAndSavePublicEndpointPayload(ILoggerFactory loggerFactory, 
            IHttpClientFactory _httpFactory, 
            IAzureClientFactory<BlobServiceClient> blobClientFactory, 
            PublicApiOptions publicApiOptions, 
            PayloadStorageOptions payloadStorageOptions,
            LogStorageOptions logStorageOptions)
        {
            _publicApiOptions = publicApiOptions;
            _payloadStorageOptions = payloadStorageOptions;
            _logStorageOptions = logStorageOptions;

            _client = _httpFactory.CreateClient();
            _blobContainerClient = blobClientFactory.CreateClient("ApiFetchAndCache").GetBlobContainerClient(_payloadStorageOptions.Container);
            _blobContainerClient.CreateIfNotExists();

            _logger = loggerFactory.CreateLogger<GetAndSavePublicEndpointPayload>();
        }

        [Function("GetAndSavePublicEndpointPayload")]
        [TableOutput("responseTable2", Connection = _connection)] // use injected service client similar to blob to get configuration from env/json
        public async Task<ApiResponse> Run([TimerTrigger("*/20 * * * * *")] MyInfo myTimer)
        {
            _logger.LogInformation($"GetAndSaveFunction starts processing... : {DateTime.Now}");

            string logMessage;

            var rowKey = DateTime.UtcNow.Ticks.ToString("d20");

            try
            {
                var response = await _client.GetAsync(_publicApiOptions.RandomGetUri);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    if (await SaveToBlob(rowKey, await response.Content.ReadAsStreamAsync()))
                    {
                        logMessage = $"Successfully downloaded and saved payload : {DateTime.Now}";
                        _logger.LogInformation(logMessage);
                        return new ApiResponse { Log = logMessage, RowKey = rowKey, Success = true, PartitionKey = _logStorageOptions.PartitionKey };
                    }
                    else
                    {
                        logMessage = $"Successfully downloaded but NOT saved payload : {DateTime.Now}";
                        _logger.LogError(logMessage);
                        return new ApiResponse { Log = logMessage, RowKey = rowKey, Success = false, PartitionKey = _logStorageOptions.PartitionKey };
                    }
                }
                else
                {
                    logMessage = $"Public API responded with {((int)response.StatusCode)} : {DateTime.Now}";
                    _logger.LogError(logMessage);

                    return new ApiResponse { Log = logMessage, RowKey = rowKey, Success = false, PartitionKey = _logStorageOptions.PartitionKey };
                }
            }
            catch (Exception ex)
            {
                logMessage = $@"Unidetified exception during execution. Probably external API is out of service : {DateTime.Now} 
                    Exception:
                    {ex.Message}";
                _logger.LogError(logMessage);

                return new ApiResponse { Log = logMessage, RowKey = rowKey, Success = false, PartitionKey = _logStorageOptions.PartitionKey };
            }
        }

        private async Task<bool> SaveToBlob(string rowKey, Stream blobValue)
        {
            var path = @$"{rowKey}.json";

            try
            {
                BlobClient blobClient = _blobContainerClient.GetBlobClient(path);
                var response =  await blobClient.UploadAsync(blobValue);

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
