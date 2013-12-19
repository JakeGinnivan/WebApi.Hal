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

        public Link(string rel, string href, string title = null)
        {
            Rel = rel;
            Href = href;
            Title = title;
        }

        public string Rel { get; set; }
        public string Href { get; set; }
        public string Title { get; set; }
        public bool IsTemplated
        {
            get { return !string.IsNullOrEmpty(Href) && Regex.Match(Href, @"{.+}", RegexOptions.Compiled).Success; }
        }

        /// <summary>
        /// If this link is templated, you can use this method to make a non templated copy
        /// </summary>
        /// <param name="newRel">A different rel</param>
        /// <param name="parameters">The parameters, i.e 'new {id = "1"}'</param>
        /// <returns>A non templated link</returns>
        public Link CreateLink(string newRel, params object[] parameters)
        {
            return new Link(newRel, CreateUri(parameters).ToString());
        }

        /// <summary>
        /// If this link is templated, you can use this method to make a non templated copy
        /// </summary>
        /// <param name="parameters">The parameters, i.e 'new {id = "1"}'</param>
        /// <returns>A non templated link</returns>
        public Link CreateLink(params object[] parameters)
        {
            return CreateLink(Rel, parameters);
        }

        public Uri CreateUri(params object[] parameters)
        {
            var href = Href;
            href = SubstituteParams(href, parameters);

            return new Uri(href, UriKind.Relative);
        }

        public static string SubstituteParams(string href, params object[] parameters)
        {
            var uriTemplate = new UriTemplate(href);
            foreach (var parameter in parameters)
            {
                foreach (var substitution in parameter.GetType().GetProperties())
                {
                    var name = substitution.Name;
                    var value = substitution.GetValue(parameter, null);
                    var substituionValue = value == null ? null : value.ToString();
                    uriTemplate.SetParameter(name, substituionValue);
                }
            }

            return uriTemplate.Resolve();
        }

        [Obsolete("use ordinary MVC/WebApi machinery; HAL UriTemplates must comply with RFC6570, which is in conflict with route generation")]
        public void RegisterLinkWithWebApi<TController>(HttpRouteCollection routes, object defaults = null) where TController : ApiController
        {
            RegisterLinkWithWebApi(routes, typeof(TController).Name, defaults);
        }

        [Obsolete("use ordinary MVC/WebApi machinery; HAL UriTemplates must comply with RFC6570, which is in conflict with route generation")]
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