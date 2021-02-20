using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;

namespace NewsCrawler.Bbc
{
    public class BbcArticleCleaner : AbstractArticleCleaner
    {
        protected override HtmlNode FindArticleNode(HtmlNode htmlNode)
        {
            var nodesToRemove = htmlNode.Descendants().Where(n => n.Name == "script" || n.Attributes.Any(attr => attr.Name == "class" && attr.Value == "off-screen")).ToArray();
            foreach (var node in nodesToRemove)
            {
                node.Remove();
            }
            var contentNode = htmlNode.Descendants().FirstOrDefault(n => n.Attributes.Any(attr => attr.Name == "class" && attr.Value == "story-body__inner"));

            if (contentNode == null)
            {
                var articleWrapper = htmlNode.Descendants().FirstOrDefault(n => n.Name == "article");

                if (articleWrapper != null)
                {
                    // remove final sections
                    var dataComponentsToRemove = new HashSet<string>(new[] {
                        "tag-list",
                        "see-alsos",
                        "related-internet-link"
                    });
                    var sectionsToRemove = articleWrapper.Descendants().Where(n => n.Attributes.Any(attr => attr.Name == "data-component" && dataComponentsToRemove.Contains(attr.Value))).ToArray();

                    foreach (var section in sectionsToRemove)
                    {
                        section.Remove();
                    }
                }
                contentNode = articleWrapper;
            }

            return contentNode;
        }

        protected override IEnumerable<HtmlNode> FindParagraphNodes(HtmlNode articleNode)
        {
            var paragraphs = articleNode.Descendants().Where(n => n.Name == "p" || n.Name == "h2");
            return paragraphs;
        }
    }
}
