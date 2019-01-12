using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace NewsCrawler.Tests
{
    [TestClass]
    public class BbcNewsArticleTitleFetcherServiceTest
    {
        private BbcNewsArticleTitleFetcherService bbcNewsArticleTitleFetcherService;

        [TestInitialize]
        public void Setup()
        {
            bbcNewsArticleTitleFetcherService = new BbcNewsArticleTitleFetcherService();
        }

        [TestMethod]
        public void TitleTest1()
        {
            var html = File.ReadAllText("HtmlPages/TestHtml1.html");
            string expectedTitle = "Mathematician Sir Michael Atiyah dies aged 89";

            var actualTitle = bbcNewsArticleTitleFetcherService.FetchTitle(html);

            Assert.AreEqual(expectedTitle, actualTitle);
        }

        [TestMethod]
        public void TitleTest2()
        {
            var html = File.ReadAllText("HtmlPages/TestHtml2.html");
            string expectedTitle = "Ministers consider ending jail terms of six months or less";

            var actualTitle = bbcNewsArticleTitleFetcherService.FetchTitle(html);

            Assert.AreEqual(expectedTitle, actualTitle);
        }
    }
}
