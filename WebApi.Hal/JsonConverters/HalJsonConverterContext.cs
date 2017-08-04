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
            {
                throw new ArgumentNullException(nameof(hypermediaResolver));
            }

            this.HypermediaResolver = hypermediaResolver;
        }

        public IHypermediaResolver HypermediaResolver { get; }

        public bool IsRoot { get; set; }

        public static HalJsonConverterContext Create()
        {
            return new HalJsonConverterContext(Hypermedia.CreateResolver());
        }
    }
}