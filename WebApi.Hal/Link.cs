using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Http;
using System.Web.Http.Routing;

namespace WebApi.Hal
{
    public class Link
    {
        public Link()
        { }

        public Link(string rel, string href, bool isTemplated = false, string title = null)
        {
            Rel = rel;
            Href = href;
            Title = title;
            IsTemplated = isTemplated;
            if (href != null)
                IsTemplated = Regex.Match(href, @"{\w+}", RegexOptions.Compiled).Success;
        }

        public string Rel { get; set; }
        public string Href { get; set; }
        public string Title { get; set; }
        public bool IsTemplated { get; set; }

        /// <summary>
        /// If this link is templated, you can use this method to make a non templated copy
        /// </summary>
        /// <param name="newRel">A different rel</param>
        /// <param name="parameters">The parameters, i.e 'new {id = "1"}'</param>
        /// <returns>A non templated link</returns>
        public Link CreateLink(string newRel, object parameters)
        {
            return new Link(newRel, CreateUri(parameters).ToString());
        }

        /// <summary>
        /// If this link is templated, you can use this method to make a non templated copy
        /// </summary>
        /// <param name="parameters">The parameters, i.e 'new {id = "1"}'</param>
        /// <returns>A non templated link</returns>
        public Link CreateLink(object parameters)
        {
            return CreateLink(Rel, parameters);
        }

        public Uri CreateUri(object parameters)
        {
            var href = Href;
            foreach (var substitution in parameters.GetType().GetProperties())
            {
                var name = substitution.Name;
                var value = substitution.GetValue(parameters, null);
                var substituionValue = value == null ? null : Uri.EscapeDataString(value.ToString());
                href = href.Replace(string.Format("{{{0}}}", name), substituionValue, StringComparison.InvariantCultureIgnoreCase);
            }

            return new Uri(href, UriKind.Relative);
        }

        public void RegisterLinkWithWebApi<TController>(HttpRouteCollection routes, object defaults = null) where TController : ApiController
        {
            RegisterLinkWithWebApi(routes, typeof(TController).Name, defaults);
        }

        public void RegisterLinkWithWebApi(HttpRouteCollection routes, string controller, object defaults = null)
        {
            defaults = defaults ?? new { };
            var dictionary = defaults.GetType().GetProperties().ToDictionary(i => i.Name, i => i.GetValue(defaults, null));
            var strings = Href.TrimStart('/').Split('?', '#');
            if (strings.Length > 1)
            {
                var queryStringParts = strings[1];
                foreach (Match match in Regex.Matches(queryStringParts, @"{(?<variable>[^&]*?)=(?<default>.*?)}"))
                {
                    dictionary.Add(match.Groups["variable"].Value, match.Groups["default"].Value);
                }
            }
            var route = strings[0];
            dictionary.Add("controller", controller.Replace("Controller", string.Empty, StringComparison.InvariantCultureIgnoreCase));
            routes.Add(controller + "+" + Rel, new HttpRoute(route, new HttpRouteValueDictionary(dictionary)));
        }
    }
}