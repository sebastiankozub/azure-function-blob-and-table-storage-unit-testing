using Azure.Data.Tables;
using Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiFetchAndCacheApp
{
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
