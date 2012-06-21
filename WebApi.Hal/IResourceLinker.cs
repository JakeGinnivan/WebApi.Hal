namespace WebApi.Hal
{
    public interface IResourceLinker
    {
        void CreateLinks<T>(T resource);
    }
}