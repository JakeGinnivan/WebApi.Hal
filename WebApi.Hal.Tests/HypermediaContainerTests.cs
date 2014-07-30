using System.IO;
using System.Linq;
using System.Net.Http;
using ApprovalTests;
using ApprovalTests.Reporters;
using WebApi.Hal.Tests.HypermediaAppenders;
using WebApi.Hal.Tests.Representations;
using Xunit;

namespace WebApi.Hal.Tests
{
    public class HypermediaContainerTests
    {
        readonly ProductRepresentation representation = new ProductRepresentation();

        [Fact]
        [UseReporter(typeof(DiffReporter))]
        public void CanUseRegisterExtensionMethod()
        {
            var curie = new CuriesLink("aap", "http://www.helpt.com/{?rel}");

            var builder = new HypermediaContainerBuilder();
            var selfLink = new Link<ProductRepresentation>("product", "http://www.product.com?id=1");
            var link2 = new Link("related", "http://www.related.com");
            var link3 = curie.CreateLink<CategoryRepresentation>("category", "http://www.category.com");
            
            builder.Register(selfLink, link2, link3);
            
            var config = builder.Build();

            // arrange
            var mediaFormatter = new JsonHalMediaTypeFormatter(config) { Indent = true };
            var content = new StringContent(string.Empty);
            var type = representation.GetType();

            // act
            using (var stream = new MemoryStream())
            {
                mediaFormatter.WriteToStreamAsync(type, representation, stream, content, null);
                stream.Seek(0, SeekOrigin.Begin);
                var serialisedResult = new StreamReader(stream).ReadToEnd();

                // assert
                Approvals.Verify(serialisedResult, s => s.Replace("\r\n", "\n"));
            }
        }

        [Fact]
        public void CanRegisterAndResolveAHypermediaAppender()
        {
            var appender = new ProductRepresentationHypermediaAppender();
            var resource = new ProductRepresentation();
            var builder = new HypermediaContainerBuilder();

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
            var builder = new HypermediaContainerBuilder();
            
            builder.RegisterSelf<ProductRepresentation>(link);
            
            var config = builder.Build();
            var registered = config.ResolveSelf(representation);

            Assert.Equal(Link.RelForSelf, registered.Rel);
            Assert.Equal(href, registered.Href);
        }

        [Fact]
        public void CanRegisterAndResolveALinkForASingleRepresentation()
        {
            var builder = new HypermediaContainerBuilder();
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
            var builder = new HypermediaContainerBuilder();

            builder.RegisterLinks<ProductRepresentation>(link1, link2);

            var config = builder.Build();
            var hypermedia = config.ResolveLinks(representation);

            Assert.Equal(2, hypermedia.Count());
            Assert.Contains(link1, hypermedia, new LinkEqualityComparer());
            Assert.Contains(link2, hypermedia, new LinkEqualityComparer());
        }

        [Fact]
        public void CanRegisterASelfLinkAndExtractACuriesLink()
        {
            const string href = "http://api.example.com/docs/{rel}";

            var foo = new CuriesLink("foo", href);
            var link = foo.CreateLink("bar", "http://api.example.com/baz");
            var builder = new HypermediaContainerBuilder();

            builder.RegisterSelf<ProductRepresentation>(link);

            var config = builder.Build();
            var curies = config.ExtractUniqueCuriesLinks(link);

            Assert.Equal(1, curies.Count());
            Assert.Equal(href, curies.First().Href);
        }

        [Fact]
        public void CanRegisterALinkAndExtractACuriesLink()
        {
            const string href = "http://api.example.com/docs/{rel}";

            var foo = new CuriesLink("foo", href);
            var link = foo.CreateLink("bar", "http://api.example.com/baz");
            var builder = new HypermediaContainerBuilder();

            builder.RegisterLinks<ProductRepresentation>(link);

            var config = builder.Build();
            var curies = config.ExtractUniqueCuriesLinks(link);

            Assert.Equal(1, curies.Count());
            Assert.Equal(href, curies.First().Href);
        }

        [Fact]
        public void CanRegistermultipleLinksAndExtractAllCuriesLink()
        {
            const string hrefFoo = "http://api.example.com/docs/{rel}";
            const string hrefQux = "http://api.company.com/docs/{rel}";

            var curie1 = new CuriesLink("foo", hrefFoo);
            var curie2 = new CuriesLink("bar", hrefQux);

            var link1 = curie1.CreateLink("baz", "http://api.example.com/baz");
            var link2 = curie2.CreateLink("qux", "http://api.company.com/baz");
            
            var builder = new HypermediaContainerBuilder();
            
            builder.RegisterLinks<ProductRepresentation>(link1, link2);

            var config = builder.Build();
            var curies = config.ExtractUniqueCuriesLinks(link1, link2);

            Assert.Equal(2, curies.Count());
            Assert.True(curies.Any(x => x.Href == hrefFoo));
            Assert.True(curies.Any(x => x.Href == hrefQux));
        }
    }
}
