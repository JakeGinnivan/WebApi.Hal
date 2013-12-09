using System.Collections.Generic;

namespace WebApi.Hal.Web.Api.Resources
{
    public abstract class PagedRepresentationList<TRepresentation> : RepresentationList<TRepresentation> where TRepresentation : Representation
    {
        readonly Link uriTemplate;

        protected PagedRepresentationList(IList<TRepresentation> res, int totalResults, int totalPages, int page, Link uriTemplate)
            : base(res)
        {
            this.uriTemplate = uriTemplate;
            TotalResults = totalResults;
            TotalPages = totalPages;
            Page = page;
        }

        public int TotalResults { get; set; }
        public int TotalPages { get; set; }
        public int Page { get; set; }

        protected override void CreateHypermedia()
        {
            base.CreateHypermedia();

            Href = Href ?? uriTemplate.CreateLink(new {Page}).Href;
            Rel = Rel ?? uriTemplate.Rel;

            if (Page > 1)
            {
                var item = uriTemplate.CreateLink("prev", new { page = Page - 1 });
                item.IsTemplated = true;
                Links.Add(item);
            } 
            if (Page < TotalPages)
            {
                var link = uriTemplate.CreateLink("next", new{page = Page + 1});
                link.IsTemplated = true;
                Links.Add(link);
            }
            Links.Add(new Link("page", uriTemplate.Href, true));
        }
    }
}