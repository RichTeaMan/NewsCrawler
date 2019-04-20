using System.Threading.Tasks;

namespace NewsCrawler.Interfaces
{
    public interface IWordCountService
    {
        Task UpdateWordCount();
    }
}