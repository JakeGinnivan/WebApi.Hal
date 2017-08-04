using System.Buffers;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ApprovalTests;
using ApprovalTests.Reporters;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using WebApi.Hal.Tests.Representations;
using Xunit;

namespace WebApi.Hal.Tests
{
    public class HalResourceWithPeopleTest
    {
        readonly OrganisationWithPeopleRepresentation resource;

        public HalResourceWithPeopleTest()
        {
            resource = new OrganisationWithPeopleRepresentation(1, "Org Name");
        }

        [Fact]
        [UseReporter(typeof(DiffReporter))]
        public async Task organisation_get_json_test()
        {
            // arrange
            var mediaFormatter = new JsonHalMediaTypeOutputFormatter(
                new JsonSerializerSettings(), ArrayPool<char>.Shared);
            var content = new StringContent(string.Empty);
            var type = resource.GetType();

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
                         resource), Encoding.UTF8);

                stream.Seek(0, SeekOrigin.Begin);
                var serialisedResult = new StreamReader(stream).ReadToEnd();

                // assert
                Approvals.Verify(serialisedResult);
            }
        }

        [Fact]
        [UseReporter(typeof(DiffReporter))]
        public async Task organisation_get_xml_test()
        {
            // arrange
            var mediaFormatter = new XmlHalMediaTypeOutputFormatter();

            var content = new StringContent(string.Empty);
            var type = resource.GetType();

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
                        resource));

                stream.Seek(0, SeekOrigin.Begin);
                var serialisedResult = new StreamReader(stream).ReadToEnd();

                // assert
                Approvals.Verify(serialisedResult);
            }
        } 
    }
}