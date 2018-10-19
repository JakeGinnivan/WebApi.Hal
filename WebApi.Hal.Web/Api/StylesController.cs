using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using WebApi.Hal.Web.Api.Resources;
using WebApi.Hal.Web.Data;

namespace WebApi.Hal.Web.Api
{
    [Route("[controller]")]
    [ApiController]
    [Produces("application/hal+json")]
    public class StylesController : ControllerBase
    {
        readonly IBeerDbContext beerDbContext;

        public StylesController(IBeerDbContext beerDbContext)
        {
            this.beerDbContext = beerDbContext;
        }

        // GET styles
        [HttpGet]
        [ProducesResponseType(typeof(BeerStyleListRepresentation), (int)HttpStatusCode.OK)]
        public ActionResult<BeerStyleListRepresentation> Get()
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

        // GET styles/5
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(BeerStyleRepresentation), (int)HttpStatusCode.OK)]
        public ActionResult<BeerStyleRepresentation> Get(int id)
        {
            var beerStyle = beerDbContext.BeerStyles.SingleOrDefault(s => s.Id == id);
            if (beerStyle == null)
                return NotFound();

            var beerStyleResource = new BeerStyleRepresentation
                                    {
                                        Id = beerStyle.Id,
                                        Name = beerStyle.Name
                                    };

            return beerStyleResource;
        }
    }
}