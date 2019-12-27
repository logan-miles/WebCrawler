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

        public async Task<HashSet<string>> Crawl(string uri) {
            ValidateUri(uri);
            SeedQueue(uri);

            do {                
                await ProcessQueue();

            } while (SitesInQueue());

            return visitedPages;
        }

        private bool Validate(string uri) {
            return Uri.IsWellFormedUriString(uri, UriKind.Absolute);
        }

        private bool NotAlreadyVisited(Uri target) {
            return !visitedPages.Contains(target.AbsoluteUri);
        }

        private void SeedQueue(string uri) {
            pagesToVisit.Enqueue(new Uri(uri));
        }

        public void ValidateUri(string uri) {
            if (!Uri.IsWellFormedUriString(uri, UriKind.Absolute))
                throw new Exception("Invalid URL, Jerk!");
        }

        public async Task ProcessQueue() {
            Uri target = pagesToVisit.Dequeue();
            if (NotAlreadyVisited(target))   
                await ProcessUri(target);
        }

        private async Task ProcessUri(Uri target) {
            visitedPages.Add(target.AbsoluteUri);
                
                Page page = await BuildPage(target);
                AddPageToMap(page);
                AddInternalLinksToQueue(page);
        }
        
        private async Task<Page> BuildPage(Uri target) {
            var client = factory.CreateClient();
            string content = await client.GetStringAsync(target);
            
            return new Page(target, content);
        }

        private void AddPageToMap(Page page) {
            map.AppendLine(page.Print());
        }

        private void AddInternalLinksToQueue(Page page) {
            page.GetInternalLinks()
                .ToList()
                .ForEach(l => pagesToVisit.Enqueue(l));
        }

        private bool SitesInQueue() {
            return pagesToVisit.Count > 0;
        }

        public string Print() {
            return map.ToString();
        }
    }
}