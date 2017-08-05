using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Assent;
using Microsoft.AspNetCore.Http;
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
        public async Task organisation_list_get_xml_test()
        {
            // arrange
            var mediaFormatter = new XmlHalMediaTypeOutputFormatter();
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
                      representation));

                stream.Seek(0, SeekOrigin.Begin);
                var serialisedResult = new StreamReader(stream).ReadToEnd();

                // assert
                this.Assent(serialisedResult);
            }
        }

        [Fact]
        public async Task organisation_list_get_json_test()
        {
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