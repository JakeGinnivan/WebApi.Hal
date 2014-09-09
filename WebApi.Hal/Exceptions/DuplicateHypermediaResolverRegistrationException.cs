using System;

namespace WebApi.Hal.Exceptions
{
    public class DuplicateHypermediaResolverRegistrationException : Exception
    {
        public DuplicateHypermediaResolverRegistrationException(Type type)
            : base("Configuration already contains a hypermedia resolver registration for " + type.Name)
        {
        }
    }
}