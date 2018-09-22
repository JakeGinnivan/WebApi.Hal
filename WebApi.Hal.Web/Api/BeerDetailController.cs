using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Hal.Web.Api.Resources;
using WebApi.Hal.Web.Data;

namespace WebApi.Hal.Web.Api
{
    [Route("[controller]")]
    [ApiController]
    [Produces("application/hal+json")]
    public class BeerDetailController : Controller
    {
        readonly IBeerDbContext beerDbContext;

        public BeerDetailController(IBeerDbContext beerDbContext)
        {
            this.beerDbContext = beerDbContext;
        }

        // GET beerdetail/5
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(BeerDetailRepresentation), (int)HttpStatusCode.OK)]
        public ActionResult<BeerDetailRepresentation> Get(int id)
        {
            var beer = beerDbContext.Beers
                                    .Include(b => b.Brewery) // lazy loading isn't on for this query; force loading
                                    .Include(b => b.Style)
                                    .Single(br => br.Id == id);

            var reviews = beerDbContext.Reviews
                                       .Where(r => r.Beer_Id == id)
                                       .ToList()
                                       .Select(s => new ReviewRepresentation
                                                    {
                                                        Id = s.Id,
                                                        Beer_Id = s.Beer_Id,
                                                        Title = s.Title,
                                                        Content = s.Content
                                                    })
                                       .ToList();

            var detail = new BeerDetailRepresentation
                         {
                             Id = beer.Id,
                             Name = beer.Name,
                             Style = new BeerStyleRepresentation {Id = beer.Style.Id, Name = beer.Style.Name},
                             Brewery = new BreweryRepresentation {Id = beer.Brewery.Id, Name = beer.Brewery.Name}
                         };

            if (reviews.Count > 0)
            {
                detail.Reviews = new List<ReviewRepresentation>();
                foreach (var review in reviews)
                    detail.Reviews.Add(review);
            }

            return detail;
        }

        // PUT beerdetail/5
        [HttpPut("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public void Put(int id, BeerDetailRepresentation beer)
        {
            // this is here just to see how the deserializer is working
            // we should get the links and all the embedded objects deserialized
            // we'd be better off creating a client to test the full deserializing, but this way is cheap for now
        }

        [HttpGet("largeset")]
        [ProducesResponseType(typeof(BeerDetailListRepresentation), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<BeerDetailListRepresentation>> GetLargeSet(int setSize = 500)
        {
            var random = new Random();
            var largeSet = new BeerDetailRepresentation[setSize];
            Parallel.For(0, setSize, index =>
            {
                largeSet[index] = new BeerDetailRepresentation
                {
                    Id = index + 1,
                    Name = $"Test beer name {Guid.NewGuid()}",
                    Reviews = new List<ReviewRepresentation>(),
                    Style = new BeerStyleRepresentation
                    {
                        Id = random.Next(1, 50),
                        Name = $"Test beer style name {Guid.NewGuid()}"
                    },
                    Brewery = new BreweryRepresentation
                    {
                        Id = random.Next(1, 10),
                        Name = $"Test brewery name {Guid.NewGuid()}"
                    }
                };
                var numberOfReviews = random.Next(2, 20);
                for (var reviewIndex = 0; reviewIndex < numberOfReviews; reviewIndex++)
                {
                    largeSet[index].Reviews.Add(new ReviewRepresentation
                    {
                        Id = random.Next(setSize * 10, setSize * 100) + largeSet[index].Id,
                        Content = $"Test beer review content {Guid.NewGuid()}",
                        Title = $"Test beer review title {Guid.NewGuid()}",
                        Beer_Id = largeSet[index].Id
                    });
                }
            });

            await Task.CompletedTask;
            return new BeerDetailListRepresentation(largeSet, largeSet.Length, 1, 1, LinkTemplates.BeerDetails.GetBeerDetail);
        }
    }
}