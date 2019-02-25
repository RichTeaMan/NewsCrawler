using System;
using System.ComponentModel.DataAnnotations;

namespace NewsCrawler.WebUI.Models
{
    public class ArticleDetail : Article
    {

        public string Content { get; set; }

        public string CleanedContent { get; set; }

        public ArticleDetail() : base() { }

        public ArticleDetail(Persistence.Article article) : base(article)
        {
            Content = article.Content;
            CleanedContent = article.CleanedContent;
        }
    }
}
