using System;
using System.Collections.Generic;
using System.Linq;
using WebApi.Hal.Exceptions;
using WebApi.Hal.Interfaces;

namespace WebApi.Hal
{
    public class HypermediaContainerBuilder
    {
        readonly IDictionary<Type, object> appenders = new Dictionary<Type, object>();
        readonly IDictionary<Type, Link> selfLinks = new Dictionary<Type, Link>();
        readonly IDictionary<Type, IList<Link>> hypermedia = new Dictionary<Type, IList<Link>>();
        readonly HypermediaConfigurationMode mode;

        public HypermediaContainerBuilder(HypermediaConfigurationMode mode = HypermediaConfigurationMode.Loose)
        {
            this.mode = mode;
        }

        public void RegisterAppender<T>(IHypermediaAppender<T> appender) where T : class, IResource
        {
            if (appender == null) 
                throw new ArgumentNullException("appender");

            var type = typeof(T);

            if (appenders.ContainsKey(type))
                throw new DuplicateHypermediaResolverRegistrationException(type);

            appenders.Add(type, appender);
        }

        public void RegisterSelf<T>(Link link) where T : IResource
        {
            if (link == null)
                throw new ArgumentNullException("link");

            var type = typeof(T);

            if (selfLinks.ContainsKey(type))
                throw new DuplicateSelfLinkRegistrationException(type);

            selfLinks.Add(type, link);
        }

        public void RegisterSelf<T>(Link<T> link) where T : class, IResource
        {
            if (link == null)
                throw new ArgumentNullException("link");

            var type = typeof(T);

            if (selfLinks.ContainsKey(type))
                throw new DuplicateSelfLinkRegistrationException(type);

            selfLinks.Add(type, link);
        }

        public void RegisterLinks<T>(params Link[] links) where T : class, IResource
        {
            if (links == null) 
                throw new ArgumentNullException("links");

            var type = typeof(T);

            if (hypermedia.ContainsKey(type))
                hypermedia[type] = hypermedia[type].Union(links).Distinct(new LinkEqualityComparer()).ToList();
            else
                hypermedia.Add(type, links.Distinct(new LinkEqualityComparer()).ToList());
        }

        public IHypermediaContainer Build()
        {
            return new HypermediaContainer(selfLinks, hypermedia, appenders, mode);
        }
    }
}