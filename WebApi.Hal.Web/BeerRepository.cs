using System.Collections.Generic;
using System.Linq;
using WebApi.Hal.Web.Data;

namespace WebApi.Hal.Web
{
    public class BeerRepository : IRepository
    {
        private readonly IBeerDbContext beerDbContext;

        public BeerRepository(IBeerDbContext beerDbContext)
        {
            this.beerDbContext = beerDbContext;
        }

        public TEntity Get<TEntity>(object id) where TEntity : class
        {
            return beerDbContext.Set<TEntity>().Find(id);
        }

        public IEnumerable<TEntity> FindAll<TEntity>() where TEntity : class
        {
            return beerDbContext.Set<TEntity>();
        }

        public IEnumerable<TEntity> Find<TEntity>(IQuery<TEntity> query) where TEntity : class
        {
            return query.Execute(beerDbContext);
        }

        public PagedResult<TEntity> Find<TEntity>(IPagedQuery<TEntity> query, int pageNumber, int itemsPerPage) where TEntity : class
        {
            return query.Execute(beerDbContext, (pageNumber - 1)*itemsPerPage, itemsPerPage);
        }

        public TEntity FindFirst<TEntity>(IQuery<TEntity> query) where TEntity : class
        {
            return query.Execute(beerDbContext).First();
        }

        public TEntity FindFirstOrDefault<TEntity>(IQuery<TEntity> query) where TEntity : class
        {
            return query.Execute(beerDbContext).FirstOrDefault();
        }

        public TEntity FindFirstOrDefault<TEntity>(IPagedQuery<TEntity> query) where TEntity : class
        {
            return query.Execute(beerDbContext, 0, 1).FirstOrDefault();
        }

        public void Execute(ICommand command)
        {
            command.Execute(beerDbContext);
        }

        public void Add<TEntity>(TEntity entity) where TEntity : class
        {
            beerDbContext.Set<TEntity>().Add(entity);
            beerDbContext.SaveChanges();
        }

        public void Remove<TEntity>(TEntity entity) where TEntity : class
        {
            beerDbContext.Set<TEntity>().Remove(entity);
            beerDbContext.SaveChanges();
        }
    }
}