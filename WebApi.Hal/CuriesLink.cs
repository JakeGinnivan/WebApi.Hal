using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using WebApi.Hal.Interfaces;

namespace WebApi.Hal
{
    public partial class CuriesLink
    {
        public CuriesLink(string name, string href)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            if (string.IsNullOrEmpty(href))
                throw new ArgumentNullException(nameof(href));

            if (HrefRelRegex().Count(href) != 1)
                throw new ArgumentException("The provided href is not a valid uri template: " + href, href);

            Name = name;
            Href = href;
        }

        public string Name { get; }
        public string Href { get; }

        private string CreateLinkRelation(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            if (name.Contains(":"))
                throw new ArgumentException("Specified link relation already contains ':' " + name, nameof(name));

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

        [GeneratedRegex(@"\{[+;/#&?.]?rel\}", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
        private static partial Regex HrefRelRegex();

        public Link ToLink()
        {
            return new Link
            {
                Rel = Link.RelForCuries,
                Name = Name,
                Href = Href
            };
        }

        public static IEqualityComparer<CuriesLink> EqualityComparer { get; } = new NameEqualityComparer();

        private sealed class NameEqualityComparer : IEqualityComparer<CuriesLink>
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
                return obj.Name?.GetHashCode() ?? 0;
            }
        }
    }
}