using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.Extensions.Logging.Abstractions;
using System.Data.Common;
using Azure.Data.Tables;
using Microsoft.Azure.Documents;
using System.Net;

namespace ApiFetchAndCache
{
    public static class GetPayloadFunction
    {
        private const string _connection = "AzureWebJobsStorage";

        [FunctionName("GetPayloadFunction")]
        [StorageAccount(_connection)]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "payload/{from:int?}/{to:int?}")] HttpRequest req,
            [Table("responseTable")] CloudTable cloudTable,
            int? from, int? to,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";


            //TableQuery<ApiResponse> rangeQuery = new TableQuery<ApiResponse>().Where(
            //    TableQuery.CombineFilters(
            //        TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.GreaterThan, "D"),
            //        TableOperators.And,
            //        TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.GreaterThan, "t")));

            //TableOperation retrieveOperation = TableOperation.Retrieve<TEntity>(partitionKey, rowKey);
            //CloudTable.ExecuteQuery and provide a query(e.g.PartitionKey eq 'your-partition-key-value'

            //    cloudTable.Ex

            //var result = await cloudTable.ExecuteAsync(rangeQuery);



            //var resp = req.CreateResponse(HttpStatusCode.OK);
            //await resp.WriteAsJsonAsync(todo);
            //return resp;

            // Execute the query and loop through the results
            //foreach (ApiResponse entity in
            //    await cloudTable.ExecuteQuerySegmentedAsync(rangeQuery, null))
            //{
            //    log.LogInformation(
            //        $"{entity.PartitionKey}\t{entity.RowKey}\t{entity.Timestamp}\t{entity.Log}");
            //}

            //return new OkObjectResult(JsonConvert.SerializeObject(result.Results));

            return new OkObjectResult(responseMessage);
        }
    }
}
