using System.Linq;
using WebApi.Hal.Tests.HypermediaAppenders;
using WebApi.Hal.Tests.Representations;
using Xunit;

namespace WebApi.Hal.Tests
{
    public class HypermediaConfigurationTests
    {
        readonly ProductRepresentation representation = new ProductRepresentation();

        [Fact]
        public void CanRegisterAndResolveAHypermediaAppender()
        {
            var appender = new ProductRepresentationHypermediaAppender();
            var resource = new ProductRepresentation();
            var builder = new HypermediaConfigurationBuilder();

            builder.RegisterAppender(appender);

            var config = builder.Build();
            var registered = config.ResolveAppender(resource);

            Assert.True(ReferenceEquals(appender, registered));

        }
        [Fact]
        public void CanRegisterAndResolveASelfLinkForARepresentation()
        {
            const string href = "http://api.example.com/products/{id}";

            var link = new Link("example-namespace:product", href); 
            var builder = new HypermediaConfigurationBuilder();
            
            builder.RegisterSelf<ProductRepresentation>(link);
            
            var config = builder.Build();
            var registered = config.ResolveSelf(representation);

            Assert.Equal(Link.RelForSelf, registered.Rel);
            Assert.Equal(href, registered.Href);
        }

        [Fact]
        public void CanRegisterAndResolveALinkForASingleRepresentation()
        {
            var builder = new HypermediaConfigurationBuilder();
            var link = new Link();

            builder.RegisterLinks<ProductRepresentation>(link);
            
            var config = builder.Build();
            var hypermedia = config.ResolveLinks(representation);

            Assert.Equal(1, hypermedia.Count());
            Assert.Equal(link, hypermedia.First(), new LinkEqualityComparer());
        }

        [Fact]
        public void CanRegisterAndResolveMultipleLinksForASingleRepresentation()
        {
            var link1 = new Link("foo", "bar");
            var link2 = new Link("baz", "qux");
            var builder = new HypermediaConfigurationBuilder();

            builder.RegisterLinks<ProductRepresentation>(link1, link2);

            var config = builder.Build();
            var hypermedia = config.ResolveLinks(representation);

            Assert.Equal(2, hypermedia.Count());
            Assert.Contains(link1, hypermedia, new LinkEqualityComparer());
            Assert.Contains(link2, hypermedia, new LinkEqualityComparer());
        }

        [Fact]
        public void CanRegisterSingleCuriesLinkAndResolveItBasedOnALink()
        {
            const string href = "http://api.example.com/docs/{rel}"; 
            
            var link = new Link("foo:bar", "http://api.example.com/baz");
            var builder = new HypermediaConfigurationBuilder();

            builder.RegisterCuries(new CuriesLink("foo", href));

            var config = builder.Build();
            var curies = config.ResolveCuries(link);

            Assert.Equal(1, curies.Count());
            Assert.Equal(href, curies.First().Href);
        }

        [Fact]
        public void CanRegisterMultipleCuriesLinksAndResolveOneBasedOnALink()
        {
            const string href = "http://api.example.com/docs/{rel}";

            var link = new Link("foo:bar", "http://api.example.com/baz");
            var builder = new HypermediaConfigurationBuilder();

            builder.RegisterCuries(new CuriesLink("foo", href), new CuriesLink("qux", "http://api.company.com/docs/{rel}"));

            var config = builder.Build();
            var curies = config.ResolveCuries(link);

            Assert.Equal(1, curies.Count());
            Assert.Equal(href, curies.First().Href);
        }

        [Fact]
        public void CanRegisterMultipleCuriesLinksAndResolveAllBasedOnLinks()
        {
            const string hrefFoo = "http://api.example.com/docs/{rel}";
            const string hrefQux = "http://api.company.com/docs/{rel}";

            var link1 = new Link("foo:bar", "http://api.example.com/baz");
            var link2 = new Link("qux:bar", "http://api.company.com/baz");
            var builder = new HypermediaConfigurationBuilder();
            
            builder.RegisterCuries(new CuriesLink("foo", hrefFoo), new CuriesLink("qux", hrefQux));

            var config = builder.Build();
            var curies = config.ResolveCuries(link1, link2);

            Assert.Equal(2, curies.Count());
            Assert.True(curies.Any(x => x.Href == hrefFoo));
            Assert.True(curies.Any(x => x.Href == hrefQux));
        }
    }
}
