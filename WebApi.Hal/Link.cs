using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

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
        public string Type { get; set; }
        public string Deprecation { get; set; }
        public string Name { get; set; }
        public string Profile { get; set; }
        public string HrefLang { get; set; }
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

        /// <summary>
        /// Gets whether the link represents a Curies link
        /// </summary>
        [JsonIgnore]
        public bool IsCuries
        {
            get
            {
                return !string.IsNullOrEmpty(Name) && IsValidCuriesRel(Rel) && IsValidCuriesHref(Href);
            }
        }

        private static bool IsValidCuriesRel(string rel)
        {
            return !string.IsNullOrEmpty(rel) && rel.Equals("curies", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsValidCuriesHref(string template)
        {
            if (string.IsNullOrEmpty(template))
                return false;

            var expression = new StringBuilder();
            var building = false;
            var foundRel = false;

            foreach (var c in template)
            {
                switch (c)
                {
                    case '{':
                        if (foundRel)
                            return false; // only a single "rel" expression is allowed in this template ...
                        building = true;
                        expression.Clear();
                        break;
                    case '}':
                        if (!expression.ToString().Equals("rel", StringComparison.OrdinalIgnoreCase))
                            return false; // only a single "rel" expression is allowed in this template ...
                        building = false;
                        foundRel = true;
                        break;
                    default:
                        if (building)
                            expression.Append(c);
                        break;
                }
            }

            return foundRel;
        }

        /// <summary>
        /// Factory method that simplifies creating a curies link
        /// </summary>
        public static Link CreateCuries(string name, string href)
        {
            if (string.IsNullOrEmpty(name)) 
                throw new ArgumentNullException("name", "A curies link requires a name");

            if (!IsValidCuriesHref(href)) 
                throw new ArgumentException("Not a valid uri template for curies, exactly one {rel} expression is required");

            return new Link("curies", href)
            {
                Name = name
            };
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