using ApiFetchAndCacheApp;
using ApiFetchAndCacheApp.Options;
using Azure;
using Azure.Core;
using Azure.Core.Serialization;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Net;
using System.Text;

namespace ApiFetchAndCacheTest;

public class GetPayloadTest
{

    [SetUp]
    public void Setup()
    {
            
    }


    [Test]
    public void GetPayloadReturnsNotFoundWhenBlobNotInStorage()
    {
        var serviceCollection = new ServiceCollection();            
        serviceCollection.AddOptions<WorkerOptions>().Configure(options => { options.Serializer = new JsonObjectSerializer(); });
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var payloadId = "003438743878734873487";

        var byteArray = Encoding.ASCII.GetBytes("test-content-finally-json-from-file");
        var binaryData = new BinaryData(byteArray);
        var bodyStream = new MemoryStream(byteArray);

        var blobDownloadResult = BlobsModelFactory.BlobDownloadResult(binaryData, new BlobDownloadDetails());
        var blobDownloadResultResponse = Response.FromValue(blobDownloadResult, default!);
              
        var mockFunctionCtx = new Mock<FunctionContext>();
        mockFunctionCtx.SetupProperty(c => c.InstanceServices, serviceProvider);

        var mockHttpReq = new Mock<HttpRequestData>(mockFunctionCtx.Object);
        mockHttpReq.Setup(r => r.Body).Returns(bodyStream);
        mockHttpReq.Setup(r => r.CreateResponse()).Returns(() =>
        {
            var response = new Mock<HttpResponseData>(mockFunctionCtx.Object);
            response.SetupProperty(r => r.Headers, new HttpHeadersCollection());
            response.SetupProperty(r => r.StatusCode);
            response.SetupProperty(r => r.Body, new MemoryStream());
            return response.Object;
        });

        //mockHttpReq
        //    .Setup(r => r.CreateResponse(It.IsAny<HttpStatusCode>())).Returns(() =>
        //    {
        //        var response = new Mock<HttpResponseData>(mockFunctionCtx.Object);
        //        response.SetupProperty(r => r.Headers, new HttpHeadersCollection());
        //        response.SetupProperty(r => r.StatusCode);
        //        response.SetupProperty(r => r.Body, new MemoryStream());
        //        return response.Object;
        //    });
        //mockHttpReq
        //    .Setup(r => r.CreateResponse(It.Is<HttpStatusCode>(u => u == HttpStatusCode.NotFound))).Returns(() =>
        //    {
        //        var response = new Mock<HttpResponseData>(mockFunctionCtx.Object);
        //        response.SetupProperty(r => r.Headers, new HttpHeadersCollection());
        //        response.SetupGet(r => r.StatusCode).Returns(HttpStatusCode.NotFound);
        //        response.SetupProperty(r => r.Body, new MemoryStream());
        //        return response.Object;
        //    });
        //mockHttpReq
        //    .Setup(r => r.CreateResponse(It.Is<HttpStatusCode>(u => u == HttpStatusCode.OK))).Returns(() =>
        //    {
        //        var response = new Mock<HttpResponseData>(mockFunctionCtx.Object);
        //        response.SetupProperty(r => r.Headers, new HttpHeadersCollection());
        //        response.SetupGet(r => r.StatusCode).Returns(HttpStatusCode.OK);
        //        response.SetupProperty(r => r.Body, new MemoryStream());
        //        return response.Object;
        //    });

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
        //mockBlobClient.Setup(m => m.DownloadContentAsync()).Returns(Task.FromResult(blobDownloadResultResponse)); //200
        mockBlobClient.Setup(m => m.DownloadContentAsync()).ThrowsAsync(new RequestFailedException(404, "RequestFailedException")); //404
        //mockBlobClient.Setup(m => m.DownloadContentAsync()).ThrowsAsync(new Exception("Exception")); //500

        mockBlobContainerClient.Setup(m => m.GetBlobClient(It.IsAny<string>())).Returns(mockBlobClient.Object);
        
        var mockBlobServiceClient = new Mock<BlobServiceClient>();
        mockBlobServiceClient.Setup(m => m.GetBlobContainerClient(It.IsAny<string>())).Returns(mockBlobContainerClient.Object);

        var mockBlobClientFactory = new Mock<IAzureClientFactory<BlobServiceClient>>();
        mockBlobClientFactory.Setup(m => m.CreateClient(It.IsAny<string>())).Returns(mockBlobServiceClient.Object);

        var expected = new List<BlobItem>() { mockBlobItem1.Object, mockBlobItem2.Object, mockBlobItem3.Object };

        var sut = new GetPayload(NullLoggerFactory.Instance, mockBlobClientFactory.Object, new PayloadStorageOptions(), new PublicApiOptions());
        var result = sut.Run(req: mockHttpReq.Object, payloadId: payloadId);
        
        Assert.That((int)result.Result.StatusCode, Is.EqualTo((int)HttpStatusCode.NotFound), "NotFound");
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