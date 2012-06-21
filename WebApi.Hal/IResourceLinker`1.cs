namespace WebApi.Hal
{
    public interface IResourceLinker<in T>
    {
        void CreateLinks(T resource, IResourceLinker resourceLinker);
    }
}