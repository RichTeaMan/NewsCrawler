using HtmlAgilityPack;
using System.Linq;

namespace NewsCrawler
{
    public class BbcNewsArticleTitleFetcherService : INewsArticleTitleFetcherService
    {
        public string FetchTitle(string articleContent)
        {
            string title = null;
            var doc = new HtmlDocument();
            doc.LoadHtml(articleContent);

            var titleNode = doc.DocumentNode.Descendants().FirstOrDefault(n => HasTitleAttribute(n));
            title = titleNode?.InnerText?.Trim();
            if (title == null)
            {
                title = doc.DocumentNode.Descendants().FirstOrDefault(n => n.Name == "title")?.InnerText?.Trim()?.Replace(" - BBC News", string.Empty);
            }
            return title;
        }

        private bool HasTitleAttribute(HtmlNode htmlNode)
        {
            return htmlNode.Attributes.Any(attr => attr.Name == "class" && (attr.Value == "story-body__h1" || attr.Value == "vxp-media__headline"));
        }
    }
}
