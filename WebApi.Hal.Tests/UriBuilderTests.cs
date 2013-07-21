using System.Linq;
using System.Web.Http;
using WebApi.Hal.Web.Api;
using Xunit;

namespace WebApi.Hal.Tests
{
    public class UriBuilderTests
    {
        [Fact]
        public void can_generate_link_without_templates()
        {
            // arrange
            var templateLink = new Link("beers", "/beers");

            // act
            var link = templateLink.CreateLink(new{});

            // assert
            Assert.Equal("beers", link.Rel);
            Assert.Equal(templateLink.Href, link.Href);
            Assert.False(link.IsTemplated);
        }

        [Fact]
        public void detects_templated_link()
        {
            // arrange
            var templateLink = new Link("beerSearch", "/beers?searchTerm={searchTerm}");

            // act
            var link = templateLink.CreateLink(new {});

            // assert
            Assert.True(link.IsTemplated);
        }

        [Fact]
        public void substitutes_templated_link()
        {
            // arrange
            var templateLink = new Link("beerSearch", "/beers?searchTerm={searchTerm}");

            // act
            var link = templateLink.CreateLink(new{searchTerm = "test"});

            // assert
            Assert.Equal("/beers?searchTerm=test", link.Href);
        }

        [Fact]
        public void registers_link_correctly_with_web_api()
        {
            // arrange
            var templateLink = new Link("beerSearch", "/beers/{name}?searchTerm={searchTerm}&page={page=1}");
            var httpRouteCollection = new HttpRouteCollection();

            // act
            templateLink.RegisterLinkWithWebApi<BeersController>(httpRouteCollection);

            // assert
            Assert.NotEmpty(httpRouteCollection);
            Assert.Equal("beers/{name}", httpRouteCollection.Single().RouteTemplate);
            Assert.Equal("1", httpRouteCollection.Single().Defaults["page"]);
            Assert.Equal("Beers", httpRouteCollection.Single().Defaults["controller"]);
        }
    }
}