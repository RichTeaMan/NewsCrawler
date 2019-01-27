using System;
using System.ComponentModel.DataAnnotations;

namespace NewsCrawler.WebUI.Models
{
    public class Article
    {
        public string Title { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy h:mm tt}")]
        public DateTimeOffset RecordedDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy h:mm tt}")]
        public DateTimeOffset? PublishedDate { get; set; }

        public string Link { get; set; }
    }
}
