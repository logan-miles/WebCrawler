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
        [Fact]
        public void GetLinks_OneLink_ShouldReturnLinks()
        {
            string sourceHtml = "<html><head></head><body><a href=\"https://www.somesite.org\"></body>";
            Page page = new Page(sourceHtml);
            page.GetLinks().Count().Should().Be(1);
        }
    }
}
