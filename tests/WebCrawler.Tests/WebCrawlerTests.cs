using Xunit;
using Moq;
using System.Net.Http;
using Moq.Contrib.HttpClient;
using FluentAssertions;

namespace WebCrawler.Tests
{
    public class WebCrawlerTests
    {
        private IHttpClientFactory factory;
        public WebCrawlerTests() {
            var handler = new Mock<HttpMessageHandler>();
            factory = handler.CreateClientFactory();
            SetupHandler(handler);
            


        }

        [Theory]
        [InlineData("https://www.logan.com", "<html><head><title>Logan Main</title></head><body><a href=\"https://www.google.com\"</body></html>", "Main")]
        [InlineData("https://www.logan.com/about", "<html><head><title>Logan About</title></head><body><a href=\"https://www.google.com\"</body></html>", "About")]
        public void TestMock(string uri, string expected, string message) {
            var b = factory.CreateClient();
            b.GetStringAsync(uri).Result
                .Should().Be(expected, message);
        }

        private void SetupHandler(Mock<HttpMessageHandler> handler) {
            string baseUrl = "https://www.logan.com";
            handler.SetupRequest(HttpMethod.Get, $"{baseUrl}")
                .ReturnsResponse("<html><head><title>Logan Main</title></head><body><a href=\"https://www.google.com\"</body></html>");
            handler.SetupRequest(HttpMethod.Get, $"{baseUrl}/about")
                .ReturnsResponse("<html><head><title>Logan About</title></head><body><a href=\"https://www.google.com\"</body></html>");    
        }
    }
}