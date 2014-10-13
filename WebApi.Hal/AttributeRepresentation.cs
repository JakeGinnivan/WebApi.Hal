using System.Linq;
using System.Reflection;
using WebApi.Hal;

namespace WebApi.Hal
{
    public class AttributeRepresentation : Representation
    {
        protected Link selfLink;
        protected string rel;
        private string href;

        public AttributeRepresentation()
        {
            this.SetRelOnRepresentationPropertiesUsingRelAttribute();

            selfLink = this.GetSelfLinkFromAttribute();
            rel = selfLink.Rel;

            var hrefValue = this.GetHrefKeyPropertyValue();
            if (hrefValue != null)
            {
                href = selfLink.CreateLink(new { id = hrefValue }).Href;
            }
            else
            {
                href = selfLink.CreateLink(new { }).Href;
            }
        }

        public override string Rel
        {
            get { return rel; }
            set { rel = value; }
        }

        public override string Href
        {
            get { return href; }
            set { href = value; }
        }

        protected internal override void CreateHypermedia()
        {
            this.BuildLinksFromAttributes();

            var hrefValue = this.GetHrefKeyPropertyValue();

            //remove and re-add the self link here so the template gets processed correctly.
            var defaultSelfLink = Links.FirstOrDefault(l => l.Rel == selfLink.Rel && l.Href == selfLink.Href);
            if (defaultSelfLink != null)
                Links.Remove(defaultSelfLink);

            if (Links.All(l => l.Rel.ToLower() != "self"))
                Links.Add(new Link("self", selfLink.Href, selfLink.Title).CreateLink(new { id = hrefValue }));


            this.SetRelOnRepresentationPropertiesUsingRelAttribute();
        }

    }
}