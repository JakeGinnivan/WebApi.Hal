using System;
using System.Linq;
using System.Linq.Expressions;
using WebApi.Hal.Web.Api.Resources;
using WebApi.Hal.Web.Models;

namespace WebApi.Hal.Web.Data.Queries
{
    /// <summary>
    /// Gets a list of beers, with no hypermedia on the resource
    /// </summary>
    public class GetBeersQuery : IPagedQuery<BeerResource>
    {
        readonly Expression<Func<Beer, bool>> where;

        public GetBeersQuery(Expression<Func<Beer, bool>> where = null)
        {
            this.where = where ?? (b=>true);
        }

        public PagedResult<BeerResource> Execute(IBeerContext context, int skip, int take)
        {
            var beers = context
                .Beers
                .Where(where)
                .OrderBy(b => b.Name)
                .Skip(skip)
                .Take(take)
                .Select(b => new BeerResource
                {
                    Id = b.Id,
                    Name = b.Name,
                    BreweryId = b.Brewery.Id,
                    BreweryName = b.Brewery.Name,
                    StyleId = b.Style.Id,
                    StyleName = b.Style.Name
                })
                .ToList();

            var count = context.Beers
                .Where(where)
                .Count();

            return new PagedResult<BeerResource>(beers, count, skip, take);
        }
    }
}