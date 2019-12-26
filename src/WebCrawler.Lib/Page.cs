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
        
        public Page(Uri baseUri, string html) {
            Document = new HtmlDocument();
            Document.LoadHtml(html);
            BaseUri = baseUri;
        }

        public IEnumerable<string> GetLinks() {
            var links = Document.DocumentNode.SelectNodes("//a[@href]")
                .Select(n => n.Attributes["href"].Value)
                .ToList();
            return links;
        }

        public IEnumerable<string> GetInternalLinks()
        {
            var links = GetLinks().Select(l => new Uri(l, UriKind.RelativeOrAbsolute));
            links = links.Select(u => u.IsAbsoluteUri ? u : new Uri(BaseUri, u))
                            .Distinct()
                            .Where(u => BaseUri.IsBaseOf(u));
            return links.Select(u => u.ToString());
        }
    }
}
