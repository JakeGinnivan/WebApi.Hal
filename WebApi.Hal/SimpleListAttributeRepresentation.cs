using System.Collections.Generic;
using WebApi.Hal;

namespace WebApi.Hal
{
    public abstract class SimpleListAttributeRepresentation<TRepresentation> : AttributeRepresentation
        where TRepresentation : AttributeRepresentation
    {
        [Rel("resourceList")]
        public virtual IList<TRepresentation> ResourceList { get; set; }

        protected SimpleListAttributeRepresentation() : base()
        {
            ResourceList = new List<TRepresentation>();
        }

        protected SimpleListAttributeRepresentation(IList<TRepresentation> list)
        {
            ResourceList = list;
        }

        protected internal override void CreateHypermedia()
        {
            this.SetRelOnRepresentationPropertiesUsingRelAttribute();
            base.CreateHypermedia();
        }
    }
}