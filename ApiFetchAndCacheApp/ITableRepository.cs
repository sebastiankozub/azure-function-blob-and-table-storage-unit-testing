using ApiFetchAndCacheApp.Model;
using ApiFetchAndCacheApp.Options;
using Azure;
using Azure.Data.Tables;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace ApiFetchAndCacheApp
{
    public interface ITableRepository<T> where T : ITableEntity
    {
        Task<T> GetAsync(string rowKey);
        IAsyncEnumerable<T> QueryAsync(string filter);        
        IAsyncEnumerable<T> QueryAsync(Expression<Func<T, bool>> filter);
        Task<Response> CreateAsync(string rowKey, T entity);
        Task<Response> UpdateAsync(string rowKey, T entity);
        Task<Response> CreateOrUpdateAsync(string rowKey, T entity);
        Task<Response> DeleteAsync(string rowKey);
    }

    public class TableRepository : ITableRepository<ApiResponse>
    {
        private readonly ILogger<BlobRepository> _logger;
        private readonly TableClient _tableClient;
        private readonly LogStorageOptions _logStorageOptions;

        public TableRepository(ILoggerFactory loggerFactory, IAzureClientFactory<TableServiceClient> tableClientFactory, LogStorageOptions logStorageOptions)
        {
            _logger = loggerFactory.CreateLogger<BlobRepository>();

            _tableClient = tableClientFactory.CreateClient("ApiFetchAndCache").GetTableClient(logStorageOptions.Table);
            _tableClient.CreateIfNotExists();

            _logStorageOptions = logStorageOptions;
        }

        public async Task<ApiResponse> GetAsync(string rowKey)
        {
            var tableResponse = await _tableClient.GetEntityAsync<ApiResponse>(_logStorageOptions.PartitionKey, rowKey);
            return tableResponse.Value;
        }

        public IAsyncEnumerable<ApiResponse> QueryAsync(string filter)
        {
            return _tableClient.QueryAsync<ApiResponse>(filter);
        }

        public IAsyncEnumerable<ApiResponse> QueryAsync(Expression<Func<ApiResponse, bool>> filter)
        {
            return _tableClient.QueryAsync<ApiResponse>(filter);
        }

        public Task<Response> CreateAsync(string rowKey, ApiResponse entity)
        {
            throw new NotImplementedException();
        }

        public Task<Response> UpdateAsync(string rowKey, ApiResponse entity)
        {
            throw new NotImplementedException();
        }

        public Task<Response> CreateOrUpdateAsync(string rowKey, ApiResponse entity)
        {
            throw new NotImplementedException();
        }

        public Task<Response> DeleteAsync(string rowKey)
        {
            throw new NotImplementedException();
        }
    }
}
