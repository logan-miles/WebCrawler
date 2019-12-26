using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

namespace WebCrawler.Lib
{
    public class Page
    {
        private HtmlDocument Document;
        private Uri BaseUri;

        private IEnumerable<Uri> Links;
        private IEnumerable<Uri> InternalLinks;
        private IEnumerable<Uri> ExternalLinks;

        public Page(Uri baseUri, string html) {
            Document = new HtmlDocument();
            Document.LoadHtml(html);
            BaseUri = baseUri;
        }

        public IEnumerable<Uri> GetLinks() {
            var links = Document.DocumentNode.SelectNodes("//a[@href]")
                .Select(n => n.Attributes["href"].Value)
                .Select(h => StripQueryString(h))
                .Select(h => GetAbsoluteUriFromHref(h))
                .Distinct()
                .ToList();
            return links;
        }

        public IEnumerable<Uri> GetInternalLinks()
        {
            return GetLinks().Where(u => BaseUri.IsBaseOf(u));
        }

        private Uri GetAbsoluteUriFromHref(string href) {
            Uri uri = new Uri(href, UriKind.RelativeOrAbsolute);
            uri = uri.IsAbsoluteUri ? uri : new Uri(BaseUri, uri);
            
            return uri;
        }

        private static string StripQueryString(string href) {
            if (href.Contains('?'))
                return href.Split('?')[0];
            return href;
        }
    }
}
