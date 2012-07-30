using System;
using System.Text.RegularExpressions;
using System.Web;

namespace WebApi.Hal
{
    public class Link
    {
        public Link()
        { }

        public Link(string rel, string href)
        {
            Rel = rel;
            Href = href;
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
        /// <param name="substitutions">Pass in substitutions like 'name=>"Value"', 
        /// if you have a conflicting variable like page, you can do '_page => page + 1'</param>
        /// <returns>A non templated link</returns>
        public Link CreateLink(string newRel, params Func<string, object>[] substitutions)
        {
            return new Link(newRel, CreateUri(substitutions).ToString());
        }

        /// <summary>
        /// If this link is templated, you can use this method to make a non templated copy
        /// </summary>
        /// <param name="substitutions">Pass in substitutions like 'name=>"Value"', 
        /// if you have a conflicting variable like page, you can do '_page => page + 1'</param>
        /// <returns>A non templated link</returns>
        public Link CreateLink(params Func<string, object>[] substitutions)
        {
            return CreateLink(Rel, substitutions);
        }

        public Uri CreateUri(params Func<string, object>[] substitutions)
        {
            var href = Href;
            foreach (var substitution in substitutions)
            {
                var name = substitution.Method.GetParameters()[0].Name.Trim('_');
                var value = substitution(null);
                var substituionValue = value == null ? null : HttpUtility.UrlEncode(value.ToString());
                href = href.Replace(string.Format("{{{0}}}", name), substituionValue);
            }

            return new Uri(href, UriKind.Relative);
        }
    }
}