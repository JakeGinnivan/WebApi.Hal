using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ApprovalTests;
using ApprovalTests.Reporters;
using WebApi.Hal.Tests.Representations;
using Xunit;

namespace WebApi.Hal.Tests
{
    public class HalAttributeResourceTests
    {
        readonly OrgWithPeopleDetailAttributeRepresentation resource;

        public HalAttributeResourceTests()
        {
            resource = new OrgWithPeopleDetailAttributeRepresentation(1, "Org Name")
            {
                Boss = new Manager(2, "Eunice PHB", 1, true)
            };
            resource.People.Add(new Employee(3, "Dilbert", 1));
            resource.People.Add(new Employee(4, "Wally", 1));
            resource.People.Add(new Employee(5, "Alice", 1));
        }

        [Fact]
        [UseReporter(typeof(DiffReporter))]
        public void attribute_peopledetail_get_json_test()
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
        public void attribute_peopledetail_get_xml_test()
        {
            //TODO: modify the XmlHalMediaTypeFormatter so it sets the rels of embedded objects in the same manner as the JsonHalMediaTypeFormatter.
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
        public void attribute_peopledetail_post_json_props_test()
        {
            // arrange
            var mediaFormatter = new JsonHalMediaTypeFormatter { Indent = true };
            var type = typeof (OrgWithPeopleDetailAttributeRepresentation);
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
                var org = obj as OrgWithPeopleDetailAttributeRepresentation;
                Assert.NotNull(org);
                Assert.Equal(5, org.Id);
                Assert.Equal("Waterproof Fire Department", org.Name);
            }
        }

        [Fact]
        public void attribute_peopledetail_post_json_links_test()
        {
            // arrange
            var mediaFormatter = new JsonHalMediaTypeFormatter { Indent = true };
            var type = typeof(OrgWithPeopleDetailAttributeRepresentation);
            const string json = @"
{
  ""Id"": 1,
  ""Name"": ""Org Name"",
  ""_links"": {
    ""employees"": [
      {
        ""href"": ""/orgs/management/1/2"",
        ""title"": ""Employees under Boss""
      },
      {
        ""href"": ""/orgs/employees/1"",
        ""title"": ""Employees in this Org""
      }
    ],
    ""boss"": {
      ""href"": ""/orgs/management/2"",
      ""title"": ""The Boss for this Org""
    },
    ""self"": {
      ""href"": ""/orgs/1""
    }
  },
  ""_embedded"": {
    ""people"": [
      {
        ""Id"": 3,
        ""Name"": ""Dilbert"",
        ""OrganisationId"": 1,
        ""_links"": {
          ""self"": {
            ""href"": ""/employees/3""
          }
        }
      },
      {
        ""Id"": 4,
        ""Name"": ""Wally"",
        ""OrganisationId"": 1,
        ""_links"": {
          ""self"": {
            ""href"": ""/employees/4""
          }
        }
      },
      {
        ""Id"": 5,
        ""Name"": ""Alice"",
        ""OrganisationId"": 1,
        ""_links"": {
          ""self"": {
            ""href"": ""/employees/5""
          }
        }
      }
    ],
    ""boss"": {
      ""HasPointyHair"": true,
      ""Id"": 2,
      ""Name"": ""Eunice PHB"",
      ""OrganisationId"": 1,
      ""_links"": {
        ""self"": {
          ""href"": ""/managers/2""
        }
      }
    }
  }
}";

            // act
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                var obj = mediaFormatter.ReadFromStreamAsync(type, stream, null, null).Result;

                // assert
                Assert.NotNull(obj);
                var org = obj as OrgWithPeopleDetailAttributeRepresentation;
                Assert.NotNull(org);
                Assert.Equal(4, org.Links.Count);
                var self = org.Links.Where(l => l.Rel == "self").ToList();
                Assert.Equal(1, self.Count);
                Assert.Equal("/orgs/1", self[0].Href);
                Assert.Equal(self[0].Href, org.Href);
                var people = org.Links.Where(l => l.Rel == "employees").ToList();
                Assert.Equal(2, people.Count);
                Assert.Equal("/orgs/management/1/2", people[0].Href);
                Assert.Equal("/orgs/employees/1", people[1].Href);
                var boss = org.Links.Where(l => l.Rel == "boss").ToList();
                Assert.Equal(1, boss.Count);
                Assert.Equal("/orgs/management/2", boss[0].Href);
            }
        }

        [Fact]
        public void attribute_peopledetail_post_json_embedded_singles_test()
        {
            // arrange
            var mediaFormatter = new JsonHalMediaTypeFormatter {Indent = true};
            var type = typeof(OrgWithPeopleDetailAttributeRepresentation);
            const string json = @"
{
  ""Id"": 1,
  ""Name"": ""Org Name"",
  ""_links"": {
    ""employees"": [
      {
        ""href"": ""/orgs/management/1/2"",
        ""title"": ""Employees under Boss""
      },
      {
        ""href"": ""/orgs/employees/1"",
        ""title"": ""Employees in this Org""
      }
    ],
    ""boss"": {
      ""href"": ""/orgs/management/2"",
      ""title"": ""The Boss for this Org""
    },
    ""self"": {
      ""href"": ""/orgs/1""
    }
  },
  ""_embedded"": {
    ""people"": [
      {
        ""Id"": 3,
        ""Name"": ""Dilbert"",
        ""OrganisationId"": 1,
        ""_links"": {
          ""self"": {
            ""href"": ""/employees/3""
          }
        }
      },
      {
        ""Id"": 4,
        ""Name"": ""Wally"",
        ""OrganisationId"": 1,
        ""_links"": {
          ""self"": {
            ""href"": ""/employees/4""
          }
        }
      },
      {
        ""Id"": 5,
        ""Name"": ""Alice"",
        ""OrganisationId"": 1,
        ""_links"": {
          ""self"": {
            ""href"": ""/employees/5""
          }
        }
      }
    ],
    ""boss"": {
      ""HasPointyHair"": true,
      ""Id"": 2,
      ""Name"": ""Eunice PHB"",
      ""OrganisationId"": 1,
      ""_links"": {
        ""self"": {
          ""href"": ""/managers/2""
        }
      }
    }
  }
}";

            // act
            using (
                var stream = new MemoryStream(Encoding.UTF8.GetBytes(json))
                )
            {
                var obj = mediaFormatter.ReadFromStreamAsync(type, stream, null, null).Result;

                // assert
                Assert.NotNull(obj);
                var org = obj as OrgWithPeopleDetailAttributeRepresentation;
                Assert.NotNull(org);
                Assert.NotNull(org.Boss);
                Assert.Equal(3, org.People.Count);
                Assert.Equal(1, org.Boss.Links.Count);
            }
        }

        [Fact]
        public void attribute_peopledetail_post_json_embedded_arrays_test()
        {
            // arrange
            var mediaFormatter = new JsonHalMediaTypeFormatter { Indent = true };
            var type = typeof(OrgWithPeopleDetailAttributeRepresentation);
            const string json = @"
{
  ""Id"": 1,
  ""Name"": ""Org Name"",
  ""_links"": {
    ""employees"": [
      {
        ""href"": ""/orgs/management/1/2"",
        ""title"": ""Employees under Boss""
      },
      {
        ""href"": ""/orgs/employees/1"",
        ""title"": ""Employees in this Org""
      }
    ],
    ""boss"": {
      ""href"": ""/orgs/management/2"",
      ""title"": ""The Boss for this Org""
    },
    ""self"": {
      ""href"": ""/orgs/1""
    }
  },
  ""_embedded"": {
    ""people"": [
      {
        ""Id"": 3,
        ""Name"": ""Dilbert"",
        ""OrganisationId"": 1,
        ""_links"": {
          ""self"": {
            ""href"": ""/employees/3""
          }
        }
      },
      {
        ""Id"": 4,
        ""Name"": ""Wally"",
        ""OrganisationId"": 1,
        ""_links"": {
          ""self"": {
            ""href"": ""/employees/4""
          }
        }
      },
      {
        ""Id"": 5,
        ""Name"": ""Alice"",
        ""OrganisationId"": 1,
        ""_links"": {
          ""self"": {
            ""href"": ""/employees/5""
          }
        }
      }
    ],
    ""boss"": {
      ""HasPointyHair"": true,
      ""Id"": 2,
      ""Name"": ""Eunice PHB"",
      ""OrganisationId"": 1,
      ""_links"": {
        ""self"": {
          ""href"": ""/managers/2""
        }
      }
    }
  }
}";

            // act
            using (
                var stream = new MemoryStream(Encoding.UTF8.GetBytes(json))
                )
            {
                var obj = mediaFormatter.ReadFromStreamAsync(type, stream, null, null).Result;

                // assert
                Assert.NotNull(obj);
                var org = obj as OrgWithPeopleDetailAttributeRepresentation;
                Assert.NotNull(org);
                Assert.NotNull(org.Boss);
                Assert.Equal(3, org.People.Count);
                Assert.Equal(1, org.Boss.Links.Count);
            }
        }

        class MySimpleAttributeList : SimpleListRepresentation<OrgWithPeopleDetailAttributeRepresentation>
        {
            protected override void CreateHypermedia()
            {
            }

            [Rel("organisation")]
            public override IList<OrgWithPeopleDetailAttributeRepresentation> ResourceList { get; set; }

            public string SimpleData { get; set; }
        }

        [Fact]
        public void attribute_simplelist_post_json_test()
        {
            // arrange
            var mediaFormatter = new JsonHalMediaTypeFormatter { Indent = true };
            var type = typeof(HalAttributeResourceTests.MySimpleAttributeList);
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
}";

            // act
            using (
                var stream = new MemoryStream(Encoding.UTF8.GetBytes(json))
                )
            {
                var obj = mediaFormatter.ReadFromStreamAsync(type, stream, null, null).Result;

                // assert
                Assert.NotNull(obj);
                var orgList = obj as HalAttributeResourceTests.MySimpleAttributeList;
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
