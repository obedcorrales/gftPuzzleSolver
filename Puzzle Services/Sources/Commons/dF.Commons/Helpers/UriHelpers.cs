using System;

namespace dF.Commons.Helpers
{
    public static class UriHelpers
    {
        public static string MapHostWildcardTo(string urlString, string host)
        {
            var i = urlString.IndexOf("+");

            if (i > 0)
                urlString = string.Format("{0}{1}{2}", urlString.Substring(0, i), host, urlString.Substring(i + 1));

            i = urlString.IndexOf("*");

            if (i > 0)
                urlString = string.Format("{0}{1}{2}", urlString.Substring(0, i), host, urlString.Substring(i + 1));

            return urlString;
        }

        public static string MapHostWildcardToLocalhost(string urlString)
        {
            return MapHostWildcardTo(urlString, "localhost");
        }

        public static string JoinURIs(string url1, string url2)
        {
            var mid = string.Empty;

            if (!url1.EndsWith("/") && !url2.StartsWith("/"))
                mid = "/";
            else if (url1.EndsWith("/") && url2.StartsWith("/"))
                url2 = url2.Substring(1);

            return url1 + mid + url2;
        }

        public static Uri ToURI(this string url)
        {
            return new Uri(url);
        }
    }
}
