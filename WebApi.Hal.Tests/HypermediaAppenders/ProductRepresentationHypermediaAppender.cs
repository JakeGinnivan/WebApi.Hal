using System.Collections.Generic;
using WebApi.Hal.Tests.Representations;

namespace WebApi.Hal.Tests.HypermediaAppenders
{
    public class ProductRepresentationHypermediaAppender : HypermediaAppender<ProductRepresentation>
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
                    case "example-namespace:category":
                        Resource.Links.Add(link.CreateLink(new {id = "Action Figures"}));
                        break;
                    case "example-namespace:related-product":
                        for (var i = 0; i < 3; i++)
                            Resource.Links.Add(link.CreateLink(new { id = string.Format("related-product-{0:00}", i) }));
                        break;
                    case "example-namespace:product-on-sale":
                        for (var i = 0; i < 3; i++)
                            Resource.Links.Add(link.CreateLink(new { id = string.Format("product-on-sale-{0:00}", i) }));
                        break;
                    default:
                        Resource.Links.Add(link); // append untouched ...
                        break;
                }
            }
        }
    }
}