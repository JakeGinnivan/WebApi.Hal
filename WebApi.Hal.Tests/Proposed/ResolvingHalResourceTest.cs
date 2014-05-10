using System.IO;
using System.Net.Http;
using ApprovalTests;
using ApprovalTests.Reporters;
using WebApi.Hal.Proposed;
using WebApi.Hal.Tests.Proposed.HypermediaAppenders;
using WebApi.Hal.Tests.Proposed.Representations;
using WebApi.Hal.Tests.Representations;
using Xunit;

namespace WebApi.Hal.Tests.Proposed
{
    public class ResolvingHalResourceTest
    {
        readonly OrganisationRepresentation resource;

        public ResolvingHalResourceTest()
        {
            resource = new OrganisationRepresentation(1, "Org Name");
        }

        [Fact]
        [UseReporter(typeof(DiffReporter))]
        public void ProperlySerializesRepresentationToJson()
        {
            // arrange
            var representation = new ProductRepresentation
            {
                Id = 9,
                Title = "Morpheus in a chair statuette",
                Category = new CategoryRepresentation
                {
                    Id = 99,
                    Title = "Action Figures"
                }
            };

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

            var config = builder.Build();
            var mediaFormatter = new JsonHalMediaTypeFormatter(config) { Indent = true };
            var content = new StringContent(string.Empty);
            var type = resource.GetType();

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
        public void organisation_get_json_with_app_path_test()
        {
            // arrange
            var mediaFormatter = new JsonHalMediaTypeFormatter { Indent = true };
            var content = new StringContent(string.Empty);
            var resourceWithAppPath = new OrganisationWithAppPathRepresentation(1, "Org Name");
            var type = resourceWithAppPath.GetType();

            // act
            using (var stream = new MemoryStream())
            {
                mediaFormatter.WriteToStreamAsync(type, resourceWithAppPath, stream, content, null);
                stream.Seek(0, SeekOrigin.Begin);
                var serialisedResult = new StreamReader(stream).ReadToEnd();

                // assert
                Approvals.Verify(serialisedResult, s => s.Replace("\r\n", "\n"));
            }
        }

        [Fact]
        [UseReporter(typeof(DiffReporter))]
        public void organisation_get_xml_test()
        {
            // arrange
            var mediaFormatter = new XmlHalMediaTypeFormatter();
            var content = new StringContent(string.Empty);
            var type = resource.GetType();

            // act
            using (var stream = new MemoryStream())
            {
                mediaFormatter.WriteToStreamAsync(type, resource, stream, content, null);
                stream.Seek(0, SeekOrigin.Begin);
                var serialisedResult = new StreamReader(stream).ReadToEnd();

                // assert
                Approvals.Verify(serialisedResult, s => s.Replace("\r\n", "\n"));
            }
        } 
    }
}