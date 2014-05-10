using System.Collections.Generic;
using WebApi.Hal.Proposed;
using WebApi.Hal.Tests.Proposed.Representations;

namespace WebApi.Hal.Tests.Proposed.HypermediaAppenders
{
    public class CategoryRepresentationHypermediaAppender : HypermediaAppender<CategoryRepresentation>
    {
        public override void Append(IEnumerable<Link> configured)
        {
            foreach (var link in configured)
            {
                switch (link.Rel)
                {
                    case Link.RelForSelf:
                        Resource.Links.Add(link.CreateLink(new { id = Resource.Id }));
                        break;
                    default:
                        Resource.Links.Add(link); // append untouched ...
                        break;
                }
            }
        }
    }
}