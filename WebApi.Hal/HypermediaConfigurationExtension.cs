using System;
using System.Collections.Generic;
using System.Linq;
using WebApi.Hal.Interfaces;

namespace WebApi.Hal
{
    public static class HypermediaConfigurationExtension
    {
        public static void Register<T>(this IHypermediaBuilder builder, Link<T> selfLink, IHypermediaAppender<T> appender, params Link[] links) where T : class, IResource
        {
            if (selfLink == null)
                throw new ArgumentNullException("selfLink");

            if (appender == null)
                throw new ArgumentNullException("appender");

            builder.RegisterSelf(selfLink);
            builder.RegisterAppender(appender);
            builder.RegisterLinks<T>(links);
        }

        public static void Register<T>(this IHypermediaBuilder builder, Link<T> selfLink, Action<T, IEnumerable<Link>> appender, params Link[] links) where T : class, IResource
        {
            if (selfLink == null)
                throw new ArgumentNullException("selfLink");

            if (appender == null)
                throw new ArgumentNullException("appender");

            builder.RegisterSelf(selfLink);
            builder.RegisterAppender(new ActionBasedHypermediaAppender<T>(appender));
            builder.RegisterLinks<T>(links);
        }

        public static void Register<T>(this IHypermediaBuilder builder, Link<T> selfLink, params Link[] links) where T : class, IResource
        {
            if (selfLink == null)
                throw new ArgumentNullException("selfLink");

            builder.RegisterSelf(selfLink);
            builder.RegisterLinks<T>(links);
            builder.RegisterAppender(new ActionBasedHypermediaAppender<T>(
                (resource, configured) =>
                {
                    resource.Links = configured.ToList();
                }));
        }
    }
}