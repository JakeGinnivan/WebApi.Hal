using System;

namespace WebApi.Hal.Exceptions
{
    public class MissingHypermediaBuilderException : Exception
    {
        public MissingHypermediaBuilderException(Type type)
            : base("Configuration does not contain a hypermedia builder registration for: " + type.Name)
        {
        }
    }
}