namespace WebApi.Hal
{
    public interface IResourceLinker
    {
        T CreateLinks<T>(T resource);
    }
}