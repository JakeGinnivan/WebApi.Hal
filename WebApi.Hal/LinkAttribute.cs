using System;
using System.Collections.Generic;

namespace WebApi.Hal
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = true)]
    public class LinkAttribute : Attribute
    {
        public LinkAttribute(string rel, string href, string title = null, string paramMappings = null)
        {
            Rel = rel;
            Href = href;
            Title = title;
            UrlParameterMappings = new Dictionary<string, string>();
            if (paramMappings != null)
            {
                var maps = paramMappings.Split(",".ToCharArray());
                if (maps.Length > 0)
                {
                    foreach (var item in maps)
                    {
                        if (!String.IsNullOrWhiteSpace(item) && item.Contains(":"))
                        {
                            var keyValuePair = item.Split(":".ToCharArray());
                            if (keyValuePair.Length == 2)
                            {
                                var key = keyValuePair[0];
                                var value = keyValuePair[1];

                                if (!String.IsNullOrWhiteSpace(key) && !String.IsNullOrWhiteSpace(value))
                                {
                                    this.UrlParameterMappings.Add(key, value);
                                }
                            }
                        }
                    }
                }
            }
        }

        public string Rel { get; set; }
        public string Href { get; set; }
        public string Title { get; set; }
        public Dictionary<string, string> UrlParameterMappings; 

        public Link CreateLink()
        {
            return new Link(this.Rel, this.Href, this.Title);
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class SelfAttribute : LinkAttribute
    {
        public SelfAttribute(string rel, string href, string title = null) : base(rel, href, title)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class HrefKeyAttribute : Attribute
    {
        public HrefKeyAttribute()
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false)]
    public class RelAttribute : Attribute
    {
        public string Rel { get; private set; }

        public RelAttribute(string rel)
        {
            if (String.IsNullOrEmpty(rel))
                throw new ArgumentException("Parameter 'rel' must not be null or empty.");
            Rel = rel;
        }
    }

}