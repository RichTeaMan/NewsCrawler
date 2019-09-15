using System.Threading.Tasks;
using NewsCrawler.Persistence.Postgres;

namespace NewsCrawler.Interfaces
{
    public interface INewsArticleFetchService
    {
        Task<Article> FetchArticleAsync(string url);
    }
}
