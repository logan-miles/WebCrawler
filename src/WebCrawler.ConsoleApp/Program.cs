using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using WebCrawler.Lib;

namespace WebCrawler.ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
                .AddHttpClient()
                .BuildServiceProvider();

            Crawler crawler = new Crawler(serviceProvider.GetService<IHttpClientFactory>());
            await crawler.Crawl(args[0]);
            string s = crawler.Print();
            Console.WriteLine(s);
        }
    }
}
