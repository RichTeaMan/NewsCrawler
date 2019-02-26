using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;

namespace NewsCrawler.Cnn
{
    public class CnnArticleCleaner : AbstractArticleCleaner
    {
        protected override HtmlNode FindArticleNode(HtmlNode htmlNode)
        {
            var nodesToRemove = htmlNode.Descendants().Where(n => n.Name == "script"
                || n.Name == "cite"
                || n.Name == "strong"
                || n.Attributes.Any(attr => attr.Name == "class" && attr.Value == "metadata")
                || n.Attributes.Any(attr => attr.Name == "class" && attr.Value.Contains("featured-video-collection"))
                || n.Attributes.Any(attr => attr.Name == "class" && attr.Value.Contains("media__video")))
            .ToArray();
            foreach (var node in nodesToRemove)
            {
                node.Remove();
            }
            var contentNode = htmlNode.Descendants().FirstOrDefault(n => n.Attributes.Any(attr => attr.Name == "itemprop" && attr.Value == "articleBody"));
            return contentNode;
        }

        protected override IEnumerable<HtmlNode> FindParagraphNodes(HtmlNode articleNode)
        {
            var paragraphs = articleNode.Descendants().Where(n => n.Name == "div" && n.Attributes.Any(a => a.Name == "class" && a.Value == "zn-body__paragraph"));
            return paragraphs;
        }
    }
}
