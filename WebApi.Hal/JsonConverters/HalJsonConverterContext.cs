using System;

namespace WebApi.Hal.JsonConverters
{
    public class HalJsonConverterContext
    {
        readonly IHypermediaResolver hypermediaResolver;

        public HalJsonConverterContext()
        {
            IsRoot = true;
        }

        public HalJsonConverterContext(IHypermediaResolver hypermediaResolver) : this()
        {
            if (hypermediaResolver == null) 
                throw new ArgumentNullException("hypermediaResolver");

            this.hypermediaResolver = hypermediaResolver;
        }

        public IHypermediaResolver HypermediaResolver
        {
            get { return hypermediaResolver; }
        }

        public bool IsRoot { get; set; }
    }
}