using System;
using System.ComponentModel.DataAnnotations;

namespace NewsCrawler.WebUI.Models
{
    public class Article
    {
        public string Title { get; set; }

        [DisplayFormat(DataFormatString = "{dd/MM/yyyy}")]
        public DateTimeOffset RecordedDate { get; set; }

        public string Link { get; set; }
    }
}
