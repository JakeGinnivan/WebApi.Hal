﻿using System.Buffers;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Assent;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using WebApi.Hal.Tests.HypermediaAppenders;
using WebApi.Hal.Tests.Representations;
using Xunit;

namespace WebApi.Hal.Tests
{
    public class HypermediaContainerTests
    {
        readonly ProductRepresentation representation = new ProductRepresentation();

        [Fact]
        public async Task CanUseRegisterExtensionMethod()
        {
            var curie = new CuriesLink("aap", "http://www.helpt.com/{?rel}");

            var builder = Hypermedia.CreateBuilder();
            var selfLink = new Link<ProductRepresentation>("product", "http://www.product.com?id=1");
            var link2 = new Link("related", "http://www.related.com");
            var link3 = curie.CreateLink<CategoryRepresentation>("category", "http://www.category.com");
            
            builder.Register(selfLink, link2, link3);
            
            var config = builder.Build();

            // arrange
            var mediaFormatter = new JsonHalMediaTypeOutputFormatter(
                new JsonSerializerSettings(), ArrayPool<char>.Shared);
            var content = new StringContent(string.Empty);
            var type = representation.GetType();

            // act
            using (var stream = new MemoryStream())
            {
                var context = new DefaultHttpContext();
                context.Response.Body = stream;

                await mediaFormatter.WriteResponseBodyAsync(
                    new Microsoft.AspNetCore.Mvc.Formatters.OutputFormatterWriteContext(
                        context,
                      (writeStream, effectiveEncoding) => new StreamWriter(writeStream, effectiveEncoding),
                      type,
                      representation), Encoding.UTF8);

                stream.Seek(0, SeekOrigin.Begin);
                var serialisedResult = new StreamReader(stream).ReadToEnd();

                // assert
                this.Assent(serialisedResult);
            }
        }

        [Fact]
        public void CanRegisterAndResolveAHypermediaAppender()
        {
            var appender = new ProductRepresentationHypermediaAppender();
            var resource = new ProductRepresentation();
            var builder = Hypermedia.CreateBuilder();

            builder.RegisterAppender(appender);

            var config = builder.Build();
            var registered = config.ResolveAppender(resource);

            Assert.True(ReferenceEquals(appender, registered));

        }

        [Fact]
        public void ResolvesNullHypermediaAppenderIfNotRegistered()
        {
            var resource = new ProductRepresentation();
            var builder = Hypermedia.CreateBuilder();

            var config = builder.Build();
            var registered = config.ResolveAppender(resource);

            Assert.Null(registered);
        }

        [Fact]
        public void CanRegisterAndResolveASelfLinkForARepresentation()
        {
            const string href = "http://api.example.com/products/{id}";

            var link = new Link("example-namespace:product", href);
            var builder = Hypermedia.CreateBuilder();
            
            builder.RegisterSelf<ProductRepresentation>(link);
            
            var config = builder.Build();
            var registered = config.ResolveSelf(representation);

            Assert.Equal(Link.RelForSelf, registered.Rel);
            Assert.Equal(href, registered.Href);
        }

        [Fact]
        public void CanRegisterAndResolveALinkForASingleRepresentation()
        {
            var builder = Hypermedia.CreateBuilder();
            var link = new Link();

            builder.RegisterLinks<ProductRepresentation>(link);
            
            var config = builder.Build();
            var hypermedia = config.ResolveLinks(representation);

            Assert.Equal(1, hypermedia.Count());
            Assert.Equal(link, hypermedia.First(), Link.EqualityComparer);
        }

        [Fact]
        public void CanRegisterAndResolveMultipleLinksForASingleRepresentation()
        {
            var link1 = new Link("foo", "bar");
            var link2 = new Link("baz", "qux");
            var builder = Hypermedia.CreateBuilder();

            builder.RegisterLinks<ProductRepresentation>(link1, link2);

            var config = builder.Build();
            var hypermedia = config.ResolveLinks(representation);

            Assert.Equal(2, hypermedia.Count());
            Assert.Contains(link1, hypermedia, Link.EqualityComparer);
            Assert.Contains(link2, hypermedia, Link.EqualityComparer);
        }
    }
}
