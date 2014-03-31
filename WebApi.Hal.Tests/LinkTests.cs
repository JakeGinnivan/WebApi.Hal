using System;
using Xunit;

namespace WebApi.Hal.Tests
{
    public class LinkTests
    {
        [Fact]
        public void createcuries_results_in_valid_curies_link()
        {
            Link curies;
            var operators = new[] { '+', ';', '/', '#', '&', '?', '.' };

            foreach (var o in operators)
            {
                curies = Link.CreateCuries("webapihal", "http://docs.webapihal.com/{" + o + "rel}");

                Assert.True(curies.IsCuries);
            }

            curies = Link.CreateCuries("webapihal", "http://docs.webapihal.com/{rel}");

            Assert.True(curies.IsCuries);
        }

        [Fact]
        public void manually_created_curies_link_results_in_valid_curies_link()
        {
            var curies = new Link
            {
                Name = "webapihal",
                Rel = "curies",
                Href = "http://docs.webapihal.com/{rel}"
            };

            Assert.True(curies.IsCuries);
        }

        [Fact]
        public void createcuries_with_invalid_template_throws()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                Link.CreateCuries("webapihal", "http://docs.webapihal.com/");    
            });
        }
    }
}
