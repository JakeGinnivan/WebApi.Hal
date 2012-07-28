using System;
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
            var link = templateLink.CreateLink();

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
            var link = templateLink.CreateLink();

            // assert
            Assert.True(link.IsTemplated);
        }

        [Fact]
        public void substitutes_templated_link()
        {
            // arrange
            var templateLink = new Link("beerSearch", "/beers?searchTerm={searchTerm}");

            // act
            var link = templateLink.CreateLink(searchTerm => "test");

            // assert
            Assert.Equal("/beers?searchTerm=test", link.Href);
        }
    }
}