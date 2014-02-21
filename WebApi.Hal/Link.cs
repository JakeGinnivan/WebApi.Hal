using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

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
            get { return !string.IsNullOrEmpty(Href) && IsTemplatedRegex.IsMatch(Href); }
        }

        private static readonly Regex IsTemplatedRegex = new Regex(@"{.+}", RegexOptions.Compiled);

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

            return new Uri(href, UriKind.RelativeOrAbsolute);
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
    }

    internal class LinkEqualityComparer : IEqualityComparer<Link>
    {
        public bool Equals(Link l1, Link l2)
        {
            return string.Compare(l1.Href, l2.Href, StringComparison.OrdinalIgnoreCase) == 0 &&
                   string.Compare(l1.Rel, l2.Rel, StringComparison.OrdinalIgnoreCase) == 0;
        }


        public int GetHashCode(Link lnk)
        {
            var str = (string.IsNullOrEmpty(lnk.Rel) ? "norel" : lnk.Rel) + "~" + (string.IsNullOrEmpty(lnk.Href) ? "nohref" : lnk.Href);
            var h = str.GetHashCode();
            return h;
        }
    }
}