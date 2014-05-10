using System;

namespace WebApi.Hal.Proposed.Exceptions
{
    public class DuplicateCurisLinkRegistrationException : Exception
    {
        public DuplicateCurisLinkRegistrationException(string name)
            : base("Configuration already containes a curies link with name: " + name)
        {
        }
    }
}