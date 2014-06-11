using System;

namespace WebApi.Hal.Exceptions
{
    public class MissingCuriesLinkException : Exception
    {
        public MissingCuriesLinkException(string name) : base("Configuration does not contain a curies link registration for: " + name)
        {
        }
    }
}