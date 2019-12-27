using System;
using System.Net.Http;

namespace WebCrawler.Lib
{
    public class WebCrawler
    {
        private IHttpClientFactory factory;
        private Uri uri;
        public WebCrawler(IHttpClientFactory factory, string uri) {
            this.factory = factory;
            this.uri = new Uri(uri);
        }

        private bool Validate(string uri) {
            return Uri.IsWellFormedUriString(uri, UriKind.Absolute);
        }
    }
}