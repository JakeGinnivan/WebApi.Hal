using System;

namespace WebApi.Hal.Proposed.Exceptions
{
    public class DuplicateSelfLinkRegistrationException : Exception
    {
        public DuplicateSelfLinkRegistrationException(Type type) : base("Configuration already contains a self link registration for " + type.Name)
        {
        }
    }
}