using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

namespace WebCrawler.Lib
{
    public class Page
    {
        HtmlDocument document;
        public Page(string html) {
            document = new HtmlDocument();
            document.LoadHtml(html);
        }

        public IEnumerable<string> GetLinks() {
            var links = document.DocumentNode.SelectNodes("//a[@href]")
                .Select(n => n.Attributes["href"].Value)
                .ToList();
            return links;
        }
    }
}
