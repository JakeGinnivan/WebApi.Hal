using System.Collections.Generic;
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
        public void detects_nontemplated_link()
        {
            // arrange
            var templateLink = new Link("beerSearch", "/beers{?searchTerm}");

            // act
            var link = templateLink.CreateLink(new {});

            // assert
            Assert.False(link.IsTemplated);
        }

        [Fact]
        public void detects_templated_link()
        {
            // arrange

            // act
            var templateLink = new Link("beerSearch", "/beers{?searchTerm}");

            // assert
            Assert.True(templateLink.IsTemplated);
        }

        [Fact]
        public void substitutes_templated_link()
        {
            // arrange
            var templateLink = new Link("beerSearch", "/beers{?searchTerm}");

            // act
            var link = templateLink.CreateLink(new{searchTerm = "test"});

            // assert
            Assert.Equal("/beers?searchTerm=test", link.Href);
        }

        [Fact]
        public void create_link_handles_expansion()
        {
            // arrange
            var templateLink = new Link("beers", "/beers{?searchTerm}");

            // act
            var link = templateLink.CreateLink(new {searchTerm = new[] {"test1", "test2"}});

            // assert
            Assert.Equal("/beers?searchTerm=test1,test2", link.Href);
        }

        [Fact]
        public void create_link_handles_expansion2()
        {
            // arrange
            var templateLink = new Link("beers", "/beers{?searchTerm*}");

            // act
            var link = templateLink.CreateLink(new {searchTerm = new[] {"test1", "test2"}});

            // assert
            Assert.Equal("/beers?searchTerm=test1&searchTerm=test2", link.Href);
        }

        [Fact]
        public void create_link_handles_non_string_objects()
        {
            // arrange
            var templateLink = new Link("beers", "/beers{?searchTerm}");

            // act
            var link = templateLink.CreateLink(new {searchTerm = new object()});

            // assert somewhat useless behaviour
            Assert.Equal("/beers?searchTerm=System.Object", link.Href);
        }
        
        [Fact]
        public void create_link_handles_spaces()
        {
            // arrange
            var templateLink = new Link("beerbyname", "/beers/{name}");

            // act
            var link = templateLink.CreateLink(new { name = "Tactical Nuclear Penguin" });

            // assert
            Assert.Equal("/beers/Tactical%20Nuclear%20Penguin", link.Href);
        }

        [Fact]
        public void create_link_substitution_is_case_sensitive1()
        {
            // arrange
            var templateLink = new Link("beerbyname", "/beers/{naMe}");

            // act
            var link = templateLink.CreateLink(new { nAme = "Sorry Charlie" });

            // assert
            Assert.Equal("/beers/", link.Href);
        }

        [Fact]
        public void create_link_substitution_is_case_sensitive2()
        {
            // arrange
            var templateLink = new Link("beerbyname", "/beers/{nAme}");

            // act
            var link = templateLink.CreateLink(new { nAme = "This Works" });

            // assert
            Assert.Equal("/beers/This%20Works", link.Href);
        }

        [Fact]
        public void create_uri_absolute()
        {
            // arrange
            var templateLink = new Link("beerbyname", "http://myserver.com/api/beers/{name}");

            // act
            var link = templateLink.CreateUri(new {name = "BeerName"});

            // assert
            Assert.Equal("http://myserver.com/api/beers/BeerName", link.ToString());
        }

        [Fact]
        public void create_link_uses_templates_title()
        {
            // arrange
            var templateLink = new Link("beerbyname", "http://myserver.com/api/beers{name}", "Beer");

            // act
            var link = templateLink.CreateLink(new {name = "BeerName"});

            // assert
            Assert.Equal("Beer", link.Title);
        }
    }
}