using System.Collections.Generic;
using System.Linq;

namespace NewsCrawler.WebUI.Models
{
    public class ArticleDetail : Article
    {

        public string Content { get; }

        public string CleanedContent { get; }

        public WordFrequency[] Nouns { get; set; } = new WordFrequency[0];

        public bool DocumentScannerError { get; set; }

        public ArticleDetail(Persistence.Article article) : base(article)
        {
            Content = article.Content;
            CleanedContent = article.CleanedContent;
        }
    }
}
