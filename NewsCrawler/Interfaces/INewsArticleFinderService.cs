using System.Collections.Generic;

namespace NewsCrawler.Interfaces
{
    public interface INewsArticleFinderService
    {
        IEnumerable<string> FindNewsArticles();
    }
}
