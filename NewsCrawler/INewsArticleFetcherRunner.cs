using System.Threading.Tasks;

namespace NewsCrawler
{
    public interface INewsArticleFetcherRunner
    {
        Task RunFetcher();
    }
}