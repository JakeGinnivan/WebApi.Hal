using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using ApprovalTests;
using ApprovalTests.Reporters;
using WebApi.Hal.Tests.Representations;
using Xunit;

namespace WebApi.Hal.Tests
{
    public class HalResourceMixedContentTest
    {
        readonly OrganisationWithPeopleDetailRepresentation resource;

        public HalResourceMixedContentTest()
        {
            resource = new OrganisationWithPeopleDetailRepresentation(1, "Org Name")
            {
                Boss = new Boss(2, "Eunice PHB", 1, true)
            };
            resource.People.Add(new Person(3, "Dilbert", 1));
            resource.People.Add(new Person(4, "Wally", 1));
            resource.People.Add(new Person(5, "Alice", 1));
        }

        [Fact]
        [UseReporter(typeof(DiffReporter))]
        public void peopledetail_get_json_test()
        {
            // arrange
            var mediaFormatter = new JsonHalMediaTypeFormatter { Indent = true };
            var content = new StringContent(string.Empty);
            var type = resource.GetType();

            // act
            using (var stream = new MemoryStream())
            {
                mediaFormatter.WriteToStreamAsync(type, resource, stream, content, null).Wait();
                stream.Seek(0, SeekOrigin.Begin);
                var serialisedResult = new StreamReader(stream).ReadToEnd();

                // assert
                Approvals.Verify(serialisedResult);
            }
        }

        [Fact]
        [UseReporter(typeof(DiffReporter))]
        public void peopledetail_get_xml_test()
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
                Approvals.Verify(serialisedResult);
            }
        }

        [Fact]
        public void peopledetail_post_json_props_test()
        {
            // arrange
            var mediaFormatter = new JsonHalMediaTypeFormatter { Indent = true };
            var type = typeof (OrganisationWithPeopleDetailRepresentation);
            const string json = @"
{
""Id"":""5"",
""Name"": ""Waterproof Fire Department""
}
";

            // act
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                var obj = mediaFormatter.ReadFromStreamAsync(type, stream, null, null).Result;

                // assert
                Assert.NotNull(obj);
                var org = obj as OrganisationWithPeopleDetailRepresentation;
                Assert.NotNull(org);
                Assert.Equal(5, org.Id);
                Assert.Equal("Waterproof Fire Department", org.Name);
            }
        }

        [Fact]
        public void peopledetail_post_json_links_test()
        {
            // arrange
            var mediaFormatter = new JsonHalMediaTypeFormatter { Indent = true };
            var type = typeof(OrganisationWithPeopleRepresentation);
            const string json = @"
{
""Id"":""3"",
""Name"": ""Dept. of Redundancy Dept."",
""_links"": {
 ""self"": {""href"": ""/api/organisations/3""},
 ""people"": {""href"": ""/api/organisations/3/people""},
 ""brownnoser"": [
   {""href"": ""/api/organisations/3/brown/1""},
   {""href"": ""/api/organisations/3/brown/2""}
        ]
    }
}
";

            // act
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                var obj = mediaFormatter.ReadFromStreamAsync(type, stream, null, null).Result;

                // assert
                Assert.NotNull(obj);
                var org = obj as OrganisationWithPeopleRepresentation;
                Assert.NotNull(org);
                Assert.Equal(4, org.Links.Count);
                var self = org.Links.Where(l => l.Rel == "self").ToList();
                Assert.Equal(1, self.Count);
                Assert.Equal("/api/organisations/3", self[0].Href);
                Assert.Equal(self[0].Href, org.Href);
                var people = org.Links.Where(l => l.Rel == "people").ToList();
                Assert.Equal(1, people.Count);
                Assert.Equal("/api/organisations/3/people", people[0].Href);
                var brownnosers = org.Links.Where(l => l.Rel == "brownnoser").ToList();
                Assert.Equal(2, brownnosers.Count);
                Assert.Equal("/api/organisations/3/brown/1", brownnosers[0].Href);
                Assert.Equal("/api/organisations/3/brown/2", brownnosers[1].Href);
            }
        }

        [Fact]
        public void peopledetail_post_json_embedded_singles_test()
        {
            // arrange
            var mediaFormatter = new JsonHalMediaTypeFormatter {Indent = true};
            var type = typeof(OrganisationWithPeopleDetailRepresentation);
            const string json = @"
{
""Id"":""3"",
""Name"": ""Singles Dept."",
""_embedded"": {
 ""person"": {""Id"": ""7"",""Name"": ""Person Seven"",""OrganisationId"": ""3"",
    ""_links"": {""self"": {""href"": ""/api/organisations/3/people/7""}}},
 ""boss"": {""Id"": ""8"",""Name"": ""Person Eight"",""OrganisationId"": ""3"",""HasPointyHair"":""true"",
    ""_links"": {""self"": {""href"": ""/api/organisations/3/boss""}}}
  }
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
                var org = obj as OrganisationWithPeopleDetailRepresentation;
                Assert.NotNull(org);
                Assert.NotNull(org.Boss);
                Assert.Equal(1, org.People.Count);
                Assert.Equal(1, org.Boss.Links.Count);
            }
        }

        [Fact]
        public void peopledetail_post_json_embedded_arrays_test()
        {
            // arrange
            var mediaFormatter = new JsonHalMediaTypeFormatter { Indent = true };
            var type = typeof(OrganisationWithPeopleDetailRepresentation);
            const string json = @"
{
""Id"":""3"",
""Name"": ""Array Dept."",
""_embedded"": {
 ""person"": [
   {""Id"": ""7"",""Name"": ""Person Seven"",""OrganisationId"": ""3"",
    ""_links"": {""self"": {""href"": ""/api/organisations/3/people/7""}}},
   {""Id"": ""9"",""Name"": ""Person Nine"",""OrganisationId"": ""3"",
    ""_links"": {""self"": {""href"": ""/api/organisations/3/people/9""}}}
   ],
 ""boss"": [{""Id"": ""8"",""Name"": ""Person Eight"",""OrganisationId"": ""3"",""HasPointyHair"":""true"",
    ""_links"": {""self"": {""href"": ""/api/organisations/3/boss""}}}]
  }
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
                var org = obj as OrganisationWithPeopleDetailRepresentation;
                Assert.NotNull(org);
                Assert.NotNull(org.Boss);
                Assert.Equal(2, org.People.Count);
                Assert.Equal(1, org.Boss.Links.Count);
            }
        }

        class MySimpleList : SimpleListRepresentation<OrganisationRepresentation>
        {
            protected override void CreateHypermedia()
            {
            }

            public string SimpleData { get; set; }
        }

        [Fact]
        public void simplelist_post_json_test()
        {
            // arrange
            var mediaFormatter = new JsonHalMediaTypeFormatter { Indent = true };
            var type = typeof(MySimpleList);
            const string json = @"
{
""_embedded"": {
 ""organisation"": [
   {""Id"": ""7"",""Name"": ""Org Seven"",
    ""_links"": {""self"": {""href"": ""/api/organisations/7""}}},
   {""Id"": ""8"",""Name"": ""Org Eight"",
    ""_links"": {""self"": {""href"": ""/api/organisations/8""}}}
   ]},
""SimpleData"": ""simple string""
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
                var orgList = obj as MySimpleList;
                Assert.NotNull(orgList);
                Assert.Equal(2, orgList.ResourceList.Count);
                Assert.Equal(7, orgList.ResourceList[0].Id);
                Assert.Equal("Org Seven", orgList.ResourceList[0].Name);
                Assert.Equal("/api/organisations/7", orgList.ResourceList[0].Href);
                Assert.Equal(8, orgList.ResourceList[1].Id);
                Assert.Equal("Org Eight", orgList.ResourceList[1].Name);
                Assert.Equal("/api/organisations/8", orgList.ResourceList[1].Href);
                Assert.Equal("simple string", orgList.SimpleData);
            }
        }
    }
}
