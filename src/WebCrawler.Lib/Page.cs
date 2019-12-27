using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

namespace WebCrawler.Lib
{
    public class Page
    {
        private HtmlDocument Document;
        public Uri Uri;

        public Page(Uri uri, string html) {
            Document = new HtmlDocument();
            Document.LoadHtml(html);
            Uri = uri;
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
            return GetLinks()?.Where(u => Uri.IsBaseOf(u)) ?? new List<Uri>();
        }

        public IEnumerable<Uri> GetExternalLinks()
        {
            return GetLinks()?.Where(u => !Uri.IsBaseOf(u)) ?? new List<Uri>();
        }

        public IEnumerable<string> GetImages()
        {
            var links = Document.DocumentNode.SelectNodes("//img[@src]")
                    ?.Select(n => n.Attributes["src"].Value)
                    ?.Distinct()
                    ?.ToList();
            
            return links ?? new List<string>();
        }

        private Uri GetAbsoluteUriFromHref(string href) {
            Uri uri = new Uri(href, UriKind.RelativeOrAbsolute);
            uri = uri.IsAbsoluteUri ? uri : new Uri(Uri, uri);
            
            return uri;
        }

        private static string StripQueryString(string href) {
            if (href.Contains('?'))
                return href.Split('?')[0];
            return href;
        }

        public string Print()
        {
            PageDto pageDto = new PageDto(this);
            return pageDto.Print();
        }
    }
}
