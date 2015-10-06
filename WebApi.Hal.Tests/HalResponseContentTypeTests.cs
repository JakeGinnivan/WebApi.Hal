using System.Web.Http;
using WebApi.Hal.Tests.Representations;
using WebApi.Hal.Tests.TestFixtures;
using Xunit;

namespace WebApi.Hal.Tests
{
    public class HalResponseContentTypeTests : HttpServerFixture
    {
        [Fact]
        public void formatter_sets_contenttype_to_applicationhaljson()
        {
            var response = Client.GetAsync("test/1").Result;
            Assert.Equal("application/hal+json", response.Content.Headers.ContentType.MediaType);
        }

        public class TestController : ApiController
        {
            public ProductRepresentation Get(int id)
            {
                return new ProductRepresentation { };
            }
        }
    }
}