using System.Threading.Tasks;

namespace NewsCrawler.Interfaces
{
    public interface INewsArticleFetcherRunner
    {
        Task RunFetcher();
    }
}
