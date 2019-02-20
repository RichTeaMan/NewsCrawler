using System.Threading.Tasks;

namespace NewsCrawler.Interfaces
{
    public interface IArticleUpdaterRunner
    {
        Task RunTitleUpdater(string urlContains);
    }
}
