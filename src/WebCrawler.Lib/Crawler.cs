using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace WebCrawler.Lib
{
    public class Crawler
    {
        private IHttpClientFactory factory;

        private HashSet<string> visitedPages;
        private Queue<Uri> pagesToVisit;
        private StringBuilder map;

        public Crawler(IHttpClientFactory factory) {
            visitedPages = new HashSet<string>();
            pagesToVisit = new Queue<Uri>();
            map = new StringBuilder();
            
            this.factory = factory;
        }

        private bool Validate(string uri) {
            return Uri.IsWellFormedUriString(uri, UriKind.Absolute);
        }

        public async Task<HashSet<string>> Crawl(string uri) {
            if (!Uri.IsWellFormedUriString(uri, UriKind.Absolute))
                throw new Exception("Invalid URL, Jerk!");

            pagesToVisit.Enqueue(new Uri(uri));

            do {                
                Uri target = pagesToVisit.Dequeue();
                if (visitedPages.Contains(target.AbsoluteUri))
                    continue;

                visitedPages.Add(target.AbsoluteUri);
                
                Page page = await BuildPage(target);
                AddPageToMap(page);

                var x = page.GetInternalLinks().ToList();
                x.ForEach(l => pagesToVisit.Enqueue(l));

            } while (SitesInQueue());

            return visitedPages;
        }
        
        private async Task<Page> BuildPage(Uri target) {
            var client = factory.CreateClient();
            string content = await client.GetStringAsync(target);
            
            return new Page(target, content);
        }

        private void AddPageToMap(Page page) {
            map.AppendLine(page.Print());
        }

        private bool SitesInQueue() {
            return pagesToVisit.Count > 0;
        }
    }
}