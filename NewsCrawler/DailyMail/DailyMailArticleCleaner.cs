using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;

namespace NewsCrawler.DailyMail
{
    public class DailyMailArticleCleaner : AbstractArticleCleaner
    {
        protected override HtmlNode FindArticleNode(HtmlNode htmlNode)
        {
            var nodesToRemove = htmlNode.Descendants().Where(n => n.Name == "script" || n.Attributes.Any(attr => attr.Name == "class" && attr.Value == "off-screen")).ToArray();
            foreach (var node in nodesToRemove)
            {
                node.Remove();
            }
            var contentNode = htmlNode.Descendants().FirstOrDefault(n => n.Attributes.Any(attr => attr.Name == "itemprop" && attr.Value == "articleBody"));
            return contentNode;
        }

        protected override IEnumerable<HtmlNode> FindParagraphNodes(HtmlNode articleNode)
        {
            var paragraphs = articleNode.Descendants().Where(n => n.Name == "p");
            return paragraphs;
        }
    }
}
