using System.Net;
using System.Text.Json;
using Azure.Data.Tables;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace ApiFetchAndCacheApp
{
    public class GetLog
    {
        private readonly ILogger _logger;

        public GetLog(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<GetLog>();
        }

        [Function("GetLog")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "log/{from:DateTime?}/{to:DateTime?}")] HttpRequestData req,
            DateTime? from,
            DateTime? to,
            [TableInput(tableName: "responseTable2", partitionKey: "publicApiResponse", Connection = "AzureWebJobsStorage")] TableClient tableInputs,
            FunctionContext context)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            
            long? ticksFrom = from?.Ticks;
            long? ticksTo = to?.Ticks;

            string ticksFromAsString = "";
            string ticksToAsString = "";

            if (ticksTo != null)
            {
                ticksToAsString = ticksTo.Value.ToString("d20");//.ToString().Replace(".", "").Replace(",", "");
            }
            else
            {
                ticksToAsString = DateTime.UtcNow.Ticks.ToString("d20");//.ToString().Replace(".", "").Replace(",", "");
            }

            //ticksFromString = ticksFrom == null ? "" : ticksFrom.ToString().Replace(".", "").Replace(",", "");

            if (ticksFrom != null )
            {
                ticksFromAsString = ticksFrom.Value.ToString("d20");//.ToString().Replace(".", "").Replace(",", "");
            }
            else
            {
                ticksFromAsString = ((long)0).ToString("d20");//.ToString().Replace(".", "").Replace(",", "");
            }

            var results = await tableInputs.QueryAsync<ApiResponse>(filter: $"RowKey ge {ticksFromAsString} and RowKey le {ticksToAsString}").ToListAsync();
                        
            var json = JsonSerializer.Serialize(results);

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            response.WriteString(json);

            return response;
        }
    }
}
