using System.IO;
using System.Net.Http;
using System.Text;
using ApprovalTests;
using ApprovalTests.Reporters;
using WebApi.Hal.Tests.Representations;
using Xunit;

namespace WebApi.Hal.Tests
{
    public class HalResourceTest
    {
        readonly OrganisationRepresentation resource;

        public HalResourceTest()
        {
            resource = new OrganisationRepresentation(1, "Org Name");
        }

        [Fact]
        [UseReporter(typeof(DiffReporter))]
        public void organisation_get_json_test()
        {
            // arrange
            var mediaFormatter = new JsonHalMediaTypeFormatter { Indent = true };
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
        public void organisation_get_json_with_no_href_test()
        {
            // arrange
            var mediaFormatter = new JsonHalMediaTypeFormatter { Indent = true };
            var content = new StringContent(string.Empty);
            var resourceWithAppPath = new OrganisationWithNoHrefRepresentation(1, "Org Name");
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
        public void organisation_get_json_with_link_title_test()
        {
            // arrange
            var mediaFormatter = new JsonHalMediaTypeFormatter { Indent = true };
            var content = new StringContent(string.Empty);
            var resourceWithAppPath = new OrganisationWithLinkTitleRepresentation(1, "Org Name");
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
        public void organisation_get_json_with_curies_test()
        {
            // arrange
            var mediaFormatter = new JsonHalMediaTypeFormatter { Indent = true };
            var content = new StringContent(string.Empty);
            var resourceWithAppPath = new OrganisationWithCuriesRepresentation(1, "Org Name");
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
        public void organisation_get_json_with_link_profile_test()
        {
            // arrange

            var link = new Link { Href = "http://foo.com/bar", Rel = "fooey", Profile = "http://bar.com/foo" };
            organization_get_json_links_test(link);
        }

        [Fact]
        [UseReporter(typeof(DiffReporter))]
        public void organisation_get_json_with_link_type_test()
        {
            // arrange

            var link = new Link { Href = "http://foo.com/bar", Rel = "fooey", Type = "text/html" };
            organization_get_json_links_test(link);
        }

        static void organization_get_json_links_test(Link link)
        {
            var mediaFormatter = new JsonHalMediaTypeFormatter {Indent = true};
            var content = new StringContent(string.Empty);
            var resourceWithAppPath = new OrganisationWithAppPathRepresentation(1, "Org Name");
            resourceWithAppPath.Links.Add(link);
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

        [Fact]
        public void curies_post_json_test()
        {
            // arrange
            var mediaFormatter = new JsonHalMediaTypeFormatter { Indent = true };
            var type = typeof(OrganisationRepresentation);
            const string json = @"
{""Id"": ""7"",""Name"": ""Org Seven"",
    
    ""_links"": {""self"": {""href"": ""/api/organisations/7""},

""curies"": [

    {
        ""name"": ""br"",
        ""href"": ""/rels/{rel}"",
        ""templated"": true
      }
    ],},
  
}
";

            // act
            using (
                var stream = new MemoryStream(Encoding.UTF8.GetBytes(json))
                )
            {
                var obj = mediaFormatter.ReadFromStreamAsync(type, stream, null, null).Result;

                // assert
                Assert.NotNull(obj);
                var resource = obj as OrganisationRepresentation;
                Assert.NotNull(resource);
                Assert.Equal("Org Seven", resource.Name);
                Assert.Equal(2, resource.Links.Count);
                Assert.Equal(typeof(Link), resource.Links[0].GetType());
                Assert.Equal(typeof(Curie), resource.Links[1].GetType());
            }
        }
    }
}