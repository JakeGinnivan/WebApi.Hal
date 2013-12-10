using System;
using System.Web;

namespace WebApi.Hal
{
    public static class HttpRequestExtensions
    {
        public static Uri GetOriginalUrl(this HttpRequest request)
        {
            return GetOriginalUrl(new HttpRequestWrapper(request));
        }

        public static Uri GetBaseUrl(this HttpRequestBase request)
        {
            return new Uri(new Uri(request.GetOriginalUrl().GetLeftPart(UriPartial.Authority)), request.ApplicationPath ?? "");
        }

        public static Uri GetOriginalUrl(this HttpRequestBase request)
        {
            var hostUrl = new UriBuilder();
            string hostHeader = request.Headers["Host"];

            if (hostHeader.Contains(":"))
            {
                hostUrl.Host = hostHeader.Split(':')[0];
                hostUrl.Port = Convert.ToInt32(hostHeader.Split(':')[1]);
            }
            else
            {
                hostUrl.Host = hostHeader;
                hostUrl.Port = -1;
            }

            Uri url = request.Url;
            var uriBuilder = new UriBuilder(url);

            // When the application is run behind a load-balancer (or forward proxy), request.IsSecureConnection returns 'true' or 'false'
            // based on the request from the load-balancer to the web server (e.g. IIS) and not the actual request to the load-balancer.
            // The same is also applied to request.Url.Scheme (or uriBuilder.Scheme, as in our case).
            bool isSecureConnection = String.Equals(request.Headers["X-Forwarded-Proto"], "https", StringComparison.OrdinalIgnoreCase);

            if (isSecureConnection)
            {
                uriBuilder.Port = hostUrl.Port == -1 ? 443 : hostUrl.Port;
                uriBuilder.Scheme = "https";
            }
            else
            {
                uriBuilder.Port = hostUrl.Port == -1 ? 80 : hostUrl.Port;
                uriBuilder.Scheme = "http";
            }

            uriBuilder.Host = hostUrl.Host;

            return uriBuilder.Uri;
        }

        public static string GetOriginalUserHostAddress(this HttpRequest request)
        {
            return GetOriginalUserHostAddress(new HttpRequestWrapper(request));
        }

        public static string GetOriginalUserHostAddress(this HttpRequestBase request)
        {
            var forwardedFor = request.Headers["X-Forwarded-For"];

            if (string.IsNullOrEmpty(forwardedFor))
            {
                return request.UserHostAddress;
            }

            return forwardedFor;
        }
    }
}