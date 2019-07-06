using System.Collections.Generic;
using System.Linq;

namespace NewsCrawler.WebUI.Models
{
    public class ArticleDetail : Article
    {

        public string Content { get; }

        public string CleanedContent { get; }

        public WordFrequency[] Nouns { get; }

        public ArticleDetail() : base() { }

        public ArticleDetail(Persistence.Article article, IEnumerable<WordFrequency> nouns) : base(article)
        {
            Content = article.Content;
            CleanedContent = article.CleanedContent;
            Nouns = nouns.ToArray();
        }
    }
}
