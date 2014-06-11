using System.IO;
using System.Net.Http;
using ApprovalTests;
using ApprovalTests.Reporters;
using WebApi.Hal.Tests.HypermediaAppenders;
using WebApi.Hal.Tests.Representations;
using Xunit;

namespace WebApi.Hal.Tests
{
    public class ResolvingHalResourceTest
    {
        readonly ProductRepresentation representation;
        readonly IHypermediaConfiguration config;

        public ResolvingHalResourceTest()
        {
            //
            // Create representation

            representation = new ProductRepresentation
            {
                Id = 9,
                Title = "Morpheus in a chair statuette",
                Price = 20.14,
                Category = new CategoryRepresentation
                {
                    Id = 99,
                    Title = "Action Figures"
                }
            };

            //
            // Build hypermedia configuration

            var curiesLink = new CuriesLink("example-namespace", "http://api.example.com/docs/{rel}");
            var productLink = new Link("example-namespace:product", "http://api.example.com/products/{id}");
            var relatedProductLink = new Link("example-namespace:related-product", productLink.Href);
            var saleProductLink = new Link("example-namespace:product-on-sale", "http://api.example.com/products/sale/{id}");
            var categoryLink = new Link("example-namespace:category", "http://api.example.com/categories/{id}");

            var builder = new HypermediaConfigurationBuilder();

            builder.RegisterAppender(new ProductRepresentationHypermediaAppender());
            builder.RegisterAppender(new CategoryRepresentationHypermediaAppender());
            builder.RegisterCuries(curiesLink);
            builder.RegisterSelf<ProductRepresentation>(productLink);
            builder.RegisterSelf<CategoryRepresentation>(categoryLink);
            builder.RegisterLinks<ProductRepresentation>(relatedProductLink, saleProductLink);

            config = builder.Build();
        }
        
        [Fact]
        [UseReporter(typeof(DiffReporter))]
        public void ProperlySerializesRepresentationToJson()
        {
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
        [UseReporter(typeof(DiffReporter))]
        public void ProperlySerializesRepresentationToXml()
        {
            // arrange
            var mediaFormatter = new XmlHalMediaTypeFormatter(config);
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
    }
}