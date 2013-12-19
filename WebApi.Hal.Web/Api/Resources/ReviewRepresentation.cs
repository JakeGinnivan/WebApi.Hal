using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Hal.Web.Api.Resources
{
    public class ReviewRepresentation : Representation
    {
        public ReviewRepresentation()
        {
            Rel = LinkTemplates.Reviews.GetBeerReview.CreateLink(new { id = Beer_Id, rid = Id }).Rel;
        }

        public int Id { get; set; }
        public int Beer_Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        protected override void CreateHypermedia()
        {
            Href = LinkTemplates.Reviews.GetBeerReview.CreateLink(new { id = Beer_Id, rid = Id }).Href;
            Links.Add(new Link { Href = Href, Rel = "self" });
            Links.Add(LinkTemplates.Beers.Beer.CreateLink(new {id = Beer_Id}));
        }
    }
}