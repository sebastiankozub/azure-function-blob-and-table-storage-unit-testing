using ApiFetchAndCacheApp;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Logging.Abstractions;


namespace ApiFetchAndCacheTest
{
    public class GetPayloadTest
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void GetPayloadReturnsNotFoundWhenBlobNotInStorage()
        { 
            var id = "003438743878734873487";
            var binaryData = new BinaryData(new byte[] { 12, 12, 12, 12 });
                       
            var mockBlobDownloadResult = new BlobsModelFactory.BlobDownloadResult(binaryData, new BlobDownloadDetails());

            //mockBlobDownloadResult.Content = binaryData;

            var mockResponse = new Mock<Response<BlobDownloadResult>>();
            mockResponse.SetupGet(p => p.Value.Content).Returns(mockBlobDownloadResult);

            var mockFunctionCtx = new Mock<FunctionContext>();
            var mockHttpReq = new Mock<HttpRequestData>(mockFunctionCtx.Object);

            var mockBlobItem1 = new Mock<BlobItem>(); 
            var mockBlobItem2 = new Mock<BlobItem>();
            var mockBlobItem3 = new Mock<BlobItem>();

            var page1 = Page<BlobItem>.FromValues(new[] { mockBlobItem1.Object, mockBlobItem2.Object }, "continuationToken", Mock.Of<Response>());
            var lastPage = Page<BlobItem>.FromValues(new[] { mockBlobItem3.Object }, continuationToken: null, Mock.Of<Response>());

            var asyncPageable = AsyncPageable<BlobItem>.FromPages(new[] { page1, lastPage });

            var mockBlobContainerClient = new Mock<BlobContainerClient>();
            mockBlobContainerClient.Setup(m => m.GetBlobsAsync(default, default, default, default)).Returns(asyncPageable);
            mockBlobContainerClient.Setup(m => m.CreateIfNotExists(default, default, default, default)).Returns(Mock.Of<Response<BlobContainerInfo>>);

            var mockBlobClient = new Mock<BlobClient>();
            mockBlobClient.Setup(m => m.DownloadContentAsync()).Returns(Task.FromResult(mockResponse.Object));

            mockBlobContainerClient.Setup(m => m.GetBlobClient(It.IsAny<string>())).Returns(mockBlobClient.Object);
            
            var mockBlobServiceClient = new Mock<BlobServiceClient>();
            mockBlobServiceClient.Setup(m => m.GetBlobContainerClient(It.IsAny<string>())).Returns(mockBlobContainerClient.Object);

            var mockBlobClientFactory = new Mock<IAzureClientFactory<BlobServiceClient>>();
            mockBlobClientFactory.Setup(m => m.CreateClient(It.IsAny<string>())).Returns(mockBlobServiceClient.Object);

            var expected = new List<BlobItem>() { mockBlobItem1.Object, mockBlobItem2.Object, mockBlobItem3.Object };

            var sut = new GetPayload(NullLoggerFactory.Instance, mockBlobClientFactory.Object);
            var result = sut.Run(req: mockHttpReq.Object, id: id);

            Assert.Equals(StatusCode.NotFound, result.Result.StatusCode); // returns 404
        }

        //[Test]
        //public void GetPayloadReturnsContentAnd200WhenBlobInStorage()
        //{
        //    //mock & assign

        //    //var sut = new GetPayload(NullLoggerFactory.Instance);
        //    //var result = sut.Run();

        //    Assert.Pass(); // 200
        //}
    }
}