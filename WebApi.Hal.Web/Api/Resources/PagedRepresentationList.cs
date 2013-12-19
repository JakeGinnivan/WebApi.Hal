using System.Collections.Generic;

namespace WebApi.Hal.Web.Api.Resources
{
    public abstract class PagedRepresentationList<TRepresentation> : SimpleListRepresentation<TRepresentation> where TRepresentation : Representation
    {
        readonly Link uriTemplate;

        protected PagedRepresentationList(IList<TRepresentation> res, int totalResults, int totalPages, int page, Link uriTemplate, object uriTemplateSubstitutionParams)
            : base(res)
        {
            this.uriTemplate = uriTemplate;
            TotalResults = totalResults;
            TotalPages = totalPages;
            Page = page;
            UriTemplateSubstitutionParams = uriTemplateSubstitutionParams;
        }

        public int TotalResults { get; set; }
        public int TotalPages { get; set; }
        public int Page { get; set; }

        protected object UriTemplateSubstitutionParams;

        protected override void CreateHypermedia()
        {
            var prms = new List<object> { new { page = Page } };
            if (UriTemplateSubstitutionParams != null)
                prms.Add(UriTemplateSubstitutionParams);

            Href = Href ?? uriTemplate.CreateLink(prms.ToArray()).Href;

            Links.Add(new Link { Href = Href, Rel = "self" });

            if (Page > 1)
            {
                var item = UriTemplateSubstitutionParams == null
                                ? uriTemplate.CreateLink("prev", new { page = Page - 1 })
                                : uriTemplate.CreateLink("prev", UriTemplateSubstitutionParams, new { page = Page - 1 }); // page overrides UriTemplateSubstitutionParams
                Links.Add(item);
            } 
            if (Page < TotalPages)
            {
                var link = UriTemplateSubstitutionParams == null // kbr
                               ? uriTemplate.CreateLink("next", new { page = Page + 1 })
                               : uriTemplate.CreateLink("next", UriTemplateSubstitutionParams, new { page = Page + 1 }); // page overrides UriTemplateSubstitutionParams
                Links.Add(link);
            }
            Links.Add(new Link("page", uriTemplate.Href));
        }
    }
}