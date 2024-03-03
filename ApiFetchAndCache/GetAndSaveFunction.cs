using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Azure;
using Azure.Data.Tables;

namespace ApiFetchAndCache
{
    public class GetAndSaveFunction
    {
        private const string _connection = "AzureWebJobsStorage";
        private const string _publicEndpointUrl = @"https://api.publicapis.org/random?auth=null";
        private readonly HttpClient _client;

        public GetAndSaveFunction(IHttpClientFactory httpClientFactory)
        {
            _client = httpClientFactory.CreateClient();
        }

        [FunctionName("GetAndSaveFunction")]
        [StorageAccount(_connection)]
        [return: Table("responseTable2")]
        public async Task<ApiResponse> RunAsync(
            [TimerTrigger("0 * * * * *")]TimerInfo myTimer,
            Binder binder,
            ILogger log)
        {
            log.LogInformation($"GetAndSaveFunction starts processing... : {DateTime.Now}");
            string logMessage;

            var newGuid = Guid.NewGuid().ToString().Replace(@"-", String.Empty);

            try
            {
                var response = await _client.GetAsync(_publicEndpointUrl);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    if (await SaveToBlob(newGuid, await response.Content.ReadAsStringAsync(), binder))
                    {
                        logMessage = $"Successfully downloaded and saved payload : {DateTime.Now}";
                        log.LogInformation(logMessage);
                        return new ApiResponse { Log = logMessage, RowKey = newGuid, Success = true };
                    }
                    else
                    {
                        logMessage = $"Successfully downloaded but NOT saved payload : {DateTime.Now}";
                        log.LogError(logMessage);
                        return new ApiResponse { Log = logMessage, RowKey = newGuid, Success = false };
                    }
                }
                else
                {
                    logMessage = $"Public API responded with {((int)response.StatusCode)} : {DateTime.Now}";
                    log.LogError(logMessage);

                    return new ApiResponse { Log = logMessage, RowKey = newGuid, Success = false };
                }
            }
            catch (Exception ex)
            {
                logMessage = $@"GetAndSaveFunction exceptioned during execution : {DateTime.Now} 
                    Exception:
                    {ex.Message}";

                log.LogError(logMessage);

                return new ApiResponse { Log = logMessage, RowKey = newGuid, Success = false };
            }
        }

        private async static Task<bool> SaveToBlob(string guid, string blobValue, Binder binder)
        {
            var path = @$"my-blobs/{guid}.json";

            var attributes = new Attribute[] {
                new BlobAttribute(path, FileAccess.Write)
            };

            try
            {
                using var writer = await binder.BindAsync<TextWriter>(new BlobAttribute(path, FileAccess.Write));

                await writer.WriteAsync(blobValue);
                await writer.FlushAsync();
            }
            catch { return false; }

            return true;
        }
    }

    public class ApiResponse : ITableEntity
    {
        public ApiResponse()
        {

            PartitionKey = "publicApiResponse";
        }

        public string Log { get; set; }

        public bool Success { get; set; }

        public string PartitionKey { get; set; }
        public string RowKey { get; set; }

        public ETag ETag { get; set; } = default!;
        public DateTimeOffset? Timestamp { get; set; } = default!;

    }
}


