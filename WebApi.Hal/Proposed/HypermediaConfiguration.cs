using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using WebApi.Hal.Interfaces;
using WebApi.Hal.Proposed.Exceptions;

namespace WebApi.Hal.Proposed
{
    class HypermediaConfiguration : IHypermediaConfiguration
    {
        readonly IDictionary<Type, Link> selfLinks;
        readonly IDictionary<Type, IList<Link>> hypermedia;
        readonly IDictionary<string, string> curiesLinks;
        readonly IDictionary<Type, IHypermediaAppender> builders;
        readonly HypermediaConfigurationMode mode;

        public HypermediaConfiguration(IDictionary<Type, Link> selfLinks, IDictionary<Type, IList<Link>> hypermedia, IDictionary<string, string> curiesLinks, IDictionary<Type, IHypermediaAppender> builders, HypermediaConfigurationMode mode)
        {
            if (selfLinks == null) 
                throw new ArgumentNullException("selfLinks");

            if (hypermedia == null) 
                throw new ArgumentNullException("hypermedia");

            if (curiesLinks == null) 
                throw new ArgumentNullException("curiesLinks");

            if (builders == null) 
                throw new ArgumentNullException("builders");

            this.selfLinks = selfLinks;
            this.hypermedia = hypermedia;
            this.curiesLinks = curiesLinks;
            this.builders = builders;
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
                var curies = ResolveCuries(all);

                foreach (var link in curies)
                    representation.Links.Add(link);

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
                var appender = ResolveAppender(resource);
                var configured = new List<Link>(ResolveLinks(resource)) { ResolveSelf(resource) };

                resource.Links.Clear();

                if (appender == null)
                    return; // no specific appender registered, so we're done ...

                appender.SetResource(resource);
                appender.Append(configured);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public IHypermediaAppender ResolveAppender(IResource resource)
        {
            var type = resource.GetType();

            if (!builders.ContainsKey(type) && (mode == HypermediaConfigurationMode.Strict))
                throw new MissingHypermediaBuilderException(type);

            return builders.ContainsKey(type)
                ? builders[type]
                : null;
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

            return null;
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

        public IEnumerable<Link> ResolveCuries(params Link[] links)
        {
            if (links == null) 
                throw new ArgumentNullException("links");

            var names = links.Select(x => x.Rel).Distinct().Select(ExtractCuriesName).Where(x => x != null);

            foreach (var name in names)
            {
                if (curiesLinks.ContainsKey(name))
                    yield return new Link
                    {
                        Rel = Link.RelForCuries,
                        Name = name,
                        Href = curiesLinks[name]
                    };
                else if (mode == HypermediaConfigurationMode.Strict)
                    throw new MissingCuriesLinkException(name);
            }
        }

        static string ExtractCuriesName(string rel)
        {
            if (string.IsNullOrEmpty(rel))
                return null;

            var parts = rel.Split(':');

            if (parts.Length != 2)
                return null;
            
           /*
            * There is some abiguity here as a rel may be a URI or a CURIE as described in 
            * section 2.2 here: http://www.w3.org/TR/2009/CR-curie-20090116/ Simply put
            * "http://" or "mailto:me@you.it" would result in the respective curie names "http" 
            * and "mailto". We can only trust the developer knows what he is doing and choses 
            * the curie names wisely.
            */
            
            return parts[0];
        }
    }
}