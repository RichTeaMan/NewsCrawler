using System;
using System.ComponentModel.DataAnnotations;

namespace NewsCrawler.WebUI.Models
{
    public class Article
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string NewsSource { get; set; }

        public bool IsIndexPage { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy h:mm tt}")]
        public DateTimeOffset RecordedDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy h:mm tt}")]
        public DateTimeOffset? PublishedDate { get; set; }

        public string Link { get; set; }

        public int ContentLength { get; set; }

        public int CleanedContentLength { get; set; }

        public Article() { }

        public Article(Persistence.Article article)
        {
            Id = article.Id;
            Title = article.Title;
            NewsSource = article.NewsSource;
            IsIndexPage = article.IsIndexPage;
            RecordedDate = article.RecordedDate;
            PublishedDate = article.PublishedDate;
            Link = article.Url;
            ContentLength = article.ContentLength;
            CleanedContentLength = article.CleanedContentLength;
        }

        public Article(Persistence.Postgres.Article article)
        {
            Id = article.Id;
            Title = article.Title;
            NewsSource = article.NewsSource;
            IsIndexPage = article.IsIndexPage;
            RecordedDate = article.RecordedDate;
            PublishedDate = article.PublishedDate;
            Link = article.Url;
            ContentLength = article.ContentLength;
            CleanedContentLength = article.CleanedContentLength;
        }
    }
}
