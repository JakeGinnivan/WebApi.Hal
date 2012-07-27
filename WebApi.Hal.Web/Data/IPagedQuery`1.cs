namespace WebApi.Hal.Web.Data
{
    public interface IPagedQuery<T>
    {
        PagedResult<T> Execute(IBeerContext context, int skip, int take);
    }
}