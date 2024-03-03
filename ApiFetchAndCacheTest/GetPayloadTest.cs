using ApiFetchAndCacheApp;
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
            //mock & assign

            var sut = new GetPayload(NullLoggerFactory.Instance);

            Assert.Pass(); // 404
        }

        [Test]
        public void GetPayloadReturnsContentAnd200WhenBlobInStorage()
        {
            //mock & assign

            var sut = new GetPayload(NullLoggerFactory.Instance);


            Assert.Pass(); // 200
        }
    }
}