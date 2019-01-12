using System.Collections.Generic;

namespace NewsCrawler
{
    public interface INewsArticleFinderService
    {
        IEnumerable<string> FindNewsArticles();
    }
}