using System;
using System.Text.RegularExpressions;
using System.Web;

namespace WebApi.Hal
{
    public class Link
    {
        public Link()
        { }

        public Link(string rel, string href, bool isTemplated = false)
        {
            Rel = rel;
            Href = href;
            IsTemplated = isTemplated;
            if (href != null)
                IsTemplated = Regex.Match(href, @"{\w+}", RegexOptions.Compiled).Success;
        }

        public string Rel { get; set; }

        public string Href { get; set; }

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
                var substituionValue = value == null ? null : HttpUtility.UrlEncode(value.ToString());
                href = href.Replace(string.Format("{{{0}}}", name), substituionValue, StringComparison.InvariantCultureIgnoreCase);
            }

            return new Uri(href, UriKind.Relative);
        }
    }
}