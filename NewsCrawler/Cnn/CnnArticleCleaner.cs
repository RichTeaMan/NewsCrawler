using System;
using System.Linq;
using System.Web;
using HtmlAgilityPack;
using NewsCrawler.Interfaces;
using NewsCrawler.Persistence;

namespace NewsCrawler.Cnn
{
    public class CnnArticleCleaner : IArticleCleaner
    {
        public string CleanArticle(Article article)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(article.Content);

            var nodesToRemove = doc.DocumentNode.Descendants().Where(n => n.Name == "script"
                || n.Name == "cite"
                || n.Name == "strong"
                || n.Attributes.Any(attr => attr.Name == "class" && attr.Value == "metadata")
                || n.Attributes.Any(attr => attr.Name == "class" && attr.Value.Contains("featured-video-collection"))
                || n.Attributes.Any(attr => attr.Name == "class" && attr.Value.Contains("media__video")))
            .ToArray();
            foreach(var node in nodesToRemove)
            {
                node.Remove();
            }
            var contentNode = doc.DocumentNode.Descendants().FirstOrDefault(n => n.Attributes.Any(attr => attr.Name == "itemprop" && attr.Value == "articleBody"));
            if (contentNode?.InnerText == null)
            {
                return string.Empty;
            }
            else
            {
                foreach (var paragraph in contentNode.Descendants().Where(n => n.Name == "div" && n.Attributes.Any(a => a.Name == "class" && a.Value == "zn-body__paragraph")))
                {
                    // Paragraphs don't always end with a space so the space trimming doesn't work well. This hacks a space in.
                    paragraph.InnerHtml = paragraph.InnerHtml + " ";
                }
                var cleanedArticle = string.Join(" ", contentNode.InnerText.Replace("\n", string.Empty).Split(' ', StringSplitOptions.RemoveEmptyEntries));
                return HttpUtility.HtmlDecode(cleanedArticle);
            }
        }
    }
}
