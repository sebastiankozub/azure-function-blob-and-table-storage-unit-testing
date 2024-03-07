using System.Net;
using System.Text.Json;
using ApiFetchAndCacheApp.Model;
using ApiFetchAndCacheApp.Options;
using Azure.Data.Tables;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace ApiFetchAndCacheApp
{
    public class GetLog
    {
        private readonly PublicApiOptions _publicApiOptions;
        private readonly PayloadStorageOptions _payloadStorageOptions;

        private readonly ILogger _logger;

        private const string _connection = "AzureWebJobsStorage";
        
        public GetLog(ILoggerFactory loggerFactory, PublicApiOptions publicApiOptions, PayloadStorageOptions payloadStorageOptions)
        {
            _logger = loggerFactory.CreateLogger<GetLog>();

            _publicApiOptions = publicApiOptions;
            _payloadStorageOptions = payloadStorageOptions;
        }

        [Function("GetLog")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "log/{from:DateTime?}/{to:DateTime?}")] HttpRequestData req,
            DateTime? from,
            DateTime? to,
            [TableInput(tableName: "responseTable2", partitionKey: "publicApiResponse", Connection = _connection)] TableClient tableInputs, // use injected service client similar to blob to get configuration from env/json
            FunctionContext context)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            
            long? ticksFrom = from?.Ticks;
            long? ticksTo = to?.Ticks;

            string ticksFromAsString = "";
            string ticksToAsString = "";

            if (ticksTo != null)
            {
                ticksToAsString = ticksTo.Value.ToString("d20");
            }
            else
            {
                ticksToAsString = DateTime.UtcNow.Ticks.ToString("d20");
            }

            //ticksFromString = ticksFrom == null ? "" : ticksFrom.ToString().Replace(".", "").Replace(",", "");

            if (ticksFrom != null )
            {
                ticksFromAsString = ticksFrom.Value.ToString("d20");
            }
            else
            {
                ticksFromAsString = ((long)0).ToString("d20");
            }

            var results = await tableInputs.QueryAsync<ApiResponse>(filter: $"RowKey ge {ticksFromAsString} and RowKey le {ticksToAsString}").OrderByDescending(x => x.RowKey).ToListAsync();
                        
            var json = JsonSerializer.Serialize(results);

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json; charset=utf-8");
            response.WriteString(json);

            return response;
        }
    }
}
