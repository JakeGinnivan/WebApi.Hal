namespace HalWebApi
{
    public interface IResourceLinker
    {
        void CreateLinks<T>(T resource);
    }

    public interface IResourceLinker<in T>
    {
        void CreateLinks(T resource, IResourceLinker resourceLinker);
    }
}