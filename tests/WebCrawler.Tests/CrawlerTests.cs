using System.Net.Http;
using FluentAssertions;
using Moq;
using Moq.Contrib.HttpClient;
using WebCrawler.Lib;
using Xunit;

namespace WebCrawler.Tests {
    public class CrawlerTests {
        private IHttpClientFactory factory;
        public CrawlerTests() {
            var handler = new Mock<HttpMessageHandler>();
            factory = handler.CreateClientFactory();
            SetupHandler(handler);
        }

        [Theory]
        [InlineData("https://www.logan.com", "<html><head><title>Logan Main</title></head><body><a href=\"https://www.google.com\"></a><a href=\"https://www.logan.com/about\"></a></body></html>", "Main")]
        [InlineData("https://www.logan.com/about", "<html><head><title>Logan About</title></head><body><a href=\"https://www.google.com\"></a></body></html>", "About")]
        public void TestMock(string uri, string expected, string message) {
            var b = factory.CreateClient();
            b.GetStringAsync(uri).Result
                .Should().Be(expected, message);
        }

        [Theory]
        [InlineData("https://www.logan.com", 2, "Main and About shoud be 2")]
        [InlineData("https://www.logan.com/loop", 1, "Loop should only hit once")]
        [InlineData("https://www.logan.com/threeWithDuplicate1", 3, "Three pages all with circular references")]
        private void CrawlTest(string uri, int length, string message) {
            Crawler crawler = new Crawler(factory, new StringPrinter());
            var result = crawler.Crawl(uri).Result;
            result.Count.Should().Be(length, message);
        }

        [Theory]
        [InlineData("https://www.logan.com", "{\n  \"uri\": \"https://www.logan.com/\",\n  \"internalLinks\": [\n    \"https://www.logan.com/about\"\n  ],\n  \"externalLinks\": [\n    \"https://www.google.com/\"\n  ],\n  \"images\": []\n}\n{\n  \"uri\": \"https://www.logan.com/about\",\n  \"internalLinks\": [],\n  \"externalLinks\": [\n    \"https://www.google.com/\"\n  ],\n  \"images\": []\n}\n")]
        private async void CrawlPrintTest(string uri, string expected) {
            Crawler crawler = new Crawler(factory, new StringPrinter());
            var result = await crawler.Crawl(uri);
            crawler.Print().Should().Be(expected);
        }

        private void SetupHandler(Mock<HttpMessageHandler> handler) {
            string baseUrl = "https://www.logan.com";
            handler.SetupRequest(HttpMethod.Get, $"{baseUrl}")
                .ReturnsResponse("<html><head><title>Logan Main</title></head><body><a href=\"https://www.google.com\"></a><a href=\"https://www.logan.com/about\"></a></body></html>");
            handler.SetupRequest(HttpMethod.Get, $"{baseUrl}/about")
                .ReturnsResponse("<html><head><title>Logan About</title></head><body><a href=\"https://www.google.com\"></a></body></html>");
            handler.SetupRequest(HttpMethod.Get, $"{baseUrl}/loop")
                .ReturnsResponse($"<html><head></head><body><a href=\"{baseUrl}/loop\"></a></body>");
            handler.SetupRequest(HttpMethod.Get, $"{baseUrl}/threeWithDuplicate1")
                .ReturnsResponse($"<html><body><a href=\"{baseUrl}/threeWithDuplicate2\"></a><a href=\"{baseUrl}/threeWithDuplicate3\"></a></body></html>");
            handler.SetupRequest(HttpMethod.Get, $"{baseUrl}/threeWithDuplicate2")
                .ReturnsResponse($"<html><body><a href=\"{baseUrl}/threeWithDuplicate1\"></a><a href=\"{baseUrl}/threeWithDuplicate3\"></a></body></html>");
            handler.SetupRequest(HttpMethod.Get, $"{baseUrl}/threeWithDuplicate3")
                .ReturnsResponse($"<html><body><a href=\"{baseUrl}/threeWithDuplicate1\"></a><a href=\"{baseUrl}/threeWithDuplicate2\"></a></body></html>");
            handler.SetupRequest(HttpMethod.Get, $"https://www.google.com")
                .ReturnsResponse("<html><body>Google</body></html>");
        }
    }
}