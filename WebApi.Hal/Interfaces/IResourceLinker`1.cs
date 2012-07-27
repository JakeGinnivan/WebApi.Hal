namespace WebApi.Hal.Interfaces
{
    public interface IResourceLinker<in T>
    {
        void CreateLinks(T resource, IResourceLinker resourceLinker);
    }
}