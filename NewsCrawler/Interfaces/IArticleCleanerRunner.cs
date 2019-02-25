using System.Threading.Tasks;

namespace NewsCrawler
{
    public interface IArticleCleanerRunner
    {
        Task CleanArticles();
    }
}