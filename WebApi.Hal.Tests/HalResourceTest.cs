using System;
using System.Buffers;
using System.IO;
using System.Linq;
using Assent;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.ObjectPool;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        public void organisation_get_json_with_app_path_test()
        {
            // arrange
            var mediaFormatter = new JsonHalMediaTypeOutputFormatter(
                new JsonSerializerSettings { Formatting = Formatting.Indented }, ArrayPool<char>.Shared, new MvcOptions());
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
                new JsonSerializerSettings { Formatting = Formatting.Indented }, ArrayPool<char>.Shared, new MvcOptions());
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
                new JsonSerializerSettings { Formatting = Formatting.Indented }, ArrayPool<char>.Shared, new MvcOptions());
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

        [Fact]
        public void organisation_get_json_with_multi_link_linkrel()
        {
            // arrange
            var mediaFormatter = new JsonHalMediaTypeOutputFormatter(
                new JsonSerializerSettings { Formatting = Formatting.Indented }, ArrayPool<char>.Shared);
            var resourceWithAppPath = new OrganisationWithLinkTitleRepresentation(1, "Org Name");
            resourceWithAppPath.Links.Add(new Link("multi-rel-with-single-link", "~/api/organisations/test")
            {
                IsMultiLink = true
            });
            resourceWithAppPath.Links.Add(new Link("multi-rel-with-multiple-links", "~/api/organisations/test1")
            {
                IsMultiLink = true
            });
            resourceWithAppPath.Links.Add(new Link("multi-rel-with-multiple-links", "~/api/organisations/test2")
            {
                IsMultiLink = true
            });
            resourceWithAppPath.Links.Add(new Link("multi-rel-with-multiple-links-with-is-multilink-false", "~/api/organisations/test-f1"));
            resourceWithAppPath.Links.Add(new Link("multi-rel-with-multiple-links-with-is-multilink-false", "~/api/organisations/test-f2"));

            // act
            using (var stream = new StringWriter())
            {
                mediaFormatter.WriteObject(stream, resourceWithAppPath);

                string serialisedResult = stream.ToString();

                // assert
                this.Assent(serialisedResult);
            }
        }

        public class OrganisationRepresentationWithEmbeddedResources : OrganisationWithAppPathRepresentation
        {
            public OrganisationRepresentationWithEmbeddedResources(int id, string name) : base(id, name)
            {
            }

            public TestRepresentation[] EmbeddedMultiResource { get; set; }

            public TestRepresentation EmbeddedSingleResource { get; set; }
        }

        public class TestRepresentation : Representation
        {
        }

        [Fact]
        public void organisation_get_json_with_multi_link_embedded()
        {
            // arrange
            var mediaFormatter = new JsonHalMediaTypeOutputFormatter(
                new JsonSerializerSettings { Formatting = Formatting.Indented }, ArrayPool<char>.Shared);
            var org = new OrganisationRepresentationWithEmbeddedResources(1, "Org Name");

            org.EmbeddedSingleResource = new TestRepresentation
            {
                Rel = "single-resource",
                Href = "~/single"
            };
            org.EmbeddedMultiResource = new[] { new TestRepresentation
            {
                Rel = "multi-resource",
                Href = "~/multi"
            } };

            // act
            using (var stream = new StringWriter())
            {
                mediaFormatter.WriteObject(stream, org);

                string serialisedResult = stream.ToString();

                // assert
                this.Assent(serialisedResult);
            }
        }

        private class JsonHalMediaTypeInputFormatterWithCreateSerializer : JsonHalMediaTypeInputFormatter
        {
            public JsonHalMediaTypeInputFormatterWithCreateSerializer(ILogger logger, JsonSerializerSettings serializerSettings, ArrayPool<char> charPool, ObjectPoolProvider objectPoolProvider, MvcOptions mvcOptions, MvcJsonOptions mvcJsonOptions) 
                : base(logger, serializerSettings, charPool, objectPoolProvider, mvcOptions, mvcJsonOptions)
            {
            }

            public new JsonSerializer CreateJsonSerializer()
            {
                return base.CreateJsonSerializer();
            }
        }

        [Fact]
        public void organisation_parse_json_with_multi_link_linkrel()
        {
            // arrange
            var mediaFormatter = new JsonHalMediaTypeOutputFormatter(
                new JsonSerializerSettings { Formatting = Formatting.Indented }, ArrayPool<char>.Shared);
            var resourceWithAppPath = new OrganisationWithLinkTitleRepresentation(1, "Org Name");
            resourceWithAppPath.Links.Add(new Link("multi-rel-with-single-link", "~/api/organisations/test")
            {
                IsMultiLink = true
            });
            resourceWithAppPath.Links.Add(new Link("multi-rel-with-multiple-links", "~/api/organisations/test1")
            {
                IsMultiLink = true
            });
            resourceWithAppPath.Links.Add(new Link("multi-rel-with-multiple-links", "~/api/organisations/test2")
            {
                IsMultiLink = true
            });
            resourceWithAppPath.Links.Add(new Link("multi-rel-with-multiple-links-with-is-multilink-false", "~/api/organisations/test1"));
            resourceWithAppPath.Links.Add(new Link("multi-rel-with-multiple-links-with-is-multilink-false", "~/api/organisations/test2"));

            string serialisedResource;
            // serialize
            using (var stream = new StringWriter())
            {
                mediaFormatter.WriteObject(stream, resourceWithAppPath);

                serialisedResource = stream.ToString();
            }

            // parse again
            var inputFormatter = new JsonHalMediaTypeInputFormatterWithCreateSerializer(
                NullLogger.Instance,
                new JsonSerializerSettings { Formatting = Formatting.Indented }, ArrayPool<char>.Shared,
                new DefaultObjectPoolProvider(), new MvcOptions(), new MvcJsonOptions());
            var inputSerializer = inputFormatter.CreateJsonSerializer();
            using (var stream = new StringReader(serialisedResource))
            {
                using (var jsonReader = new JsonTextReader(stream))
                {
                    var parsedResource = inputSerializer.Deserialize<OrganisationWithLinkTitleRepresentation>(jsonReader);
                    Assert.True(parsedResource.Links.Where(l => l.Rel == "multi-rel-with-single-link").All(l => l.IsMultiLink));
                    Assert.True(parsedResource.Links.Where(l => l.Rel == "multi-rel-with-multiple-links").All(l => l.IsMultiLink));
                    Assert.True(parsedResource.Links.Where(l => l.Rel == "multi-rel-with-multiple-links-with-is-multilink-false").All(l => l.IsMultiLink));
                    Assert.True(parsedResource.Links.Where(l => l.Rel == "someRel").All(l => !l.IsMultiLink));
                }
            }
        }
    }
}
