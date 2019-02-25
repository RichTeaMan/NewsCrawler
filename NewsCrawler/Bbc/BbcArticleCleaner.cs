using System;
using System.Linq;
using System.Web;
using HtmlAgilityPack;
using NewsCrawler.Interfaces;
using NewsCrawler.Persistence;

namespace NewsCrawler.Bbc
{
    public class BbcArticleCleaner : IArticleCleaner
    {
        public string CleanArticle(string articleContent)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(articleContent);

            var nodesToRemove = doc.DocumentNode.Descendants().Where(n => n.Name == "script" || n.Attributes.Any(attr => attr.Name == "class" && attr.Value == "off-screen")).ToArray();
            foreach(var node in nodesToRemove)
            {
                node.Remove();
            }
            var contentNode = doc.DocumentNode.Descendants().FirstOrDefault(n => n.Attributes.Any(attr => attr.Name == "class" && attr.Value == "story-body__inner"));
            if (contentNode?.InnerText == null)
            {
                return string.Empty;
            }
            else
            {
                var cleanedArticle = string.Join(" ", contentNode.InnerText.Replace("\r", string.Empty).Replace("\n", string.Empty).Split(' ', StringSplitOptions.RemoveEmptyEntries));
                return HttpUtility.HtmlDecode(cleanedArticle);
            }
        }
    }
}
