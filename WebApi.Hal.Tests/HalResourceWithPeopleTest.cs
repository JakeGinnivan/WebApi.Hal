using System.Buffers;
using System.IO;
using Assent;
using Microsoft.AspNetCore.Mvc;
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
        public void organisation_get_json_test()
        {
            // arrange
            var mediaFormatter = new JsonHalMediaTypeOutputFormatter(
                new JsonSerializerSettings { Formatting = Formatting.Indented }, ArrayPool<char>.Shared, new MvcOptions());

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
