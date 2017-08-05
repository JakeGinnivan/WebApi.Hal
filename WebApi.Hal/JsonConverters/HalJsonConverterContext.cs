using System;

namespace WebApi.Hal.JsonConverters
{
    public class HalJsonConverterContext
    {
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

            HypermediaResolver = hypermediaResolver;
        }

        public IHypermediaResolver HypermediaResolver { get; }

        public bool IsRoot { get; set; }

        public static HalJsonConverterContext Create()
        {
            return new HalJsonConverterContext(Hypermedia.CreateResolver());
        }
    }
}