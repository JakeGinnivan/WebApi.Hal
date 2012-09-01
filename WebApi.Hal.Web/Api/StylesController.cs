using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Filters;
using WebApi.Hal.JsonConverters;
using WebApi.Hal.Web.Api.Resources;
using WebApi.Hal.Web.Data;
using WebApi.Hal.Web.Data.Queries;
using WebApi.Hal.Web.Models;

namespace WebApi.Hal.Web.Api
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class LinkedResourceAttribute : Attribute
    {
        readonly string resourceId;

        public LinkedResourceAttribute(string resourceId)
        {
            this.resourceId = resourceId;
        }

        public string Resource
        {
            get { return resourceId; }
        }
    }

    

    public class HalFilter : ActionFilterAttribute
    {
        public static class Resource
        {
            public const string Self = "self";
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            if (!actionExecutedContext.Request.Headers.Accept.Any(h => h.MediaType.ToLower() == "application/hal+json"))
                return;

            var controller = actionExecutedContext.ActionContext.ControllerContext.Controller as IHalController;
            if (controller == null)
                return;

            var objectContent = actionExecutedContext.Response.Content as ObjectContent;
            if (objectContent == null)
                return;

            var attrs = actionExecutedContext.ActionContext.ActionDescriptor.GetCustomAttributes<LinkedResourceAttribute>();
            if (attrs == null || attrs.Count == 0)
                return;

            HypermediaContent newContent;
            var objectList = objectContent.Value as IEnumerable;
            if (objectList != null)
            {
                var newList = new ArrayList();
                newContent = new HypermediaContent(newList);
                foreach (var o in objectList)
                {
                    var newNewContent = new HypermediaContent(o);
                    var o1 = o;
                    attrs.Select(attr => controller.GetLinkForResource(attr.Resource, o1)).ToList()
                        .ForEach(a => newNewContent.Links.Add(a));
                    newList.Add(newNewContent);
                }
            }
            else
            {
                newContent = new HypermediaContent(objectContent.Value);
                var o = objectContent.Value;
                attrs.Select(attr => controller.GetLinkForResource(attr.Resource, o)).ToList()
                    .ForEach(a => newContent.Links.Add(a));
            }
            newContent.Links.Add(new Link(Resource.Self, actionExecutedContext.Request.RequestUri.AbsolutePath));
            actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(actionExecutedContext.Response.StatusCode, newContent);
        }
    }

    public interface IHalController
    {
        Link GetLinkForResource(string resourceId, object o);
    }

    public class StylesController : ApiController, IHalController
    {
        private static class Resource
        {
            public const string Self = HalFilter.Resource.Self;
            public const string Beers = "beers";
        }

        public Link GetLinkForResource(string resourceId, object o)
        {
            var beerStyle = o as BeerStyle;
            if (beerStyle == null)
                return null;

            switch (resourceId)
            {
                case Resource.Self:
                    return LinkTemplates.BeerStyles.Style.CreateLink(Resource.Self, _id => beerStyle.Id);
                case Resource.Beers:
                    return LinkTemplates.BeerStyles.AssociatedBeers.CreateLink(Resource.Beers, _id => beerStyle.Id);
            }

            return null;
        }

        readonly IBeerDbContext beerDbContext;
        readonly IRepository repository;

        public StylesController(IBeerDbContext beerDbContext, IRepository repository)
        {
            this.beerDbContext = beerDbContext;
            this.repository = repository;
        }

        [LinkedResource(Resource.Self)]
        [LinkedResource(Resource.Beers)]
        public List<BeerStyle> Get()
        {
            return beerDbContext.Styles.ToList();
        }

        [LinkedResource(Resource.Beers)]
        public HttpResponseMessage Get(int id)
        {
            var beerStyle = beerDbContext.Styles.SingleOrDefault(s => s.Id == id);
            if (beerStyle == null)
                return Request.CreateResponse(HttpStatusCode.NotFound);

            return Request.CreateResponse(HttpStatusCode.OK, beerStyle);
        }
        /*
        [HttpGet]
        public BeerListRepresentation AssociatedBeers(int id)
        {
            return AssociatedBeers(id, 1);
        }

        [HttpGet]
        public BeerListRepresentation AssociatedBeers(int id, int page)
        {
            var beers = repository.Find(new GetBeersQuery(b => b.Style.Id == id), page, BeersController.PageSize);

            var resourceList = new BeerListRepresentation(beers.ToList())
            {
                Total = beers.TotalResults
            };

            return resourceList;
        }
        */
    }
}