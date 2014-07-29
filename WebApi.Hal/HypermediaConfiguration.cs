using System;
using System.Collections.Generic;
using System.Linq;
using WebApi.Hal.Exceptions;
using WebApi.Hal.Interfaces;

namespace WebApi.Hal
{
    class HypermediaConfiguration : IHypermediaConfiguration
    {
        readonly IDictionary<Type, Link> selfLinks;
        readonly IDictionary<Type, IList<Link>> hypermedia;
        readonly IDictionary<Type, object> appenders;
        readonly HypermediaConfigurationMode mode;

        public HypermediaConfiguration(IDictionary<Type, Link> selfLinks, IDictionary<Type, IList<Link>> hypermedia, IDictionary<Type, object> appenders, HypermediaConfigurationMode mode)
        {
            if (selfLinks == null) 
                throw new ArgumentNullException("selfLinks");

            if (hypermedia == null) 
                throw new ArgumentNullException("hypermedia");

            if (appenders == null) 
                throw new ArgumentNullException("appenders");

            this.selfLinks = selfLinks;
            this.hypermedia = hypermedia;
            this.appenders = appenders;
            this.mode = mode;
        }

        public void Configure(IResource resource)
        {
            var representation = resource as Representation;

            if (representation == null)
            {
                ResolveAndAppend(resource); // for backwards compatibility ...
            }
            else
            {
                representation.RepopulateAndConfigureDeep(ResolveAndAppend, ResolveRel);

                var all = representation.GetHypermediaDeep().ToArray();
                var curies = ExtractUniqueCuriesLinks(all);

                foreach (var link in curies)
                    representation.Links.Insert(0, link);

                /*
                 * TODO: IMHO we should also add links to any embedded resource to its direct parent
                 * 
                 * See: https://tools.ietf.org/html/draft-kelly-json-hal-06#section-8.3
                 * 
                 * Servers SHOULD NOT entirely "swap out" a link for an embedded
                 * resource (or vice versa) because client support for this technique is
                 * OPTIONAL.
                 * */
            }
        }

        private void ResolveAndAppend(IResource resource)
        {
            try
            {
                resource.Links.Clear();

                var configured = new List<Link>(ResolveLinks(resource)) { ResolveSelf(resource) };
                var resourceType = resource.GetType();

                // As the IHypermediaAppender interface is generic and the parameter of the Append 
                // method which represents the resource is typed based on that, we need some reflection
                // in order to resolve the appender and to subsequently invoke the Append method on it.

                var info = GetType().GetMethod("ResolveAppender");
                var typedInfo = info.MakeGenericMethod(resourceType);
                var appender = typedInfo.Invoke(this, new object[] {resource});
                var appenderType = appender.GetType();
                var appendMethodInfo = appenderType.GetMethod("Append");
                
                appendMethodInfo.Invoke(appender, new object[]{resource, configured});
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public IHypermediaAppender<T> ResolveAppender<T>(T resource) where T: class, IResource
        {
            var type = resource.GetType();

            if (!appenders.ContainsKey(type)) 
                throw new MissingHypermediaBuilderException(type);
            
            return (IHypermediaAppender<T>) appenders[type];
        }

        public IEnumerable<Link> ResolveLinks(IResource resource)
        {
            var type = resource.GetType();
            
            return hypermedia.ContainsKey(type)
                ? hypermedia[type]
                : new Link[0];
        }

        public string ResolveRel(IResource resource)
        {
            var type = resource.GetType();

            if (selfLinks.ContainsKey(type))
                return selfLinks[type].Rel;

            if (mode == HypermediaConfigurationMode.Strict)
                throw new MissingSelfLinkException(type);

            return type.Name;
        }

        public Link ResolveSelf(IResource resource)
        {
            var type = resource.GetType();

            if (selfLinks.ContainsKey(type))
            {
                var clone = selfLinks[type].Clone();
                clone.Rel = Link.RelForSelf;
                return clone;
            }

            if (mode == HypermediaConfigurationMode.Strict)
                throw new MissingSelfLinkException(type);

            return null;
        }

        public IEnumerable<Link> ExtractUniqueCuriesLinks(params Link[] links)
        {
            if (links == null) 
                throw new ArgumentNullException("links");

            return links.Where(x => x.Curie != null)
                .Select(x => x.Curie)
                .Distinct(CuriesLink.NameComparer)
                .Select(x => x.ToLink());
        }
    }
}