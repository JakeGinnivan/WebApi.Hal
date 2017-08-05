using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using Assent;
using Newtonsoft.Json;
using WebApi.Hal.Tests.Representations;
using Xunit;

namespace WebApi.Hal.Tests
{
    public class HalResourceListTests
    {
        readonly OrganisationListRepresentation representation;

        readonly OrganisationListRepresentation oneitemrepresentation;

        public HalResourceListTests()
        {
            representation = new OrganisationListRepresentation(
                new List<OrganisationRepresentation>
                       {
                           new OrganisationRepresentation(1, "Org1"),
                           new OrganisationRepresentation(2, "Org2")
                       });

            oneitemrepresentation = new OrganisationListRepresentation(
                new List<OrganisationRepresentation>
                       {
                           new OrganisationRepresentation(1, "Org1")
                       });
        }

        [Fact]
        public void organisation_list_get_xml_test()
        {
            // arrange
            var mediaFormatter = new XmlHalMediaTypeOutputFormatter();

            // act
            using (var stream = new Utf8StringWriter())
            {
                mediaFormatter.WriteObject(stream, representation);

                string serialisedResult = stream.ToString();

                // assert
                this.Assent(serialisedResult);
            }
        }

        [Fact]
        public void organisation_list_get_json_test()
        {
            // arrange
            var mediaFormatter = new JsonHalMediaTypeOutputFormatter(
                new JsonSerializerSettings(), ArrayPool<char>.Shared);

            // act
            using (var stream = new StringWriter())
            {
                mediaFormatter.WriteObject(stream, representation);

                string serialisedResult = stream.ToString();

                // assert
                this.Assent(serialisedResult);
            }
        }

        [Fact]
        public void one_item_organisation_list_get_json_test()
        {
            // arrange
            var mediaFormatter = new JsonHalMediaTypeOutputFormatter(
                new JsonSerializerSettings(), ArrayPool<char>.Shared);
            var content = new StringContent(string.Empty);
            var type = oneitemrepresentation.GetType();

            // act
            using (var stream = new StringWriter())
            {
                mediaFormatter.WriteObject(stream, oneitemrepresentation);

                string serialisedResult = stream.ToString();

                // assert
                this.Assent(serialisedResult);
            }
        }
    }
}