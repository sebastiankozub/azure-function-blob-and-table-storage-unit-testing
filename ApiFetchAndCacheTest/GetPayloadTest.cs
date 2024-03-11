using ApiFetchAndCacheApp;
using Azure;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text;

namespace ApiFetchAndCacheTest;

public class GetPayloadTest
{
    private Mock<ILoggerFactory> _loggerFactoryMock;
    private Mock<ILogger> _loggerMock;
    private Mock<IBlobRepository> _blobRepositoryMock;
    private Mock<FunctionContext> _mockFunctionCtx;


    [SetUp]
    public void Setup()
    {
        _loggerFactoryMock = new Mock<ILoggerFactory>();
        _loggerMock = new Mock<ILogger>();
        _blobRepositoryMock = new Mock<IBlobRepository>();

        _loggerFactoryMock.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(_loggerMock.Object);

       _mockFunctionCtx = new Mock<FunctionContext>();
        //mockFunctionCtx.SetupProperty(c => c.InstanceServices, serviceProvider);
    }

    [Test]
    public async Task GetPayload_SuccessfulExecution()
    {
        var payloadId = "123";
        var expectedBlobContent = "Your expected blob content";

        _blobRepositoryMock.Setup(x => x.GetAsync($"{payloadId}.json")).ReturnsAsync(new MemoryStream(Encoding.UTF8.GetBytes(expectedBlobContent)));

        var responseMock = new FakeHttpResponseData(_mockFunctionCtx.Object);

        var httpRequestMock = new Mock<HttpRequestData>(_mockFunctionCtx.Object);
        httpRequestMock.Setup(x => x.CreateResponse()).Returns(responseMock);

        var function = new GetPayload(_loggerFactoryMock.Object, _blobRepositoryMock.Object);
        var result = await function.Run(httpRequestMock.Object, payloadId);

        Assert.That(result, Is.Not.Null);
        Assert.That((int)result.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));

        //result.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(result.Body, Encoding.UTF8);        
        var bodyString = await reader.ReadToEndAsync();        
        Assert.That(bodyString, Is.EqualTo(expectedBlobContent));
    }

    [Test]
    public async Task GetPayload_NotFound()
    {
        var payloadId = "123";

        _blobRepositoryMock.Setup(x => x.GetAsync($"{payloadId}.json")).ThrowsAsync(new RequestFailedException(404, "Not Found"));

        var responseMock = new FakeHttpResponseData(_mockFunctionCtx.Object);

        var httpRequestMock = new Mock<HttpRequestData>(_mockFunctionCtx.Object);
        httpRequestMock.Setup(x => x.CreateResponse()).Returns(responseMock);

        var function = new GetPayload(_loggerFactoryMock.Object, _blobRepositoryMock.Object);
        var result = await function.Run(httpRequestMock.Object, payloadId);

        Assert.That(result, Is.Not.Null);
        Assert.That((int)result.StatusCode, Is.EqualTo((int)HttpStatusCode.NotFound));
    }

    [Test]
    public async Task GetPayload_InternalServerError()
    {
        var payloadId = "789";

        _blobRepositoryMock.Setup(x => x.GetAsync($"{payloadId}.json")).ThrowsAsync(new RequestFailedException(500, "Internal Server Error"));

        var responseMock = new FakeHttpResponseData(_mockFunctionCtx.Object);

        var httpRequestMock = new Mock<HttpRequestData>(_mockFunctionCtx.Object);
        httpRequestMock.Setup(x => x.CreateResponse()).Returns(responseMock);

        var function = new GetPayload(_loggerFactoryMock.Object, _blobRepositoryMock.Object);
        var result = await function.Run(httpRequestMock.Object, payloadId);

        Assert.That(result, Is.Not.Null);
        Assert.That((int)result.StatusCode, Is.EqualTo((int)HttpStatusCode.InternalServerError));
    }

    [Test]
    public async Task GetPayload_UnhandledException()
    {
        var payloadId = "101";

        _blobRepositoryMock.Setup(x => x.GetAsync($"{payloadId}.json")).ThrowsAsync(new InvalidOperationException("Some unhandled exception"));

        var responseMock = new FakeHttpResponseData(_mockFunctionCtx.Object);

        var httpRequestMock = new Mock<HttpRequestData>(_mockFunctionCtx.Object);
        httpRequestMock.Setup(x => x.CreateResponse()).Returns(responseMock);

        var function = new GetPayload(_loggerFactoryMock.Object, _blobRepositoryMock.Object);
        var result = await function.Run(httpRequestMock.Object, payloadId);

        Assert.That(result, Is.Not.Null);
        Assert.That((int)result.StatusCode, Is.EqualTo((int)HttpStatusCode.InternalServerError));
    }
}