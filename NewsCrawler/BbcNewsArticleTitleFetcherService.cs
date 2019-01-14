using HtmlAgilityPack;
using System.Linq;

namespace NewsCrawler
{
    public class BbcNewsArticleTitleFetcherService : INewsArticleTitleFetcherService
    {
        public string FetchTitle(string articleContent)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(articleContent);

            var titleNode = doc.DocumentNode.Descendants().FirstOrDefault(n => HasTitleAttribute(n));
            return titleNode?.InnerText?.Trim();
        }

        private bool HasTitleAttribute(HtmlNode htmlNode)
        {
            return htmlNode.Attributes.Any(attr => attr.Name == "class" && (attr.Value == "story-body__h1" || attr.Value == "vxp-media__headline"));
        }
    }
}
