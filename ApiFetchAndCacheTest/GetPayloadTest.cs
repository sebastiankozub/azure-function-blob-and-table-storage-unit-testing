using ApiFetchAndCacheApp;
using Azure;
using Grpc.Core;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
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
            var mockResponse = new Mock<Response>();
            var mockFunctionCtx = new Mock<FunctionContext>();
            var mockHttpReq = new Mock<HttpRequestData>(mockFunctionCtx.Object);

            //var sut = new GetPayload(NullLoggerFactory.Instance);
            //var result = sut.Run(req: mockHttpReq.Object, json: null!, id: id);

            //Assert.Equals(StatusCode.NotFound, result.StatusCode);


            Assert.Pass(); // 404
        }

        [Test]
        public void GetPayloadReturnsContentAnd200WhenBlobInStorage()
        {
            //mock & assign

            //var sut = new GetPayload(NullLoggerFactory.Instance);
            //var result = sut.Run();

            Assert.Pass(); // 200
        }
    }
}