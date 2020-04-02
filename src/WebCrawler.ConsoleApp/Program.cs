using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using WebCrawler.Lib;

namespace WebCrawler.ConsoleApp {
    class Program {
        static async Task Main(string[] args) {
            var serviceProvider = new ServiceCollection()
                .AddHttpClient()
                .AddSingleton<IMapPrinter, ConsolePrinter>()
                .BuildServiceProvider();

            Crawler crawler = new Crawler(args[0], serviceProvider.GetService<IHttpClientFactory>(), serviceProvider.GetService<IMapPrinter>());
            await crawler.Crawl();
            crawler.Print();
        }
    }
}