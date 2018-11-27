using System.Collections;

namespace WebApi.Hal
{
    public interface IResourceList : IEnumerable
    {
        string RelationName { get; }
    }
}