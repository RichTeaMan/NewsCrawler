using System;
using System.Collections.Generic;
using System.Text;

namespace NewsCrawler
{
    public class NewsArticleDeterminationService : INewsArticleDeterminationService
    {
        public bool IsNewsArticle(string articleLink)
        {
            return articleLink?.StartsWith("/news/") == true;
        }
    }
}
