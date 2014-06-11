using System;
using System.Collections.Generic;
using System.Linq;
using WebApi.Hal.Exceptions;
using WebApi.Hal.Interfaces;

namespace WebApi.Hal
{
    public class HypermediaConfigurationBuilder
    {
        readonly IDictionary<Type, IHypermediaAppender> builders = new Dictionary<Type, IHypermediaAppender>();
        readonly IDictionary<Type, Link> selfLinks = new Dictionary<Type, Link>();
        readonly IDictionary<Type, IList<Link>> hypermedia = new Dictionary<Type, IList<Link>>();
        readonly IDictionary<string, string> curiesLinks = new Dictionary<string, string>();
        readonly HypermediaConfigurationMode mode;

        public HypermediaConfigurationBuilder(HypermediaConfigurationMode mode = HypermediaConfigurationMode.Loose)
        {
            this.mode = mode;
        }

        public void RegisterAppender<T>(HypermediaAppender<T> appender) where T : class, IResource
        {
            if (appender == null) 
                throw new ArgumentNullException("appender");

            var type = typeof(T);

            if (selfLinks.ContainsKey(type))
                throw new DuplicateHypermediaResolverRegistrationException(type);

            builders.Add(type, appender);
        }

        public void RegisterSelf<T>(Link link) where T: IResource
        {
            if (link == null) 
                throw new ArgumentNullException("link");

            var type = typeof (T);

            if (selfLinks.ContainsKey(type))
                throw new DuplicateSelfLinkRegistrationException(type);

            selfLinks.Add(type, link);
        }

        public void RegisterCuries(params CuriesLink[] links)
        {
            if (links == null) 
                throw new ArgumentNullException("links");

            if (!links.Any())
                return; // nothing to add ...

            foreach (var curiesLink in links)
            {
                if (curiesLinks.ContainsKey(curiesLink.Name))
                    throw new DuplicateCurisLinkRegistrationException(curiesLink.Name);
                
                // no need to (re)validate the href, it has already been done by the ctor of CuriesLink

                curiesLinks.Add(curiesLink.Name, curiesLink.Href);
            }
        }

        public void RegisterLinks<T>(params Link[] links)
        {
            if (links == null) 
                throw new ArgumentNullException("links");

            var type = typeof(T);

            if (hypermedia.ContainsKey(type))
                hypermedia[type] = hypermedia[type].Union(links).Distinct(new LinkEqualityComparer()).ToList();
            else
                hypermedia.Add(type, links.Distinct(new LinkEqualityComparer()).ToList());
        }

        public IHypermediaConfiguration Build()
        {
            return new HypermediaConfiguration(selfLinks, hypermedia, curiesLinks, builders, mode);
        }
    }
}