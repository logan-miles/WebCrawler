using System;
using Xunit;
using FluentAssertions;
using WebCrawler.Lib;
using System.Collections.Generic;
using System.Linq;

namespace WebCrawler.Tests
{
    public class PageTests
    {
        [Theory]
        [InlineData("<html><head></head><body><a href=\"https://www.somesite.org\">SomeLink</a></body>",1)]
        [InlineData("<html><head></head><body><a href=\"https://www.somesite.org\">SomeLink</a><img src=\"https://www.somesite.org/image.gif\"></img></body>",1)]
        [InlineData("<html><head></head><body><a href=\"https://www.somesite.org\">SomeLink</a><a href=\"https://www.somesite.org/link\">SomeLink2</a></body>",2)]
        public void GetLinks_NoFilter_ShouldReturnRightNumberOfLinks(string sourceHtml, int linkCount)
        {
            Page page = new Page(new Uri("https://www.somesite.org"), sourceHtml);
            page.GetLinks().Count().Should().Be(linkCount);
        }

        [Fact]
        public void GetInternalLinks_OneInternal_ShouldReturnOneLink() {
            string sourceHtml = "<html><head></head><body><a href=\"https://www.somesite.org\">SomeLink</a></body>";
            Page page = new Page(new Uri("https://www.somesite.org"), sourceHtml);
            page.GetInternalLinks().Count().Should().Be(1);
        }
    }
}
