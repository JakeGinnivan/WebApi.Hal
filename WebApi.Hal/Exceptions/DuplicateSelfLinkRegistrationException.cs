using System;

namespace WebApi.Hal.Exceptions
{
    public class DuplicateSelfLinkRegistrationException : Exception
    {
        public DuplicateSelfLinkRegistrationException(Type type) : base("Configuration already contains a self link registration for " + type.Name)
        {
        }
    }
}