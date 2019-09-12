using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace NewsCrawler.Persistence.Postgres
{
    public class Article
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(Constants.MAX_URL_LENGTH)]
        public string Url { get; set; }

        [MaxLength(Constants.MAX_TITLE_LENGTH)]
        public string Title { get; set; }

        [Required]
        [Column(TypeName = "text")]
        public string Content { get; set; }

        public int ContentLength { get; set; }

        [Column(TypeName = "text")]
        public string CleanedContent { get; set; }

        public int CleanedContentLength { get; set; }

        [Required]
        public DateTimeOffset RecordedDate { get; set; }
        
        public DateTimeOffset? PublishedDate { get; set; }

        [Required]
        public bool IsIndexPage { get; set; }

        [Required]
        [MaxLength(Constants.MAX_NEWS_SOURCE_LENGTH)]
        public string NewsSource { get; set; }
    }
}
