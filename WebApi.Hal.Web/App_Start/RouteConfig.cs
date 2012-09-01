﻿using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebApi.Hal.Web.App_Start
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            /*routes.MapHttpRoute(
                name: "BeersFilterApi",
                routeTemplate: "api/{controller}/{id}/beers",
                defaults: new { id = RouteParameter.Optional, action = "AssociatedBeers" }
            );*/

            routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}