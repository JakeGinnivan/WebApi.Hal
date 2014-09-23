﻿using System.IO;
using System.Net.Http;
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
    }
}