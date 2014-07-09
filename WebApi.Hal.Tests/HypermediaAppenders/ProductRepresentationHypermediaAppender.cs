using System.Collections.Generic;
using WebApi.Hal.Interfaces;
using WebApi.Hal.Tests.Representations;

namespace WebApi.Hal.Tests.HypermediaAppenders
{
    public class ProductRepresentationHypermediaAppender : IHypermediaAppender<ProductRepresentation>
    {
        public void Append(ProductRepresentation resource, IEnumerable<Link> configured)
        {
            foreach (var link in configured)
            {
                switch (link.Rel)
                {
                    case Link.RelForSelf:
                        resource.Links.Add(link.CreateLink(new { id = resource.Id }));
                        break;
                    case "example-namespace:category":
                        resource.Links.Add(link.CreateLink(new {id = "Action Figures"}));
                        break;
                    case "example-namespace:related-product":
                        for (var i = 0; i < 3; i++)
                            resource.Links.Add(link.CreateLink(new { id = string.Format("related-product-{0:00}", i) }));
                        break;
                    case "example-namespace:product-on-sale":
                        for (var i = 0; i < 3; i++)
                            resource.Links.Add(link.CreateLink(new { id = string.Format("product-on-sale-{0:00}", i) }));
                        break;
                    default:
                        resource.Links.Add(link); // append untouched ...
                        break;
                }
            }
        }
    }
}