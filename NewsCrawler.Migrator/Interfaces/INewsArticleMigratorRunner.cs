using System.Threading.Tasks;

namespace NewsCrawler.Interfaces
{
    public interface INewsArticleMigratorRunner
    {
        Task RunMigrator();
    }
}
