using HtmlAgilityPack;
using NewsCrawler.Interfaces;
using NewsCrawler.Persistence;
using System;
using System.Linq;

namespace NewsCrawler.NewYorkTimes
{
    public class NewYorkTimesArticleTitleFetcherService : INewsArticleTitleFetcherService
    {
        public string FetchTitle(string articleContent)
        {
            string title = null;
            var doc = new HtmlDocument();
            doc.LoadHtml(articleContent);

            var titleNode = doc.DocumentNode.Descendants().FirstOrDefault(n => HasTitleAttribute(n));
            title = titleNode?.InnerText?.Trim();
            return title;
        }

        private bool HasTitleAttribute(HtmlNode htmlNode)
        {
            return htmlNode.Attributes.Any(attr => attr.Name == "itemprop" && attr.Value == "headline");
        }
    }
}
