namespace WebApi.Hal.Interfaces
{
    public interface IResourceLinker
    {
        T CreateLinks<T>(T resource);
    }
}