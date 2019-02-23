using HtmlAgilityPack;
using NewsCrawler.Interfaces;
using NewsCrawler.Persistence;
using System;
using System.Linq;

namespace NewsCrawler.Cnn
{
    public class CnnArticleTitleFetcherService : INewsArticleTitleFetcherService
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
            return htmlNode.Name == "h1" && htmlNode.Attributes.Any(attr => attr.Name == "class" && (attr.Value?.Contains("pg-headline") == true));
        }
    }
}
