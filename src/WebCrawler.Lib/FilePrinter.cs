using System.IO;
using System.Text.RegularExpressions;

namespace WebCrawler.Lib {
    public class FilePrinter : IMapPrinter {
        public string Print(Crawler crawler) {
            string output = crawler.GetMap();
            string fileName = $"./out/{crawler.targetSite.Host}.txt";
            File.WriteAllText(fileName, output);

            return fileName;
        }
    }
}