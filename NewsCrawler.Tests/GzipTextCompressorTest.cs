using Microsoft.VisualStudio.TestTools.UnitTesting;
using NewsCrawler.Bbc;
using System.IO;
using System.Threading.Tasks;

namespace NewsCrawler.Tests
{
    [TestClass]
    public class GzipTextCompressorTest
    {
        private GzipTextCompressor gzipTextCompressor;

        [TestInitialize]
        public void Setup()
        {
            gzipTextCompressor = new GzipTextCompressor();
        }

        [TestMethod]
        public async Task CompressionText()
        {
            var html = File.ReadAllText("HtmlPages/TestHtml4.html");

            var compressed = await gzipTextCompressor.Compress(html);

            var decompressed = await gzipTextCompressor.Decompress(compressed);

            Assert.AreEqual(html, decompressed);
        }

        [TestMethod]
        public async Task CompressionNullText()
        {
            string html = null;

            var compressed = await gzipTextCompressor.Compress(html);

            var decompressed = await gzipTextCompressor.Decompress(compressed);

            Assert.AreEqual("", decompressed);
        }

        [TestMethod]
        public async Task CompressionEmptyText()
        {
            var html = "";

            var compressed = await gzipTextCompressor.Compress(html);

            var decompressed = await gzipTextCompressor.Decompress(compressed);

            Assert.AreEqual(html, decompressed);
        }
    }
}
