using System;
using System.Collections.Generic;
using System.Web;
using HtmlAgilityPack;
using NewsCrawler.Interfaces;

namespace NewsCrawler
{
    public abstract class AbstractArticleCleaner : IArticleCleaner
    {
        protected abstract HtmlNode FindArticleNode(HtmlNode htmlNode);

        protected abstract IEnumerable<HtmlNode> FindParagraphNodes(HtmlNode articleNode);

        public string CleanArticle(string articleContent)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(articleContent);

            var contentNode = FindArticleNode(doc.DocumentNode);
            if (contentNode?.InnerText == null)
            {
                return string.Empty;
            }
            else
            {
                foreach (var paragraph in FindParagraphNodes(contentNode))
                {
                    // Paragraphs don't always end with a space so the space trimming doesn't work well. This hacks a space in.
                    paragraph.InnerHtml = paragraph.InnerHtml + " #P#";
                }
                var cleanedArticle = string.Join(" ", contentNode.InnerText.Replace("\r", string.Empty).Replace("\n", string.Empty).Split(' ', StringSplitOptions.RemoveEmptyEntries)).Replace("#P#", "\n\n");
                return HttpUtility.HtmlDecode(cleanedArticle);
            }
        }
    }
}
