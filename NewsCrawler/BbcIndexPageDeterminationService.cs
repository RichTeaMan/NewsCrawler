using NewsCrawler.Persistence;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace NewsCrawler
{
    public class BbcIndexPageDeterminationService : IIndexPageDeterminationService
    {
        public bool IsIndexPage(Article article)
        {
            return !Regex.IsMatch(article.Url, @"\d") && !article.Url.Contains("correspondents");
        }
    }
}
