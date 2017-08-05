using System.Buffers;
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
    public class HalResourceTest
    {
        readonly OrganisationRepresentation resource;

        public HalResourceTest()
        {
            resource = new OrganisationRepresentation(1, "Org Name");
        }

        [Fact]
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
                using (var stream1 = new MemoryStream())
                {
                    var context = new DefaultHttpContext();
                    context.Response.Body = stream1;

                    await mediaFormatter.WriteResponseBodyAsync(
                        new Microsoft.AspNetCore.Mvc.Formatters.OutputFormatterWriteContext(
                            context,
                            (writeStream, effectiveEncoding) => new StreamWriter(writeStream, effectiveEncoding),
                            type,
                            resource), Encoding.UTF8);

                    await context.Response.Body.CopyToAsync(stream);
                }

                stream.Seek(0, SeekOrigin.Begin);
                var serialisedResult = new StreamReader(stream).ReadToEnd();

                // assert
                this.Assent(serialisedResult);
            }
        }

        [Fact]
        public async Task organisation_get_json_with_app_path_test()
        {
            // arrange
            var mediaFormatter = new JsonHalMediaTypeOutputFormatter(
                new JsonSerializerSettings(), ArrayPool<char>.Shared);
            var content = new StringContent(string.Empty);
            var resourceWithAppPath = new OrganisationWithAppPathRepresentation(1, "Org Name");
            var type = resourceWithAppPath.GetType();

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
                       resourceWithAppPath), Encoding.UTF8);

                stream.Seek(0, SeekOrigin.Begin);
                var serialisedResult = new StreamReader(stream).ReadToEnd();

                // assert
                this.Assent(serialisedResult);
            }
        }

        [Fact]
        public async Task organisation_get_json_with_no_href_test()
        {
            // arrange
            var mediaFormatter = new JsonHalMediaTypeOutputFormatter(
                new JsonSerializerSettings(), ArrayPool<char>.Shared);
            var content = new StringContent(string.Empty);
            var resourceWithAppPath = new OrganisationWithNoHrefRepresentation(1, "Org Name");
            var type = resourceWithAppPath.GetType();

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
                      resourceWithAppPath), Encoding.UTF8);

                stream.Seek(0, SeekOrigin.Begin);
                var serialisedResult = new StreamReader(stream).ReadToEnd();

                // assert
                this.Assent(serialisedResult);
            }
        }

        [Fact]
        public async Task organisation_get_json_with_link_title_test()
        {
            // arrange
            var mediaFormatter = new JsonHalMediaTypeOutputFormatter(
                new JsonSerializerSettings(), ArrayPool<char>.Shared);
            var content = new StringContent(string.Empty);
            var resourceWithAppPath = new OrganisationWithLinkTitleRepresentation(1, "Org Name");
            var type = resourceWithAppPath.GetType();

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
                      resourceWithAppPath), Encoding.UTF8);

                stream.Seek(0, SeekOrigin.Begin);
                var serialisedResult = new StreamReader(stream).ReadToEnd();

                // assert
                this.Assent(serialisedResult);
            }
        }

        [Fact]
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
                this.Assent(serialisedResult);
            }
        } 
    }
}