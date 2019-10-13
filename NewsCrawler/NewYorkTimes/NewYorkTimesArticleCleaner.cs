using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;

namespace NewsCrawler.NewYorkTimes
{
    public class NewYorkTimesArticleCleaner : AbstractArticleCleaner
    {
        protected override HtmlNode FindArticleNode(HtmlNode htmlNode)
        {
            var nodesToRemove = htmlNode.Descendants().Where(n => n.Name == "script" || n.Attributes.Any(attr => attr.Name == "class" && attr.Value == "off-screen") || n.Name == "aside").ToArray();
            foreach (var node in nodesToRemove)
            {
                node.Remove();
            }
            var contentNode = htmlNode.Descendants().FirstOrDefault(n => n.Attributes.Any(attr => attr.Name == "name" && attr.Value == "articleBody"));
            return contentNode;
        }

        protected override IEnumerable<HtmlNode> FindParagraphNodes(HtmlNode articleNode)
        {
            var paragraphs = articleNode.Descendants().Where(n => n.Name == "p" || n.Name == "h2");
            return paragraphs;
        }
    }
}
