using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using WebApi.Hal.Interfaces;

namespace WebApi.Hal
{
    public class CuriesLink
    {
        const string CuriesRelExpression = "rel";

        public CuriesLink(string name, string href)
        {
            if (string.IsNullOrEmpty(name)) 
                throw new ArgumentNullException("name");

            if (string.IsNullOrEmpty(href)) 
                throw new ArgumentNullException("href");

            if (!IsValidCuriesHref(href))
                throw new ArgumentException("The provided href is not a valid uri template: " + href, href);

            Name = name;
            Href = href;
        }

        public string Name { get; private set; }
        public string Href { get; private set; }

        string CreateLinkRelation(string name)
        {
            if (string.IsNullOrEmpty(name)) 
                throw new ArgumentNullException("name");

            if (name.Contains(":"))
                throw new ArgumentException("Specified link relation already contains ':' " + name, "name");

            return string.Concat(Name, ":", name);
        }

        public Link CreateLink(string name, string href)
        {
            return new Link(CreateLinkRelation(name), href, this);
        }

        public Link<T> CreateLink<T>(string name, string href) where T : class, IResource
        {
            return new Link<T>(CreateLinkRelation(name), href, this);
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
                        if (!IsValidCuriesHrefRelExpression(expression.ToString()))
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

        private static bool IsValidCuriesHrefRelExpression(string expression)
        {
            if (expression.Equals(CuriesRelExpression, StringComparison.OrdinalIgnoreCase))
                return true;

            var operators = new[] { '+', ';', '/', '#', '&', '?', '.' };
            var first = expression[0];

            if (operators.Any(o => o == first))
                return expression.Substring(1).Equals(CuriesRelExpression, StringComparison.OrdinalIgnoreCase);

            return false; // only a single "rel" expression is allowed in this template ...
        }

        sealed class NameEqualityComparer : IEqualityComparer<CuriesLink>
        {
            public bool Equals(CuriesLink x, CuriesLink y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return string.Equals(x.Name, y.Name);
            }

            public int GetHashCode(CuriesLink obj)
            {
                return (obj.Name != null
                    ? obj.Name.GetHashCode()
                    : 0);
            }
        }

        static readonly IEqualityComparer<CuriesLink> NameComparerInstance = new NameEqualityComparer();

        public static IEqualityComparer<CuriesLink> NameComparer
        {
            get { return NameComparerInstance; }
        }

        public Link ToLink()
        {
            return new Link
            {
                Rel = Link.RelForCuries,
                Name = Name,
                Href = Href
            };
        }
    }
}