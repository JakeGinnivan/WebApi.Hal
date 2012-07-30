using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using ApprovalTests;
using ApprovalTests.Reporters;
using WebApi.Hal.Tests.Representations;
using Xunit;

namespace WebApi.Hal.Tests
{
    public class HalResourceListTests
    {
        readonly RepresentationList<OrganisationRepresentation> representation;

        public HalResourceListTests()
        {
            representation = new OrganisationListRepresentation(
                new List<OrganisationRepresentation>
                       {
                           new OrganisationRepresentation(1, "Org1"),
                           new OrganisationRepresentation(2, "Org2")
                       });
        }

        [Fact]
        [UseReporter(typeof(DiffReporter))]
        public void organisation_list_get_xml_test()
        {
            // arrange
            var mediaFormatter = new XmlHalMediaTypeFormatter();
            var contentHeaders = new StringContent(string.Empty).Headers;
            var type = representation.GetType();

            // act
            using (var stream = new MemoryStream())
            {
                mediaFormatter.WriteToStream(type, representation, stream, contentHeaders);
                stream.Seek(0, SeekOrigin.Begin);
                var serialisedResult = new StreamReader(stream).ReadToEnd();

                // assert
                Approvals.Verify(serialisedResult);
            }
        }

        [Fact]
        [UseReporter(typeof(DiffReporter))]
        public void organisation_list_get_json_test()
        {
            // arrange
            var mediaFormatter = new JsonHalMediaTypeFormatter { Indent = true };
            var contentHeaders = new StringContent(string.Empty).Headers;
            var type = representation.GetType();

            // act
            using (var stream = new MemoryStream())
            {
                mediaFormatter.WriteToStreamAsync(type, representation, stream, contentHeaders, null).Wait();
                stream.Seek(0, SeekOrigin.Begin);
                var serialisedResult = new StreamReader(stream).ReadToEnd();

                // assert
                Approvals.Verify(serialisedResult);
            }
        }
    }
}