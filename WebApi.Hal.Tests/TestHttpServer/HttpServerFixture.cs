using System;
using System.Net.Http;
using System.Web.Http;

namespace WebApi.Hal.Tests.TestFixtures
{
    public class HttpServerFixture : IDisposable
    {
        public HttpServerFixture()
        {
            var config = new HttpConfiguration();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            config.Formatters.Insert(0, new JsonHalMediaTypeFormatter { Indent = true });

            Server = new HttpServer(config);

            Client = new HttpClient(Server);
            Client.BaseAddress = new Uri("http://www.test.com/");
        }

        public HttpClient Client { get; private set; }
        public HttpServer Server { get; private set; }

        public void Dispose()
        {
            Server.Dispose();
        }
    }
}