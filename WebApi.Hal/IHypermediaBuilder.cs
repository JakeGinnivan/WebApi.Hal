using WebApi.Hal.Interfaces;

namespace WebApi.Hal
{
    public interface IHypermediaBuilder
    {
        void RegisterAppender<T>(IHypermediaAppender<T> appender) where T : class, IResource;
        void RegisterSelf<T>(Link link) where T : IResource;
        void RegisterSelf<T>(Link<T> link) where T : class, IResource;
        void RegisterLinks<T>(params Link[] links) where T : class, IResource;
        IHypermediaResolver Build();
    }
}