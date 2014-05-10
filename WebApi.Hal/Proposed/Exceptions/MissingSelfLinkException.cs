using System;

namespace WebApi.Hal.Proposed.Exceptions
{
    public class MissingSelfLinkException : Exception
    {
        public MissingSelfLinkException(Type type)
            : base("Configuration does not contain a self link registration for: " + type.Name)
        {
        }
    }
}