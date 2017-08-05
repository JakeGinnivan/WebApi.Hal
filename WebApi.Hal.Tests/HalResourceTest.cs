using System.Buffers;
using System.IO;
using Assent;
using Newtonsoft.Json;
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
        public void organisation_get_json_test()
        {
            // arrange
            var mediaFormatter = new JsonHalMediaTypeOutputFormatter(
                new JsonSerializerSettings { Formatting = Formatting.Indented }, ArrayPool<char>.Shared);

            // act
            using (var stream = new StringWriter())
            {
                mediaFormatter.WriteObject(stream, resource);

                string serialisedResult = stream.ToString();

                // assert
                this.Assent(serialisedResult);
            }
        }

        [Fact]
        public void organisation_get_json_with_app_path_test()
        {
            // arrange
            var mediaFormatter = new JsonHalMediaTypeOutputFormatter(
                new JsonSerializerSettings { Formatting = Formatting.Indented }, ArrayPool<char>.Shared);
            var resourceWithAppPath = new OrganisationWithAppPathRepresentation(1, "Org Name");

            // act
            using (var stream = new StringWriter())
            {
                mediaFormatter.WriteObject(stream, resourceWithAppPath);

                string serialisedResult = stream.ToString();

                // assert
                this.Assent(serialisedResult);
            }
        }

        [Fact]
        public void organisation_get_json_with_no_href_test()
        {
            // arrange
            var mediaFormatter = new JsonHalMediaTypeOutputFormatter(
                new JsonSerializerSettings { Formatting = Formatting.Indented }, ArrayPool<char>.Shared);
            var resourceWithAppPath = new OrganisationWithNoHrefRepresentation(1, "Org Name");

            // act
            using (var stream = new StringWriter())
            {
                mediaFormatter.WriteObject(stream, resourceWithAppPath);

                string serialisedResult = stream.ToString();

                // assert
                this.Assent(serialisedResult);
            }
        }

        [Fact]
        public void organisation_get_json_with_link_title_test()
        {
            // arrange
            var mediaFormatter = new JsonHalMediaTypeOutputFormatter(
                new JsonSerializerSettings { Formatting = Formatting.Indented }, ArrayPool<char>.Shared);
            var resourceWithAppPath = new OrganisationWithLinkTitleRepresentation(1, "Org Name");

            // act
            using (var stream = new StringWriter())
            {
                mediaFormatter.WriteObject(stream, resourceWithAppPath);

                string serialisedResult = stream.ToString();

                // assert
                this.Assent(serialisedResult);
            }
        }

        [Fact]
        public void organisation_get_xml_test()
        {
            // arrange
            var mediaFormatter = new XmlHalMediaTypeOutputFormatter();

            // act
            using (var stream = new Utf8StringWriter())
            {
                mediaFormatter.WriteObject(stream, resource);

                string serialisedResult = stream.ToString();

                // assert
                this.Assent(serialisedResult);
            }
        } 
    }
}