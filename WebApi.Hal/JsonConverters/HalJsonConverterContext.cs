using System;
using WebApi.Hal.Proposed;

namespace WebApi.Hal.JsonConverters
{
    public class HalJsonConverterContext
    {
        readonly IHypermediaConfiguration hypermediaConfiguration;

        public HalJsonConverterContext()
        {
            IsRoot = true;
        }

        public HalJsonConverterContext(IHypermediaConfiguration hypermediaConfiguration) : this()
        {
            if (hypermediaConfiguration == null) 
                throw new ArgumentNullException("hypermediaConfiguration");

            this.hypermediaConfiguration = hypermediaConfiguration;
        }

        public IHypermediaConfiguration HypermediaConfiguration
        {
            get { return hypermediaConfiguration; }
        }

        public bool IsRoot { get; set; }
    }
}