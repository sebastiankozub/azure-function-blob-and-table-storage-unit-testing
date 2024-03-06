using Azure.Storage.Blobs;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiFetchAndCacheApp
{
    internal interface IPayloadService
    {
        Task<bool> SavePayload(string guid, string blobValue);
        Task<string> GetPayload(string guid);
    }

    public class PayloadService : IPayloadService
    {
        private readonly ILogger<PayloadService> _logger;
        private readonly BlobContainerClient _blobContainerClient;

        public PayloadService(ILoggerFactory loggerFactory, IAzureClientFactory<BlobServiceClient> blobClientFactory)
        {
            _logger = loggerFactory.CreateLogger<PayloadService>();

            _blobContainerClient = blobClientFactory.CreateClient("ApiFetchAndCache").GetBlobContainerClient("my-blobs");
            _blobContainerClient.CreateIfNotExists();
        }

        public Task<string> GetPayload(string guid)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SavePayload(string guid, string blobValue)
        {
            throw new NotImplementedException();
        }
    }
}
