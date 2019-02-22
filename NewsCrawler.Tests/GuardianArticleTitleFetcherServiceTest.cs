using Microsoft.VisualStudio.TestTools.UnitTesting;
using NewsCrawler.Bbc;
using NewsCrawler.Guardian;
using System.IO;

namespace NewsCrawler.Tests
{
    [TestClass]
    public class GuardianArticleTitleFetcherServiceTest
    {
        private GuardianArticleTitleFetcherService guardianArticleTitleFetcherService;

        [TestInitialize]
        public void Setup()
        {
            guardianArticleTitleFetcherService = new GuardianArticleTitleFetcherService();
        }

        [TestMethod]
        public void TitleTest1()
        {
            var html = File.ReadAllText("HtmlPages/TestHtml5.html");
            string expectedTitle = "Tennis";

            var actualTitle = guardianArticleTitleFetcherService.FetchTitle(html);

            Assert.AreEqual(expectedTitle, actualTitle);
        }

    }
}
