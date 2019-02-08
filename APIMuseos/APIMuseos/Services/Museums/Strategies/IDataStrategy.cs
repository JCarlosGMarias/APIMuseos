
using System.Threading.Tasks;

namespace APIMuseos.Services.Museums.Strategies
{
    public interface IDataStrategy
    {
        Task FetchData();
        void WatchMuseums();
        void WatchVisitedMuseums();
    }
}
