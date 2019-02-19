using HtmlAgilityPack;
using NewsCrawler.Interfaces;
using System.Linq;

namespace NewsCrawler.DailyMail
{
    public class DailyMailNewsArticleTitleFetcherService : INewsArticleTitleFetcherService
    {
        public string FetchTitle(string articleContent)
        {
            string title = null;
            var doc = new HtmlDocument();
            doc.LoadHtml(articleContent);

            var titleNode = doc.DocumentNode.Descendants().FirstOrDefault(n => n.Name == "h2");
            title = titleNode?.InnerText?.Trim();
            return title;
        }
    }
}
