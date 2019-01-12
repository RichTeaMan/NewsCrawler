using System.Threading.Tasks;
using NewsCrawler.Persistence;

namespace NewsCrawler
{
    public interface INewsArticleFetchService
    {
        Task<Article> FetchArticleAsync(string url);
    }
}