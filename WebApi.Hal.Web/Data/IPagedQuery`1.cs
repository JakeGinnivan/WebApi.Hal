namespace WebApi.Hal.Web.Data
{
    public interface IPagedQuery<T>
    {
        PagedResult<T> Execute(IBeerDbContext dbContext, int skip, int take);
    }
}