using Microsoft.EntityFrameworkCore;
using WebApi.Hal.Web.Models;

namespace WebApi.Hal.Web.Data
{
    public class BeerDbContext : DbContext, IBeerDbContext
    {
        public BeerDbContext(DbContextOptions options) : base(options) {
        }

        public DbSet<Beer> Beers { get; set; }
        public DbSet<BeerStyle> Styles { get; set; }
        public DbSet<Brewery> Breweries { get; set; }
        public DbSet<Review> Reviews { get; set; }
    }
}