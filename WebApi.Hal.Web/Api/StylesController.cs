using System.Linq;
using Microsoft.AspNetCore.Mvc;
using WebApi.Hal.Web.Api.Resources;
using WebApi.Hal.Web.Data;

namespace WebApi.Hal.Web.Api
{
    [Route("[controller]")]
    public class StylesController : Controller
    {
        readonly IBeerDbContext beerDbContext;

        public StylesController(IBeerDbContext beerDbContext)
        {
            this.beerDbContext = beerDbContext;
        }

        [HttpGet]
        // GET styles
        public BeerStyleListRepresentation Get()
        {
            var beerStyles = beerDbContext.BeerStyles
                                          .ToList()
                                          .Select(s => new BeerStyleRepresentation
                                                       {
                                                           Id = s.Id,
                                                           Name = s.Name
                                                       })
                                          .ToList();

            return new BeerStyleListRepresentation(beerStyles);
        }

        [HttpGet("{id}")]
        // GET styles/5
        public IActionResult Get(int id)
        {
            var beerStyle = beerDbContext.BeerStyles.SingleOrDefault(s => s.Id == id);
            if (beerStyle == null)
                return NotFound();

            var beerStyleResource = new BeerStyleRepresentation
                                    {
                                        Id = beerStyle.Id,
                                        Name = beerStyle.Name
                                    };

            return Ok(beerStyleResource);
        }
    }
}