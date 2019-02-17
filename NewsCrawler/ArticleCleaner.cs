using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using NewsCrawler.Persistence;

namespace NewsCrawler
{
    public class ArticleCleaner : IArticleCleaner
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
            var contentNode = doc.DocumentNode.Descendants().FirstOrDefault(n => n.Attributes.Any(attr => attr.Name == "class" && attr.Value == "story-body__inner"));
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
