using System;
using System.Linq;
using HtmlAgilityPack;
using NewsCrawler.Interfaces;
using NewsCrawler.Persistence;

namespace NewsCrawler.DailyMail
{
    public class DailyMailArticleCleaner : IArticleCleaner
    {
        public string CleanArticle(Article article)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(article.Content);

            var nodesToRemove = doc.DocumentNode.Descendants().Where(n => n.Name == "script" || n.Attributes.Any(attr => attr.Name == "class" && attr.Value == "off-screen")).ToArray();
            foreach(var node in nodesToRemove)
            {
                node.Remove();
            }
            var contentNode = doc.DocumentNode.Descendants().FirstOrDefault(n => n.Attributes.Any(attr => attr.Name == "id" && attr.Value == "js-article-text"));
            if (contentNode?.InnerText == null)
            {
                return string.Empty;
            }
            else
            {
                return string.Join(" ", contentNode.InnerText.Replace("\n", string.Empty).Split(' ', StringSplitOptions.RemoveEmptyEntries));
            }
        }
    }
}
