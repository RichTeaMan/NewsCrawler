using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsCrawler.WebUI.Models
{
    public class ArticleResult
    {
        public IEnumerable<Article> ArticleList { get; set; }
    }
}
