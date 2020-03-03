using System;
using System.Linq;
using FluentAssertions;
using WebCrawler.Lib;
using Xunit;

namespace WebCrawler.Tests {
    public class PageTests {
        [Theory]
        [InlineData("<html><head></head><body><a href=\"https://www.somesite.org\">SomeLink</a></body>", 1)]
        [InlineData("<html><head></head><body><a href=\"https://www.somesite.org\">SomeLink</a><img src=\"https://www.somesite.org/image.gif\"></img></body>", 1)]
        [InlineData("<html><head></head><body><a href=\"https://www.somesite.org\">SomeLink</a><a href=\"https://www.somesite.org/link\">SomeLink2</a></body>", 2)]
        public void GetLinks_NoFilter_ShouldReturnRightNumberOfLinks(string sourceHtml, int linkCount) {
            Page page = new Page(new Uri("https://www.somesite.org"), sourceHtml);
            page.GetLinks().Count().Should().Be(linkCount);
        }

        [Theory]
        [InlineData("<html><head></head><body><a href=\"https://www.somesite.org\">SomeLink</a></body>", 1, "1 link returns 1")]
        [InlineData("<html><head></head><body><a href=\"https://www.somesite.org\">SomeLink</a><img src=\"https://www.somesite.org/image.gif\"></img></body>", 1, "1 link 1 image returns 1 link")]
        [InlineData("<html><head></head><body><a href=\"https://www.somesite.org\">SomeLink</a><a href=\"https://www.somesite.org/link\">SomeLink2</a></body>", 2, "2 internal links returns 2")]
        [InlineData("<html><head></head><body><a href=\"https://www.somesite.org\">SomeLink</a><a href=\"https://www.someothersite.org/link\">SomeLink2</a></body>", 1, "1 internal 1 external returns 1")]
        [InlineData("<html><head></head><body><a href=\"https://www.somesite.org\">SomeLink</a><a href=\"/link\">SomeLink2</a></body>", 2, "1 absolute internal 1 relative returns 2")]
        [InlineData("<html><head></head><body><a href=\"https://www.somesite.org\">SomeLink</a><a href=\"/link\">SomeLink2</a><a href=\"/link/linker\">SomeLink2</a></body>", 3, "1 absolute, 1 relative one level, 1 relative 2 levels, returns 3")]
        [InlineData("<html><head></head><body><a href=\"https://www.somesite.org/link\">SomeLink</a><a href=\"https://www.somesite.org/link\">SomeLink2</a></body>", 1, "2 identical internal returns 1 link")]
        [InlineData("<html><head></head><body><a href=\"https://www.somesite.org/link\">SomeLink</a><a href=\"https://www.somesite.org/link#about\">SomeLink2</a></body>", 1, "2 identical internal with anchor returns 1 link")]
        [InlineData("<html><head></head><body><a href=\"https://www.somesite.org/link\">SomeLink</a><a href=\"https://www.somesite.org/link?value=1\">SomeLink2</a></body>", 1, "2 identical internal with query param returns 1 link")]
        public void GetInternalLinks_ShouldReturnNumberOfInternal(string sourceHtml, int linkCount, string message) {
            Page page = new Page(new Uri("https://www.somesite.org/unlink"), sourceHtml);
            page.GetInternalLinks().Count().Should().Be(linkCount, message);
        }

        [Theory]
        [InlineData("<html><head></head><body><a href=\"https://www.somesite.org\">SomeLink</a><a href=\"https://www.someothersite.org/link\">SomeLink2</a></body>", 1, "1 internal 1 external returns 1")]
        public void GetExternalLinks_ShouldReturnNumberOfExternal(string sourceHtml, int linkCount, string message) {
            Page page = new Page(new Uri("https://www.somesite.org/unlink"), sourceHtml);
            page.GetExternalLinks().Count().Should().Be(linkCount, message);
        }

        [Theory]
        [InlineData("<html><head></head><body><a href=\"https://www.somesite.org\">SomeLink</a><img src=\"https://www.somesite.org/image.gif\"></img></body>", 1, "1 link 1 image returns 1 image")]
        public void GetImages_ShouldReturnNumberOfImages(string sourceHtml, int imageCount, string message) {
            Page page = new Page(new Uri("https://www.somesite.org/unlink"), sourceHtml);
            page.GetImages().Count().Should().Be(imageCount, message);
        }

        [Theory]
        [InlineData("<html><head></head><body><a href=\"https://www.somesite.org/link\">SomeLink</a><a href=\"https://www.someothersite.org/link\">SomeLink</a><img src=\"https://www.somesite.org/image.gif\"></img></body>", "{\n  \"uri\": \"https://www.somesite.org/\",\n  \"internalLinks\": [\n    \"https://www.somesite.org/link\"\n  ],\n  \"externalLinks\": [\n    \"https://www.someothersite.org/link\"\n  ],\n  \"images\": [\n    \"https://www.somesite.org/image.gif\"\n  ]\n}")]
        public void Print_ShouldReturnSerializedDto(string sourceHtml, string serializedDto) {
            Page page = new Page(new Uri("https://www.somesite.org/"), sourceHtml);
            var printedPage = page.Print();
            printedPage.Should().Be(serializedDto);
        }
    }
}