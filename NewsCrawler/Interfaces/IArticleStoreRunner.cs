using System.Threading.Tasks;

namespace NewsCrawler
{
    public interface IArticleStoreRunner
    {
        Task StoreArticles();
    }
}