using System.Collections.Generic;

namespace WebApi.Hal.Web.Data
{
    public interface IRepository
    {
        TEntity Get<TEntity>(object id) where TEntity : class;
        IEnumerable<TEntity> FindAll<TEntity>() where TEntity : class;
        IEnumerable<TEntity> Find<TEntity>(IQuery<TEntity> query) where TEntity : class;
        PagedResult<TEntity> Find<TEntity>(IPagedQuery<TEntity> query, int pageNumber, int itemsPerPage) where TEntity : class;
        TEntity FindFirst<TEntity>(IQuery<TEntity> query) where TEntity : class;
        TEntity FindFirstOrDefault<TEntity>(IQuery<TEntity> query) where TEntity : class;
        TEntity FindFirstOrDefault<TEntity>(IPagedQuery<TEntity> query) where TEntity : class;
        void Execute(ICommand command);
        void Add<TEntity>(TEntity entity) where TEntity : class;
        void Remove<TEntity>(TEntity entity) where TEntity : class;
    }
}