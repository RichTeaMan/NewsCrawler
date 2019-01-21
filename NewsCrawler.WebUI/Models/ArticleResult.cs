using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsCrawler.WebUI.Models
{
    public class ArticleResult
    {
        public IEnumerable<Article> ArticleList { get; set; }

        public int ArticleCount { get; set; }

        public string SearchTerm { get; set; } = string.Empty;

        public int Page { get; set; }

        public int TotalPages { get; set; }
    }
}
