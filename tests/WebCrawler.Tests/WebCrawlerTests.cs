using Xunit;
using Moq;
using System.Net.Http;
using Moq.Contrib.HttpClient;

namespace WebCrawler.Tests
{
    public class WebCrawlerTests
    {
        private IHttpClientFactory factory;
        public WebCrawlerTests() {
            var handler = new Mock<HttpMessageHandler>();
            factory = handler.CreateClientFactory();


        }

        private void SetupHandler(Mock<HttpMessageHandler> handler) {
            string baseUrl = "https://www.logan.com";
            handler.SetupRequest(HttpMethod.Get, $"{baseUrl}")
                .ReturnsResponse("<html><head><title>Logan Main</title></head><body><a href=\"https://www.google.com\"</body></html>");
        }
    }
}